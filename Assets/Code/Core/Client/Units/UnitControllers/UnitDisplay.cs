using System;
using System.Collections;
using System.Linq;
using Code.Core.Client.Units.Managed;
using Code.Libaries.GameObjects;
using Code.Libaries.Generic.Managers;
using Code.Libaries.UnityExtensions.Independent;
using Libaries.Generic.Managers;
using Libaries.UnityExtensions.Independent;
using Shared.Content.Types;
using UnityEngine;

namespace Client.Units.UnitControllers
{
    public class UnitDisplay : MonoBehaviour
    {
        private const float MinWalkSpeed = 0.02f;
        private const float MaxRunSpeed = 1f;

        const float FadeOutTime = 0.25f;

        private PlayerUnit _unit;
        private Animation _animation;

        public Face Face { get; private set; }
        public GameObject Hair { get; set; }

        private PlayerUnit _lookAtUnit;
        private Vector3 _lookAtPositionLerped;

        private float _cachedWalkAnimLen = -1;
        private float _cachedRunAnimLen = -1;
        private int _model = -1;
        private bool _updateWalkRunStand = true;

        private Material _skin;
        private Material _underWear;
        private Material _boots;
        private Material _skirt;
        private Material _chest;

        public Transform NeckBone, BodyBone, Offhand, Mainhand,RightHand,LeftHand, LeftShoulder, RightShoulder;

        [SerializeField]
        private string _standAnimation;
        [SerializeField]
        private string _walkAnimation;
        [SerializeField]
        private string _runAnimation;
        private string _actionAnimation;

        public float LookStrenght = 0.5f;
        private bool _itemsRigid;
        private SkinnedMeshRenderer _bootsRenderer;
        private SkinnedMeshRenderer _skirtRenderer;
        private SkinnedMeshRenderer _chestRenderer;

        public const string RigidAnimations = "Rest Dead";

        public bool ItemsRigid
        {
            get { return _itemsRigid; }
            set
            {
                if (value != _itemsRigid)
                {
                    Item[] items = GetComponentsInChildren<Item>();
                    foreach (var item in items)
                    {
                        if (item != null && (item.transform.parent == Offhand || item.transform.parent == Mainhand))
                        {
                            var rigid = item.GetComponent<Rigidbody>();
                            if (rigid != null)
                            {
                                if (value)
                                {
                                    rigid.isKinematic = false;
                                }
                                else
                                {

                                    rigid.isKinematic = true;
                                    Item item1 = item;
                                    CorotineManager.Instance.StartCoroutine(Ease.Vector(
                                            item.transform.localPosition,
                                            new Vector3(-0.1f, 0, -0.06f),
                                            vector3 => item1.transform.localPosition = vector3)
                                            );

                                    Item item2 = item;
                                    CorotineManager.Instance.StartCoroutine(Ease.Vector(
                                            item.transform.localEulerAngles,
                                            new Vector3(270, 0, 0),
                                            vector3 => item2.transform.localEulerAngles = vector3)
                                            );

                                }
                            }
                        }
                    }
                }
                _itemsRigid = value;
            }
        }

        public string StandAnimation
        {
            get { return _standAnimation; }
            set
            {
                if (_standAnimation != value)
                {
                    if (_animation[value] == null)
                    {
                        Debug.LogError("missing animation named ["+value+"]");
                        return;
                    }
                    _animation[value].wrapMode = WrapMode.Loop;
                    //Turn off old anim
                    if(_animation.GetClip(_standAnimation) != null)
                        _animation.Blend(_standAnimation, 0f, FadeOutTime);

                    bool shouldBeRigid = false;

                    if (RigidAnimations.Split(" "[0]).Any(anim => anim == value))
                    {
                        ItemsRigid = true;
                        shouldBeRigid = true;
                    }

                    if (!shouldBeRigid)
                    {
                        ItemsRigid = false;
                    }
                }
                _standAnimation = value;
            }
        }

        public string WalkAnimation
        {
            get { return _walkAnimation; }
            set
            {
                if (_walkAnimation != value)
                {
                    _animation[value].wrapMode = WrapMode.Loop;

                    //turn off old anim
                    if (_animation.GetClip(_walkAnimation) != null)
                        _animation.Blend(_walkAnimation, 0f, FadeOutTime);
                }
                _walkAnimation = value;
            }
        }

        public string RunAnimation
        {
            get { return _runAnimation; }
            set
            {
                if (_runAnimation != value)
                {
                    _animation[value].wrapMode = WrapMode.Loop;

                    //turn off old anim
                    if (_animation.GetClip(_runAnimation) != null)
                        _animation.Blend(_runAnimation, 0f, FadeOutTime);
                }
                _runAnimation = value;
            }
        }

        public string ActionAnimation
        {
            get { return _actionAnimation; }
            set
            {
                if (!string.IsNullOrEmpty(_actionAnimation))
                    if (_animation.IsPlaying(_actionAnimation))
                    {
                        _animation.Blend(_actionAnimation, 0);
                    }
                _actionAnimation = value;

                StartCoroutine(RunAcionAnimation(value));

            }
        }

        public PlayerUnit LookAtUnit
        {
            get
            {
                return _lookAtUnit;
            }
            set
            {
                if (_lookAtUnit == null)
                {
                    _lookAtPositionLerped = transform.position + transform.forward * 10f;
                }
                _lookAtUnit = value;
            }
        }

        private UnitPrefab _inUseModel;
        public Action<int> OnModelChange;

        public int Model
        {
            get { return _model; }
            set
            {
                if (_model != value)
                {
                    _model = value;

                    if (_model > ContentManager.I.Models.Count)
                    {
                        Debug.Log("Invalid model Id: " + value);
                        return;
                    }
                    if (_inUseModel != null)
                    {
                        Destroy(_inUseModel.gameObject);
                    }
                    _inUseModel = Instantiate(ContentManager.I.Models[_model].gameObject).GetComponent<UnitPrefab>();
                    SetModel(_inUseModel);
                    if (OnModelChange != null)
                        OnModelChange(value);
                }
            }
        }

        public UnitPrefab UnitPrefab
        {
            get { return _inUseModel; }
        }

        public int HeadId { get; private set; }
        public int LegsId { get; private set; }
        public int ChestId { get; private set; }
        public int BootsId { get; private set; }
        public int MainHandId { get; private set; }
        public int OffHandId { get; private set; }

        public event Action OnEquipmentChanged;

        public void Awake()
        {
            _unit = GetComponent<PlayerUnit>();
            _lookAtUnit = null;
            _lookAtPositionLerped = Vector3.zero;
            HeadId = -1;
            LegsId = -1;
            ChestId = -1;
            BootsId = -1;
            MainHandId = -1;
            OffHandId = -1;
        }

        public virtual void Update()
        {
            if (_animation != null)
            {
                ProcessWalkAndStand();
            }
        }

        public virtual void LateUpdate()
        {
            if (_animation != null)
            {
                ProcessLookAtRotation();
            }
        }

        /// <summary>
        /// Processes the walk and stand animations.
        /// </summary>
        private void ProcessWalkAndStand()
        {
            if (!_updateWalkRunStand)
                return;

            float speed = _unit.VisualSpeed;

            float weightStand = speed < MinWalkSpeed ? 1f : 0f;
            float weightWalk = (speed > MinWalkSpeed ? 1f - (speed / MaxRunSpeed) : 0f) * 1.3f;
            float weightRun = speed > MinWalkSpeed ? 1f - weightWalk : 0f;

            _animation.Blend(StandAnimation, weightStand, FadeOutTime);
            _animation.Blend(WalkAnimation, weightWalk, FadeOutTime);
            _animation.Blend(RunAnimation, weightRun, FadeOutTime);

            /*if (speed <= MinWalkSpeed)
            {
                _animation.Blend(StandAnimation, 1f, FadeOutTime);
                _animation.Blend(WalkAnimation, 0f, FadeOutTime);
                _animation.Blend(RunAnimation, 0f, FadeOutTime);
            }*
            else
            {
                /*
                float walkSpeed = 0.7f + weightRun * 0.7f;
                if (speed <= (MaxRunSpeed) / 2f)
                {
                    _animation[WalkAnimation].speed = walkSpeed;
                    _animation[RunAnimation].speed = walkSpeed;
                    _animation.Blend(StandAnimation, 0f, FadeOutTime);
                    _animation.Blend(WalkAnimation, 1f, FadeOutTime);
                    _animation.Blend(RunAnimation, 0, FadeOutTime);
                }
                else
                {
                    _animation[WalkAnimation].speed = walkSpeed;
                    _animation[RunAnimation].speed = walkSpeed;
                    _animation.Blend(StandAnimation, 0f, FadeOutTime);
                    _animation.Blend(WalkAnimation, weightWalk, FadeOutTime);
                    _animation.Blend(RunAnimation, weightRun, FadeOutTime);
                }
                
            }*/
        }

        void ProcessLookAtRotation()
        {
            if (!_updateWalkRunStand)
                return;

            bool lookAtItReally = _lookAtUnit != null && _lookAtUnit.gameObject != gameObject && (Vector3.Distance(transform.position + transform.forward, _lookAtUnit.transform.position) <
                                                          Vector3.Distance(transform.position + transform.forward * -1f, _lookAtUnit.transform.position) ||
                                                          _lookAtUnit == _unit);

            _lookAtPositionLerped = Vector3.Lerp(_lookAtPositionLerped, !lookAtItReally ? transform.position + transform.forward * 10f : _lookAtUnit.transform.position, Time.deltaTime * 10);
            LookAtBone(BodyBone, LookStrenght);
            LookAtBone(NeckBone, LookStrenght);
        }

        void LookAtBone(Transform bone, float strenght)
        {
            if (bone != null)
            {
                Vector3 euler = Quaternion.LookRotation(_lookAtPositionLerped - bone.position, Vector3.up).eulerAngles;
                euler.z -= 90;
                Quaternion rot = Quaternion.Lerp(Quaternion.Euler(euler), bone.rotation, strenght);
                bone.rotation = rot;
            }
        }

        public void SetModel(UnitPrefab model)
        {
            _animation = model.Visual.GetComponent<Animation>();

            //Copy animations from male to female
            if (_model == 0)
            {
                var maleAnim = ContentManager.I.Models[1].GetComponent<Animation>();
                foreach (AnimationState animClip in maleAnim)
                {
                    _animation.AddClip(maleAnim.GetClip(animClip.name), animClip.name);
                }
            }


            model.transform.parent = transform;
            model.transform.localPosition = Vector3.zero;

            NeckBone = TransformHelper.FindTraverseChildren("Neck", model.Visual.transform);
            BodyBone = TransformHelper.FindTraverseChildren("Body", model.Visual.transform);

            
            LeftShoulder = TransformHelper.FindTraverseChildren("LeftShoulder", model.Visual.transform);
            RightShoulder = TransformHelper.FindTraverseChildren("RightShoulder", model.Visual.transform);

            //If its human male or female model
            if (_model == 0 || _model == 1)
            {
                Mainhand = TransformHelper.FindTraverseChildren("MainHand", model.Visual.transform);
                Offhand = TransformHelper.FindTraverseChildren("OffHand", model.Visual.transform);
                RightHand = Mainhand.parent;
                LeftHand = Offhand.parent;

                _animation["HandRightHold"].wrapMode = WrapMode.Clamp;
                _animation["HandRightHold"].layer = 4;
                _animation["HandRightHold"].AddMixingTransform(RightHand);

                //First create an face.
                Face = UnitFactory.Instance.CreateFace(this);

                //Lets get the existing materials and create new instances of them so we can change the equipment
                _chestRenderer = TransformHelper.FindTraverseChildren("BodyMesh", model.Visual.transform).GetComponent<SkinnedMeshRenderer>();

                _chest = Instantiate(_chestRenderer.materials[0]);
                _chestRenderer.materials[0] = _chest;
                _chestRenderer.enabled = false;

                _bootsRenderer = TransformHelper.FindTraverseChildren("Boots", model.Visual.transform).GetComponent<SkinnedMeshRenderer>();

                _boots = Instantiate(_bootsRenderer.materials[0]);
                _bootsRenderer.materials[0] = _boots;
                _bootsRenderer.enabled = false;

                _skirtRenderer = TransformHelper.FindTraverseChildren("SkirtMesh", model.Visual.transform).GetComponent<SkinnedMeshRenderer>();

                _skirt = Instantiate(_skirtRenderer.materials[0]);
                _skirtRenderer.materials[0] = _skirt;
                _skirtRenderer.enabled = false;

                var skinMesh = TransformHelper.FindTraverseChildren("SkinMesh", model.Visual.transform).GetComponent<SkinnedMeshRenderer>();

                var mats = skinMesh.materials;

                _underWear = Instantiate(skinMesh.materials[0]);
                mats[0] = _underWear;

                _skin = Instantiate(skinMesh.materials[1]);
                mats[1] = _skin;
                skinMesh.materials = mats;

                //Hence the skin should be same as on ears, pass the material
                Face.EarMaterial = _skin;

                var wep = TransformHelper.FindTraverseChildren("ExampleWeapon", model.Visual.transform);
                var shield = TransformHelper.FindTraverseChildren("ExampleShield", model.Visual.transform);

                //Remove Example weapons
                if (wep != null)
                    Destroy(wep.gameObject);
                if (shield != null)
                    Destroy(shield.gameObject);
            }
            if (_animation != null)
            {
                StandAnimation = "Idle";
                WalkAnimation = "Walk";
                RunAnimation = "Run";
            }
        }

        private bool IsPowerAnim(string id)
        {
            if (string.IsNullOrEmpty(id)) return false;

            if (id.Contains("Power"))
            {
                return true;
            }
            return false;
        }
        private bool IsAttackAnim(string id)
        {
            if (string.IsNullOrEmpty(id)) return false;

            if ((id.Contains("Power") || id.Contains("Attack")) && !id.Contains("Full") || id.Contains("Shot"))
            {
                return true;
            }
            return false;
        }
        private bool IsStrongAnim(string id)
        {
            if (string.IsNullOrEmpty(id)) return false;

            if (id.Contains("Strong"))
            {
                return true;
            }
            return false;
        }

        IEnumerator RunAcionAnimation(string animationName)
        {

            if (_animation == null || _animation.GetClip(animationName) == null)
                yield break;
            if (!IsAttackAnim(animationName) || IsStrongAnim(animationName))
            {

                /*if (!string.IsNullOrEmpty(_lastActivatedPowerAnim))
                {
                    animation.Blend(_lastActivatedPowerAnim, 0, FADE_OUT_TIME);
                    _lastActivatedPowerAnim = "";
                }*/

                _updateWalkRunStand = false;

                _animation.Blend(StandAnimation, 0);
                _animation.Blend(WalkAnimation, 0);
                _animation.Blend(RunAnimation, 0);

                _animation.Blend(animationName, 1, FadeOutTime);
                yield return new WaitForSeconds(_animation[animationName].length - FadeOutTime);
                _animation.Blend(animationName, 0, FadeOutTime);

                _updateWalkRunStand = true;

            }
            else
            {
                _animation[animationName].layer = 1;
                _animation[animationName].AddMixingTransform(BodyBone);
                if(IsPowerAnim(animationName))
                _animation[animationName].wrapMode = WrapMode.ClampForever; 
                _animation.Blend(animationName, 1, FadeOutTime / (animationName.Contains("Power") ? 0.5f : 3f));
            }
        }

        /// <summary>
        /// Update the units equipment.
        /// </summary>
        /// <param name="head">Unit id</param>
        /// <param name="body">Unit id</param>
        /// <param name="legs">Unit id</param>
        /// <param name="boots">Unit id</param>
        /// <param name="mainHand">Unit id</param>
        /// <param name="offHand">Unit id</param>
        public void EquipItems(int head, int body, int legs, int boots, int mainHand, int offHand)
        {
            HeadId = head;
            ChestId = body;
            LegsId = legs;
            BootsId = boots;
            MainHandId = mainHand;
            OffHandId = offHand;

            if (OnEquipmentChanged != null)
            {
                OnEquipmentChanged();
            }

            /*EquipItemUnit(head, NeckBone);
            EquipItemUnit(mainHand, Mainhand);
            EquipItemUnit(offHand, Offhand);*/
            if(mainHand == -1)
                _animation.Play("HandRightHold");
            else
                _animation.Stop("HandRightHold");

            if (BootsId == -1)
            {
                _bootsRenderer.enabled = false;
            }
            else
            {
                _bootsRenderer.enabled = true;

                try
                {
                    _boots.mainTexture =
                        ContentManager.I.Items[BootsId].GetComponentInChildren<Renderer>().material.mainTexture;
                }
                catch (ArgumentOutOfRangeException)
                {
                    Debug.LogError("un existing boots item id: " + BootsId);
                }
            }

            if (ChestId == -1)
            {
                _chestRenderer.enabled = false;
            }
            else
            {
                _chestRenderer.enabled = true;

                try
                {
                    _chest.mainTexture =
                        ContentManager.I.Items[ChestId].GetComponentInChildren<Renderer>().material.mainTexture;
                }
                catch (ArgumentOutOfRangeException)
                {
                    Debug.LogError("un existing chest item id: " + ChestId);
                }
            }

            if (LegsId == -1)
            {
                _skirtRenderer.enabled = false;
            }
            else
            {

                Item item = null;
                try
                {
                    item = ContentManager.I.Items[LegsId];
                }
                catch (ArgumentOutOfRangeException)
                {
                    Debug.LogError("un existing legs item id: " + LegsId);
                }
                // ReSharper disable once PossibleNullReferenceException
                var meshRenderer = item.transform.GetChild(0).GetComponent<MeshRenderer>();
                Material material = meshRenderer.material;

                _skirtRenderer.enabled = true;
                _skirt.mainTexture = material.GetTexture(0);
            }
        }

        public void SetCharacterCustoms(int[] customs)
        {
            //FACE
            try
            {
                var faceCollection = _model == 1 ? HumanModelConfig.I.MaleFaceTextures : HumanModelConfig.I.FemaleFaceTextures;

                Face.FaceRenderer.material = new Material(Face.FaceRenderer.material)
                {
                    mainTexture = faceCollection[customs[2]]
                };
                Face.FaceRenderer.material.SetColor("Red", HumanModelConfig.I.FaceColors[customs[3]]);
            }
            catch (Exception e) { Debug.LogException(e); }

            //Hair
            try
            {
                var hairCollection = _model == 1 ? HumanModelConfig.I.MaleHairs : HumanModelConfig.I.FemaleHairs;

                if (Hair != null)
                {
                    Destroy(Hair);
                }

                Hair = Instantiate(hairCollection[customs[1]].gameObject);


                var hairRenderer = Hair.GetComponent<MeshRenderer>();
                if (hairRenderer != null)
                {
                    if (hairRenderer.material != null)
                    {
                        hairRenderer.material = new Material(hairRenderer.material)
                        {
                            color = HumanModelConfig.I.HairsColors[customs[0]]
                        };
                    }
                }

                Hair.transform.parent = NeckBone;
                Hair.transform.localPosition = Vector3.zero + new Vector3(-0.12f, 0, -0.03f);
                Hair.transform.localRotation = Quaternion.identity;
                Hair.transform.localEulerAngles = new Vector3(0, 270, 90);
                Hair.transform.localScale = Vector3.one;
            }
            catch (Exception e) { Debug.LogException(e); }

            //Skin
            try
            {
                _skin.color = HumanModelConfig.I.SkinColors[customs[4]];
            }
            catch (Exception e) { Debug.LogException(e); }

            //Underwear
            try
            {
                _underWear.color = HumanModelConfig.I.UnderWearColors[customs[5]];
            }
            catch (Exception e) { Debug.LogException(e); }
        }
    }
}
