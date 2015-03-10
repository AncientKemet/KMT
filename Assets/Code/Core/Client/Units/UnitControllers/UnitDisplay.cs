using System;
using System.Collections;
using Client.Units;
using Code.Core.Client.Units.Extensions;
using Code.Core.Client.Units.Managed;
using Code.Core.Shared.Content;
using Code.Core.Shared.Content.Types;
using Code.Core.Shared.Content.Types.ItemExtensions;
using Code.Libaries.GameObjects;
using Code.Libaries.Generic.Managers;
using Code.Libaries.UnityExtensions.Independent;
using Libaries.Generic.Managers;
using Shared.Content.Types;
using UnityEngine;

namespace Code.Core.Client.Units.UnitControllers
{
    public class UnitDisplay : MonoBehaviour
    {
        [SerializeField]
        private float MaxMovementSpeed = 0.2f;

        const float FADE_OUT_TIME = 0.25f;

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

        public Transform NeckBone, BodyBone, LeftHand, RightHand, LeftShoulder, RightShoulder;

        private string _standAnimation = "Idle";
        private string _walkAnimation = "Walk";
        private string _runAnimation = "Run";
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
                        if (item != null && (item.transform.parent == LeftHand || item.transform.parent ==  RightHand))
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
                                            vector3 => item1.transform.localPosition = vector3, null, 0.3f)
                                            );

                                    Item item2 = item;
                                    CorotineManager.Instance.StartCoroutine(Ease.Vector(
                                            item.transform.localEulerAngles,
                                            new Vector3(270, 0, 0),
                                            vector3 => item2.transform.localEulerAngles = vector3, null, 0.3f)
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
                    _animation.Blend(_standAnimation, 0f, FADE_OUT_TIME);

                    bool shouldBeRigid = false;

                    foreach (var anim in RigidAnimations.Split(" "[0]))
                    {
                        if (anim == value)
                        {
                            ItemsRigid = true;
                            shouldBeRigid = true;
                            break;
                        }
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
                    _animation.Blend(_walkAnimation, 0f, FADE_OUT_TIME);
                }
                _walkAnimation = value;
            }
        }

        public string RunAnimation
        {
            get { return _runAnimation; }
            set { _runAnimation = value; }
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

                StartCoroutine(FocusAnimation(value));

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

        private GameObject _InUseModel;
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
                    if (_InUseModel != null)
                    {
                        Destroy(_InUseModel);
                    }
                    _InUseModel = (GameObject) Instantiate(ContentManager.I.Models[_model]);
                    SetModel(_InUseModel);
                    if (OnModelChange != null)
                        OnModelChange(value);
                }
            }
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
        /// Processes the walk and stand.
        /// </summary>
        private void ProcessWalkAndStand()
        {
            if (!_updateWalkRunStand)
                return;

            float speed = _unit.VisualSpeed;
            float maxSpeed = MaxMovementSpeed;
            float weightRun = _unit.VisualSpeed / maxSpeed;
            float weightWalk = 1f - weightRun;

            if (speed <= 0.017f)
            {
                _animation.Blend(StandAnimation, 1f, FADE_OUT_TIME);
                _animation.Blend(WalkAnimation, 0f, FADE_OUT_TIME);
                _animation.Blend(RunAnimation, 0f, FADE_OUT_TIME);
            }
            else
            {
                float walkSpeed = 0.7f + weightRun * 0.7f;
                if (speed <= MaxMovementSpeed / 2f)
                {
                    _animation[WalkAnimation].speed = walkSpeed;
                    _animation[RunAnimation].speed = walkSpeed;
                    _animation.Blend(StandAnimation, 0f, FADE_OUT_TIME);
                    _animation.Blend(WalkAnimation, 1f, FADE_OUT_TIME);
                    _animation.Blend(RunAnimation, 0, FADE_OUT_TIME);
                }
                else
                {
                    _animation[WalkAnimation].speed = walkSpeed;
                    _animation[RunAnimation].speed = walkSpeed;
                    _animation.Blend(StandAnimation, 0f, FADE_OUT_TIME);
                    _animation.Blend(WalkAnimation, weightWalk, FADE_OUT_TIME);
                    _animation.Blend(RunAnimation, weightRun, FADE_OUT_TIME);
                }
            }
        }

        void ProcessLookAtRotation()
        {
            if (!_updateWalkRunStand)
                return;

            bool lookAtItReally = true;
            if (_lookAtUnit == null ||
                Vector3.Distance(transform.position + transform.forward, _lookAtUnit.transform.position) >
                Vector3.Distance(transform.position + transform.forward * -1f, _lookAtUnit.transform.position) ||
                _lookAtUnit == _unit)
            {
                lookAtItReally = false;
            }

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

        public void PlayAnimation(string id, int layer, float strenght)
        {
            if (id != "-1")
                if (_animation != null)
                {
                    if (_animation[id] == null)
                    {
                        Debug.LogError("missing anim id: " + id);
                        return;
                    }

                    _animation[id].layer = layer;
                    _animation.Blend(id, strenght, FADE_OUT_TIME);
                    if (layer == 1)
                    {
                        _animation[id].AddMixingTransform(BodyBone);
                    }
                    if (layer == 2)
                    {
                        _animation[id].AddMixingTransform(LeftShoulder);
                    }
                    if (layer == 3)
                    {
                        _animation[id].AddMixingTransform(RightShoulder);
                    }
                }
        }

        public void PlayAnimation(string id, int layer)
        {
            PlayAnimation(id, layer, 1f);
        }

        public void SetModel(GameObject model)
        {
            _animation = model.GetComponent<Animation>();

            //Copy animations from male to female
            if (_model == 0)
            {
                var maleAnim = ContentManager.I.Models[1].animation;
                foreach (AnimationState animClip in maleAnim)
                {
                    _animation.AddClip(maleAnim.GetClip(animClip.name), animClip.name);
                }
            }


            model.transform.parent = transform;
            model.transform.localPosition = Vector3.zero;

            NeckBone = TransformHelper.FindTraverseChildren("Neck", model.transform);
            BodyBone = TransformHelper.FindTraverseChildren("Body", model.transform);

            RightHand = TransformHelper.FindTraverseChildren("RightHand", model.transform);
            LeftHand = TransformHelper.FindTraverseChildren("LeftHand", model.transform);
            LeftShoulder = TransformHelper.FindTraverseChildren("LeftShoulder", model.transform);
            RightShoulder = TransformHelper.FindTraverseChildren("RightShoulder", model.transform);

            if (_animation != null)
            {
                _animation[StandAnimation].wrapMode = WrapMode.Loop;
                _animation[RunAnimation].wrapMode = WrapMode.Loop;
                _animation[WalkAnimation].wrapMode = WrapMode.Loop;
            }

            //If its human male or female model
            if (_model == 0 || _model == 1)
            {
                //First create an face.
                Face = UnitFactory.Instance.CreateFace(this);

                //Lets get the existing materials and create new instances of them so we can change the equipment
                _chestRenderer = TransformHelper.FindTraverseChildren("BodyMesh", model.transform).GetComponent<SkinnedMeshRenderer>();

                _chest = (Material)Instantiate(_chestRenderer.materials[0]);
                _chestRenderer.materials[0] = _chest;
                _chestRenderer.enabled = false;

                _bootsRenderer = TransformHelper.FindTraverseChildren("Boots", model.transform).GetComponent<SkinnedMeshRenderer>();

                _boots = (Material)Instantiate(_bootsRenderer.materials[0]);
                _bootsRenderer.materials[0] = _boots;
                _bootsRenderer.enabled = false;

                _skirtRenderer = TransformHelper.FindTraverseChildren("SkirtMesh", model.transform).GetComponent<SkinnedMeshRenderer>();

                _skirt = (Material)Instantiate(_skirtRenderer.materials[0]);
                _skirtRenderer.materials[0] = _skirt;
                _skirtRenderer.enabled = false;

                SkinnedMeshRenderer skinMesh = TransformHelper.FindTraverseChildren("SkinMesh", model.transform).GetComponent<SkinnedMeshRenderer>();

                var mats = skinMesh.materials;

                _underWear = (Material)Instantiate(skinMesh.materials[0]);
                mats[0] = _underWear;

                _skin = (Material)Instantiate(skinMesh.materials[1]);
                mats[1] = _skin;
                skinMesh.materials = mats;

                //Hence the skin should be same as on ears, pass the material
                Face.EarMaterial = _skin;

                var wep = TransformHelper.FindTraverseChildren("ExampleWeapon", model.transform);
                var shield = TransformHelper.FindTraverseChildren("ExampleShield", model.transform);
                //Remove Example weapons
                if(wep != null)
                Destroy(TransformHelper.FindTraverseChildren("ExampleWeapon", model.transform).gameObject);
                if(shield != null)
                Destroy(TransformHelper.FindTraverseChildren("ExampleShield", model.transform).gameObject);
                
            }
        }

        private bool IsAttackAnim(string id)
        {
            if (string.IsNullOrEmpty(id)) return false;

            if ((id.Contains("Power") || id.Contains("Attack")) && !id.Contains("Full"))
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

        IEnumerator FocusAnimation(string animationName)
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

                _animation.Blend(animationName, 1, FADE_OUT_TIME);
                yield return new WaitForSeconds(_animation[animationName].length - FADE_OUT_TIME);
                _animation.Blend(animationName, 0, FADE_OUT_TIME );

                _updateWalkRunStand = true;

            }
            else
            {
                _animation[animationName].layer = 1;
                _animation[animationName].AddMixingTransform(BodyBone);
                _animation.Blend(animationName, 1, FADE_OUT_TIME/ (animationName.Contains("Power") ? 0.5f : 3f));
            }
        }

        /// <summary>
        /// Update the units equipment.
        /// </summary>
        /// <param name="head">item id</param>
        /// <param name="body">item id</param>
        /// <param name="legs">item id</param>
        /// <param name="boots">item id</param>
        /// <param name="mainHand">item id</param>
        /// <param name="offHand">item id</param>
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

            EquipItemGameObject(head, NeckBone);
            EquipItemGameObject(mainHand, RightHand);
            EquipItemGameObject(offHand, LeftHand);

            if (BootsId == -1)
            {
                _bootsRenderer.enabled = false;
            }
            else
            {
                _bootsRenderer.enabled = true;
                _boots.mainTexture =
                    ContentManager.I.Items[BootsId].GetComponentInChildren<Renderer>().material.mainTexture;
            }

            if (ChestId == -1)
            {
                _chestRenderer.enabled = false;
            }
            else
            {
                _chestRenderer.enabled = true;
                _chest.mainTexture =
                    ContentManager.I.Items[ChestId].GetComponentInChildren<Renderer>().material.mainTexture;
            }

            if (LegsId == -1)
            {
                _skirtRenderer.enabled = false;
            }
            else
            {

                Item item = ContentManager.I.Items[LegsId];
                MeshRenderer renderer = item.transform.GetChild(0).GetComponent<MeshRenderer>();
                Material material = renderer.material;

                _skirtRenderer.enabled = true;
                _skirt.mainTexture = material.GetTexture(0);
            }
        }

        private void EquipItemGameObject(int itemId, Transform rootBone)
        {
            Item item = rootBone.GetComponentInChildren<Item>();
            if (item != null)
            {
                if (item.InContentManagerIndex != itemId)
                {
                    Destroy(item.gameObject);
                }
                else
                {
                    return;
                }
            }
            if(_animation != null)
            if (rootBone == RightHand)
            {
                try
                {
                    string HoldAnimName = "HandRight" + (itemId == -1 ? "Free" : "Hold");
                    _animation[HoldAnimName].layer = 2;
                    foreach (var v in RightHand)
                    {
                        Transform t = (Transform) v;
                        _animation[HoldAnimName].AddMixingTransform(t);
                    }
                    _animation.Blend(HoldAnimName, 1, FADE_OUT_TIME);
                }
                catch (Exception e)
                {
                    
                }
            }
            if (itemId != -1)
            {
                GameObject newItem = (GameObject)Instantiate(ContentManager.I.Items[itemId].gameObject);
                newItem.transform.parent = rootBone;

                    var rigid = newItem.GetComponentInChildren<Rigidbody>();
                    if (rigid != null)
                    {
                        rigid.isKinematic = true;
                    }

                //Exceptions  hands
                if (rootBone == LeftHand || rootBone == RightHand)
                {
                    newItem.transform.localPosition = new Vector3(-0.1f, 0, -0.06f);
                    newItem.transform.localEulerAngles = new Vector3(270, 0, 0);
                    newItem.transform.localScale = Vector3.one;
                }
                else
                {
                    newItem.transform.localPosition = Vector3.zero;
                    newItem.transform.localRotation = Quaternion.identity;
                    newItem.transform.localScale = Vector3.one;
                }
            }
        }

        public void SetCharacterCustoms(int[] customs)
        {
            //FACE
            try
            {
                var FaceCollection = _model == 1 ? HumanModelConfig.I.MaleFaceTextures : HumanModelConfig.I.FemaleFaceTextures;

                Face.FaceRenderer.material = new Material(Face.FaceRenderer.material);
                Face.FaceRenderer.material.mainTexture = FaceCollection[customs[2]];
                Face.FaceRenderer.material.SetColor("Red", HumanModelConfig.I.FaceColors[customs[3]]);
            }
            catch (Exception e) { Debug.LogException(e); }

            //Hair
            try
            {
                var HairCollection = _model == 1 ? HumanModelConfig.I.MaleHairs : HumanModelConfig.I.FemaleHairs;

                if (Hair != null)
                {
                    Destroy(Hair);
                }

                Hair = (GameObject) Instantiate(HairCollection[customs[1]].gameObject);
                

                var _hairRenderer = Hair.GetComponent<MeshRenderer>();
                if (_hairRenderer != null)
                {
                    if (_hairRenderer.material != null)
                    {
                        _hairRenderer.material = new Material(_hairRenderer.material);
                        _hairRenderer.material.color = HumanModelConfig.I.HairsColors[customs[0]];
                    }
                }

                Hair.transform.parent = NeckBone;
                Hair.transform.localPosition = Vector3.zero + new Vector3(-0.12f, 0 , -0.03f);
                Hair.transform.localRotation = Quaternion.identity;
                Hair.transform.localEulerAngles = new Vector3(0, 270, 90);
                Hair.transform.localScale = Vector3.one;
            }
            catch (Exception e) { Debug.LogException(e);}

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
