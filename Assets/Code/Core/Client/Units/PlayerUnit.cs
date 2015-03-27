using System;
using System.Collections;
using System.Collections.Generic;
using Client.Enviroment;
using Client.Net;
using Client.UI.Interfaces;
using Code.Code.Libaries.Net;
using Code.Core.Client.UI.Controls;
using Code.Core.Client.UI.Interfaces;
using Code.Core.Client.UI.Interfaces.UpperLeft;
using Code.Core.Client.Units.Extensions;
using Code.Core.Client.Units.Managed;
using Code.Core.Client.Units.UnitControllers;
using Code.Core.Shared.Content.Types;
using Code.Core.Shared.Content.Types.ItemExtensions;
using Code.Libaries.Generic.Managers;
using Code.Libaries.Net.Packets.ForClient;
using Code.Libaries.Net.Packets.ForServer;
using Code.Libaries.UnityExtensions.Independent;
using Libaries.Net.Packets.ForClient;
using Libaries.UnityExtensions.Independent;
using Shared.Content.Types;
using Shared.Content.Types.ItemExtensions;
using UnityEngine;

namespace Client.Units
{
    [RequireComponent(typeof(UnitDisplay))]
    public class PlayerUnit : Clickable
    {
        [SerializeField]
        private int _id = -1;

        [SerializeField] private int _modelIndex = -1;

        private UnitDisplay _display;

        private bool IsStatic
        {
            get { return _isStatic; }
            set
            {
                _isStatic = value;
                this.enabled = !value;
                Display.enabled = !value;
                if (collider != null)
                    collider.enabled = !value;
            }
        }

        private static PlayerUnit _myPlayerUnitInstance;

        [SerializeField]
        private tk2dTextMesh _2dNameLabel;

        private string _name;

        [SerializeField]
        protected float _basemovementSpeed = 1;
        
        protected Vector3 movementTargetPosition;
        protected Vector3 smoothedTargetPosition;
        protected float targetRotation;

        private Projector _projector;

        [SerializeField]
        protected float distanceToTarget;

        [SerializeField]
        protected float _visualSpeed;

        private bool _isStatic;
        private int _parentId = -1;
        private int _parentPlaneId = -1;

        public float Health { get; private set; }
        public float Energy { get; private set; }
        public float MaxHealth { get; private set; }
        public float MaxEnergy { get; private set; }
        public float HealthRegen { get; private set; }
        public float EnergyRegen { get; private set; }

        public List<BuffInstance> BuffInstances { get; private set; }
        public event Action<BuffInstance> OnBuffWasAdded;
        public event Action<BuffInstance> OnBuffWasRemoved;

        public int Id
        {
            get
            {
                return _id;
            }
            set
            {
                _id = value;
            }
        }

        public int ParentId
        {
            get { return _parentId; }
            set
            {
                if (_parentId != value)
                {
                    if (value == -1)
                    {
                        transform.parent = KemetMap.Instance.transform;
                    }
                    else
                    {
                        if (_parentPlaneId == -1)
                        {
                            if (UnitManager.Instance.HasUnit(value))
                            {
                                var parent = UnitManager.Instance[value];
                                StartCoroutine(Ease.Join(
                                    transform,
                                    parent.transform,
                                    () => { transform.parent = parent.transform; },
                                    0.3f));
                            }
                            else
                            {
                                //todo wait for parent to come visible
                            }
                        }
                    }
                }
                _parentId = value;
            }
        }

        public int ParentPlaneId
        {
            get { return _parentPlaneId; }
            set
            {
                if (_parentPlaneId != value)
                {
                    if (_parentId != -1)
                    {
                        if (UnitManager.Instance.HasUnit(_parentId))
                        {
                            var parent = UnitManager.Instance[_parentId];
                            var plane = parent.Display.UnitPrefab.Planes[value];
                            StartCoroutine(Ease.Join(
                                    transform,
                                    plane,
                                    () => { transform.parent = plane; },
                                    0.3f));
                        }
                        else
                        {
                            //todo wait for parent to come visible
                        }
                    }
                }
                _parentPlaneId = value;
            }
        }

        public static PlayerUnit MyPlayerUnit
        {
            get { return _myPlayerUnitInstance; }
            set { _myPlayerUnitInstance = value; }
        }

        public string Name
        {
            get { return _name; }
            set
            {
                _name = value;
            }
        }

        /// <summary>
        /// Gets the current movement speed.
        /// </summary>
        /// <value>The current movement speed.</value>
        public float CurrentMovementSpeed
        {
            get
            {
                return _basemovementSpeed;
            }
        }

        /// <summary>
        /// Gets or sets the movement target DirecionVector.
        /// The value has to be in parent's local space;
        /// </summary>
        /// <value>The movement target DirecionVector.</value>
        public Vector3 MovementTargetPosition
        {
            get
            {
                return movementTargetPosition;
            }
            set
            {
                movementTargetPosition = value;
            }
        }

        public float VisualSpeed
        {
            get
            {
                return _visualSpeed;
            }
        }

        public float TargetRotation
        {
            get { return targetRotation; }
            set { targetRotation = value; }
        }

        public UnitDisplay Display
        {
            get
            {
                if (_display == null)
                    _display = GetComponent<UnitDisplay>();
                return _display;
            }
        }

        public Projector Projector
        {
            get
            {
                if (_projector == null)
                {
                    _projector = UnitFactory.Instance.CreateProjector(this);
                    _projector.orthoGraphicSize = 0.66f * transform.localScale.x;
                }
                return _projector;
            }
        }

        public void AddBuff(BuffInstance b)
        {
            if (BuffInstances == null)
                BuffInstances = new List<BuffInstance>();

            var existing = BuffInstances.Find(instance => instance.Index == b.Index);
            if(existing != null)
                RemoveBuff(existing);

            BuffInstances.Add(b);

            if (OnBuffWasAdded != null)
                OnBuffWasAdded(b);
        }

        public void RemoveBuff(BuffInstance b)
        {
            if (BuffInstances == null)
                BuffInstances = new List<BuffInstance>();

            BuffInstances.Remove(BuffInstances.Find(instance => instance.Index == b.Index));

            if (OnBuffWasRemoved != null)
                OnBuffWasRemoved(b);
        }

        protected override void Start()
        {
            base.Start();
            
            if (_projector != null)
            {
                _projector.gameObject.SetActive(false);
                _projector.material = (Material) Instantiate(_projector.material);
            }

            OnLeftClick += delegate
            {
                if (!CreateCharacterInterface.IsNull)
                    return;

                if(UnitSelectionInterface.IsNull)
                    return;

                if (UnitSelectionInterface.I.Unit != this)
                    UnitSelectionInterface.I.Unit = this;
                else
                {
                    if(this == null)
                        return;
                    
                    if (Actions[0].Action != null)
                    {
                        Actions[0].Action();
                        GameObject effect = (GameObject) Instantiate((ContentManager.I.Effects[0]));
                        effect.transform.parent = transform;
                        effect.transform.localPosition = Vector3.zero;
                    }
                }
            };

            OnRightClick += () =>
            {
                if (!CreateCharacterInterface.IsNull)
                    return;

                if (this == null)
                    return;

                if (UnitSelectionInterface.I.Unit == this)
                {
                    OpenRightClickMenu();
                }
            };

            
            OnMouseIn += delegate
            {
                DescriptionInterface.I.Show(name);
            };
            OnMouseOff += delegate
            {
                DescriptionInterface.I.Hide();
            };

            OnStart();
        }

        void Update()
        {
            OnUpdate();
        }
        
        void FixedUpdate()
        {
            /*if(this != MyPlayerUnit && MyPlayerUnit != null)
            if (Vector3.Distance(transform.position, MyPlayerUnit.transform.position) > 90)
            {
                Destroy(gameObject);
            }*/
            OnFixedUpdate();
        }

        protected virtual void OnStart()
        {
            MovementTargetPosition = transform.localPosition;
        }

        protected virtual void OnUpdate()
        {

            if(ParentId != -1)
                return;
            
            distanceToTarget = Vector2.Distance(new Vector2(transform.localPosition.x, transform.localPosition.z), new Vector2(movementTargetPosition.x, movementTargetPosition.z));
            _visualSpeed = Mathf.Clamp(distanceToTarget, 0f, _basemovementSpeed) ;

            smoothedTargetPosition = Vector3.Lerp(smoothedTargetPosition, movementTargetPosition, 1.3f);

            Vector3 calculatedPosition = transform.localPosition;

            if (distanceToTarget > 0.017f)
            {// Process DirecionVector
                calculatedPosition = Vector3.Lerp(calculatedPosition, smoothedTargetPosition, Time.deltaTime * 7.5f);

                FixYOnTerrain(ref calculatedPosition);
                transform.localPosition = calculatedPosition;
            }

            Quaternion calculatedRotation;

            {// Process rotation
                if (Mathf.Abs(targetRotation - transform.localEulerAngles.y) > 1f)
                {
                    calculatedRotation = Quaternion.Euler(new Vector3(0, TargetRotation, 0));
                    calculatedRotation.x = 0;
                    calculatedRotation.z = 0;

                    calculatedRotation = Quaternion.Lerp(transform.localRotation, calculatedRotation, 0.25f);
                    transform.localRotation = calculatedRotation;
                }
            }
        }

        protected virtual void OnFixedUpdate()
        {
        }

        public void DecodeUnitUpdate(UnitUpdatePacket p)
        {
            ByteStream b = p.SubPacketData;
            int mask = b.GetByte();

            BitArray bitArray = new BitArray(new[] { mask });

            bool movementUpdate = bitArray[0];
            bool displayUpdate = bitArray[1];
            bool combatUpdate = bitArray[2];
            bool animUpdate = bitArray[3];
            bool equipmentUpdate = bitArray[4];
#if DEBUG_NETWORK
            string log = "";
            log += "\n" + "Packet size " + b.GetSize();

            log += "\n" + "nMovementUpdate " + movementUpdate;
            log += "\n" + "ndisplayUpdate " + displayUpdate;
            log += "\n" + "combatUpdate " + combatUpdate;
            log += "\n" + "animUpdate " + animUpdate;
            Debug.Log(log);
#endif

            if (movementUpdate)
            {
                int mask2 = b.GetByte();
                bitArray = new BitArray(new[] { mask2 });

                bool positionUpdate = bitArray[0];
                bool rotationUpdate = bitArray[1];
                bool teleported = bitArray[2];
                bool correction = bitArray[3];
                bool parented = bitArray[4];
                if (correction)
                {
                    if (positionUpdate)
                    {
                        Vector3 pos = b.GetPosition6B();
                        if (!teleported && !IsStatic)
                        {
                            //has not teleported
                            MovementTargetPosition = pos;
                        }
                        else
                        {
                            //has teleported
                            MovementTargetPosition = pos;
                            FixYOnTerrain(ref pos);
                            transform.localPosition = pos;
                        }
                    }

                    if (rotationUpdate)
                    {
                        float rotation = b.GetAngle1B();
                        TargetRotation = rotation;
                    }
                }
                if (parented)
                {
                    this.ParentId = b.GetShort();
                    this.ParentPlaneId = b.GetByte();
                }

#if DEBUG_NETWORK
                log = "";
                log += "\n" + "post movement offset " + b.Offset;
                log += "\n" + "positionUpdate " + positionUpdate;
                log += "\n" + "rotationUpdate " + rotationUpdate;
                Debug.Log(log);
#endif
            }

            if (displayUpdate)
            {
                var displayMask = b.GetBitArray();
                int modelId = b.GetUnsignedByte();
                float size = b.GetFloat4B();

                bool isItem = displayMask[0];
                bool wasDestroyed = displayMask[1];
                IsStatic = displayMask[2];
                bool hasEffects = displayMask[3];
                bool _hasCharacterCustoms = displayMask[4];

                _modelIndex = modelId;

                if (!isItem)
                {
                    if (Display != null)
                    {
                        if (Display.Model != modelId)
                        {

                            /*StartCoroutine(Ease.Vector(Vector3.zero, Vector3.one * size, vector3 =>
                            {
                                transform.localScale = vector3;
                                if (_projector != null)
                                {
                                    _projector.orthoGraphicSize = vector3.x;
                                }
                            }, () => { },
                                0.3f));*/
                            Display.Model = modelId;
                            AddAction(new RightClickAction("Inspect",
                                delegate
                                {

                                    UnitActionPacket pac = new UnitActionPacket();
                                    pac.UnitId = Id;
                                    pac.ActionName = "Inspect";
                                    ClientCommunicator.Instance.SendToServer(pac);
                                }
                                ));
                        }
                    }
                }
                else
                {
                    Item item = ((GameObject) Instantiate(ContentManager.I.Items[modelId].gameObject)).GetComponent<Item>();
                    transform.localPosition += Vector3.up;
                    item.transform.parent = transform;
                    item.transform.localPosition = Vector3.zero;

                    if (_parentId == -1)
                    {
                        var rigid = item.GetComponent<ItemRigid>();

                        rigid.ForwardClicksToParent();
                        rigid.PhysicsEnabled = true;
                    }

                    if(collider != null)
                        collider.enabled = false;
                }

                if (wasDestroyed)
                {
                    var destroyMask = b.GetBitArray();

                    var wasPickuped = destroyMask[0];

                    if (wasPickuped)
                    {
                        int unitID = b.GetUnsignedShort();
                        PlayerUnit u = UnitManager.Instance[unitID];

                        if (u != null)
                        {
                            GetComponentInChildren<Item>().EnterUnit(u);
                            GetComponentInChildren<Item>().transform.parent = null;
                        }
                        else
                        {
                            Debug.Log("null unit id: "+unitID);
                        }
                    }
                    Debug.Log("destroying object :"+name);
                    Destroy(gameObject);
                }

                if (hasEffects)
                {
                    int count = b.GetUnsignedByte();
                    for (int i = 0; i < count; i++)
                    {
                        try
                        {
                            GameObject newEffect =
                                (GameObject) Instantiate(ContentManager.I.Effects[b.GetUnsignedByte()]);
                            newEffect.transform.parent = transform;
                            newEffect.transform.localPosition = Vector3.zero;
                            newEffect.transform.localScale = Vector3.one;
                        }
                        catch (Exception e)
                        {
                            Debug.LogException(e);
                        }
                    }
                }

                if (_hasCharacterCustoms)
                {
                    int len = b.GetUnsignedByte();
                    int[] customs = new int[len];
                    for (int i = 0; i < len; i++)
                    {
                        customs[i] = b.GetUnsignedByte();
                    }

                    Display.SetCharacterCustoms(customs);
                }

                transform.localScale = Vector3.one*size;

#if DEBUG_NETWORK
                log = "";
                log += "\n" + "post display offset " + b.Offset;
                log += "\n" + "modelId " + modelId;
                log += "\n" + "isItem " + isItem;
                Debug.Log(log);
#endif
            }

            if (combatUpdate)
            {
                int health = b.GetUnsignedByte();
                int maxhealth = b.GetUnsignedByte();
                int energy = b.GetUnsignedByte();
                int maxenergy = b.GetUnsignedByte();
                
                Health = health;
                MaxHealth = maxhealth;
                Energy = energy;
                MaxEnergy = maxenergy;

                var combat = GetComponent<CombatUnit>();

                if (this == MyPlayerUnit)
                {
                    StatsBarInterfaces.I.HPBar.Progress = (float)((float)health / ((float)maxhealth + 0.01f));
                    StatsBarInterfaces.I.ENBar.Progress = ((float)energy / ((float)maxenergy + 0.01f));
                }
                if (combat != null)
                {
                    combat.SetHealth(health);
                    combat.SetEnergy(energy);
                }

#if DEBUG_NETWORK
                log = "";
                log += "\n" + "post combat offset " + b.Offset;
                log += "\n" + "health " + health;
                log += "\n" + "energy " + energy;
                Debug.Log(log);
#endif
            }

            if (animUpdate)
            {
                int mask2 = b.GetByte();

                var bitArray2 = new BitArray(new[] { mask2 });

#if DEBUG_NETWORK
                log = "";
                log += "\n" + "pre anim offset " + b.Offset;
                log += "\n" + "bitArray2 " + bitArray2;
                Debug.Log(log);
#endif

                if (bitArray2[0])
                {
                    string a = b.GetString();
                    if (Display != null)
                        Display.StandAnimation = a;
                }
                if (bitArray2[1])
                {
                    string a = b.GetString();
                    if (Display != null)
                        Display.WalkAnimation = a;
                }
                if (bitArray2[2])
                {
                    string a = b.GetString();
                    if (Display != null)
                        Display.RunAnimation = a;
                }
                if (bitArray2[3])
                {
                    string a = b.GetString();
                    if (Display != null)
                        Display.ActionAnimation = a;
                }

                int lookingAtUnitID = b.GetShort();

                Display.LookAtUnit = lookingAtUnitID == -1 ? null : UnitManager.Instance.GetUnit(lookingAtUnitID);
            }

            if (equipmentUpdate)
                Display.EquipItems(b.GetShort(), b.GetShort(), b.GetShort(), b.GetShort(), b.GetShort(), b.GetShort());

        }
        
        public static bool GetBit(int byt, int index)
        {
            if (index < 0 || index > 7)
                throw new ArgumentOutOfRangeException();

            int shift = 7 - index;

            // Get a single bit in the proper DirecionVector.
            byte bitMask = (byte)(1 << shift);

            // Mask out the appropriate bit.
            byte masked = (byte)(byt & bitMask);

            // If masked != 0, then the masked out bit is 1.
            // Otherwise, masked will be 0.
            return masked != 0;
        }

        protected void FixYOnTerrain(ref Vector3 position)
        {
            Ray ray = new Ray(position + new Vector3(0, 50, 0), Vector3.down);
            RaycastHit hit = new RaycastHit();
            int layerMask = 1 << 8;
            //layerMask = ~layerMask;
            if (Physics.Raycast(ray, out hit, 100.0f, layerMask))
            {
                position.y = hit.point.y;
            }
        }

        public void OnUnprecieseMovement(UDPUnprecieseMovement p)
        {
            float angleInRadians = p.Angle / (180 / Mathf.PI);
            Vector3 vector = new Vector3(Mathf.Cos(angleInRadians), 0, Mathf.Sin(angleInRadians));
            movementTargetPosition = movementTargetPosition + (vector * p.Distance);
            TargetRotation = p.Face;
        }
    }
}
