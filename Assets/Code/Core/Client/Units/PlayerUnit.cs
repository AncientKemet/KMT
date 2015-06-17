using System;
using System.Collections;
using System.Collections.Generic;
using Client.Enviroment;
using Client.Net;
using Client.UI.Interfaces;
using Client.Units.UnitControllers;
using Code.Code.Libaries.Net;
using Code.Core.Client.UI.Controls;
using Code.Core.Client.UI.Interfaces;
using Code.Core.Client.UI.Interfaces.UpperLeft;
using Code.Core.Client.Units.Managed;
using Code.Libaries.Generic.Managers;
using Code.Libaries.Net.Packets.ForClient;
using Code.Libaries.Net.Packets.ForServer;
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

        private UnitDisplay _display;

        private bool IsStatic
        {
            get { return _isStatic; }
            set
            {
                _isStatic = value;
                enabled = !value;
                Display.enabled = !value;
                if (GetComponent<Collider>() != null)
                    GetComponent<Collider>().enabled = !value;
            }
        }


        protected Vector3 _movementTargetPosition;
        protected Vector3 _smoothedTargetPosition;
        protected float _targetRotation;

        private Projector _projector;
        protected float _distanceToTarget;
        [SerializeField]
        protected float _visualSpeed = 0.2f;

        private bool _isStatic;
        private int _parentId = -1;
        private int _parentPlaneId = -1;

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

        public bool IsFlying { get; set; }

        public static PlayerUnit MyPlayerUnit { get; set; }

        public string Name
        {
            get { return name; }
            set
            {
                name = value;
                gameObject.name = name;
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
                return 1f;
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
                return _movementTargetPosition;
            }
            set
            {
                _movementTargetPosition = value;
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
            get { return _targetRotation; }
            set { _targetRotation = value; }
        }

        public UnitDisplay Display
        {
            get { return _display ?? (_display = GetComponent<UnitDisplay>()); }
        }

        public Projector Projector
        {
            get
            {
                if (_projector == null)
                {
                    _projector = UnitFactory.Instance.CreateProjector(this);
                    _projector.orthographicSize = 0.66f * transform.localScale.x;
                }
                return _projector;
            }
        }

        public PlayerUnitAttributes PlayerUnitAttributes
        {
            get { return _playerUnitAttributes; }
        }

        public void AddBuff(BuffInstance b)
        {
            if (BuffInstances == null)
                BuffInstances = new List<BuffInstance>();

            var existing = BuffInstances.Find(instance => instance.Index == b.Index);
            if (existing != null)
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
                _projector.material = Instantiate(_projector.material);
            }

            OnLeftClick += delegate
            {
                if (!CreateCharacterInterface.IsNull)
                    return;

                if (UnitSelectionInterface.IsNull)
                    return;

                if (UnitSelectionInterface.I.Unit != this)
                    UnitSelectionInterface.I.Unit = this;
                else
                {
                    // ReSharper disable once ConditionIsAlwaysTrueOrFalse
                    if (this == null)
                        return;

                    if (Actions[0].Action != null)
                    {
                        Actions[0].Action();
                        GameObject effect = Instantiate((ContentManager.I.Effects[0]));
                        effect.transform.parent = transform;
                        effect.transform.localPosition = Vector3.zero;
                    }
                }
            };

            OnRightClick += () =>
            {
                if (!CreateCharacterInterface.IsNull)
                    return;

                // ReSharper disable once ConditionIsAlwaysTrueOrFalse
                if (this == null)
                    return;

                if (UnitSelectionInterface.I.Unit == this)
                {
                    OpenRightClickMenu();
                }
            };


            OnMouseIn += () => DescriptionInterface.I.Show(Name);
            OnMouseOff += () => DescriptionInterface.I.Hide();

            MovementTargetPosition = transform.localPosition;
        }

        protected virtual void Update()
        {

            {// Process rotation
                if (Mathf.Abs(_targetRotation - transform.localEulerAngles.y) > 1f)
                {
                    var calculatedRotation = Quaternion.Euler(new Vector3(0, TargetRotation, 0));
                    calculatedRotation.x = 0;
                    calculatedRotation.z = 0;

                    calculatedRotation = Quaternion.Lerp(transform.localRotation, calculatedRotation, 0.25f);
                    transform.localRotation = calculatedRotation;
                }
            }

            if (_parentId != -1)
                return;

            _distanceToTarget = Vector2.Distance(new Vector2(transform.localPosition.x, transform.localPosition.z), new Vector2(_movementTargetPosition.x, _movementTargetPosition.z));
            _visualSpeed = Mathf.Clamp(_distanceToTarget, 0f, CurrentMovementSpeed);

            _smoothedTargetPosition = Vector3.Lerp(_smoothedTargetPosition, _movementTargetPosition, 1.3f);

            Vector3 calculatedPosition = transform.localPosition;

            if (_distanceToTarget > 0.017f)
            {// Process DirecionVector
                calculatedPosition = Vector3.Lerp(calculatedPosition, _smoothedTargetPosition, Time.deltaTime * 7.5f);

                if (!IsFlying)
                    FixYOnTerrain(ref calculatedPosition);
                transform.localPosition = calculatedPosition;
            }
        }

        public void DecodeUnitUpdate(UnitUpdatePacket p)
        {
            ByteStream b = p.SubPacketData;

            var bitArray = b.GetBitArray();

            bool movementUpdate = bitArray[0];
            bool displayUpdate = bitArray[1];
            bool combatUpdate = bitArray[2];
            bool animUpdate = bitArray[3];
            bool equipmentUpdate = bitArray[4];
            bool detailsUpdate = bitArray[5];

            if (movementUpdate)
            {
                int mask2 = b.GetByte();
                var movementMask = new BitArray(new[] { mask2 });

                bool teleported = movementMask[0];
                bool parentUpdate = movementMask[1];
                IsFlying = movementMask[2];

                Vector3 pos = b.GetPosition6B();
                if (!teleported && !IsStatic)
                {
                    MovementTargetPosition = pos;
                }
                else
                {
                    MovementTargetPosition = pos;
                    transform.localPosition = pos;
                }

                float rotation = b.GetAngle1B();
                TargetRotation = rotation;


                if (parentUpdate)
                {
                    _parentId = b.GetShort();
                    _parentPlaneId = b.GetByte();
                    if (_parentId != -1)
                    {
                        if (UnitManager.Instance[_parentId].Display.Model != -1)
                        {
                            JoinParent();
                        }
                        else
                        {
                            UnitManager.Instance[_parentId].Display.OnModelChange += JoinParent;
                        }
                    }
                    else
                    {
                        transform.parent = KemetMap.Instance.transform;
                    }
                }
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
                bool _visible = displayMask[5];

                if (!isItem)
                {
                    if (Display != null)
                    {
                        if (Display.Model != modelId)
                        {
                            Display.Model = modelId;
                        }
                    }
                }
                else
                {
                    if (item != null)
                        Destroy(item.gameObject);
                    else
                        transform.localPosition += Vector3.up;

                    item = (Instantiate(ContentManager.I.Items[modelId].gameObject)).GetComponent<Item>();
                    item.transform.parent = transform;
                    item.transform.localPosition = Vector3.zero;
                    item.transform.localRotation = Quaternion.identity;

                    if (_parentId == -1)
                    {
                        var rigid = item.GetComponent<ItemRigid>();

                        rigid.ForwardClicksToParent();
                        rigid.PhysicsEnabled = true;
                    }

                    if (GetComponent<Collider>() != null)
                        GetComponent<Collider>().enabled = false;
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
                            Debug.Log("null unit id: " + unitID);
                        }
                    }
                    if (OnBeforeDestroy != null)
                        OnBeforeDestroy();
                    Destroy(gameObject);
                }

                if (hasEffects)
                {
                    int count = b.GetUnsignedByte();
                    for (int i = 0; i < count; i++)
                    {
                        try
                        {
                            var newEffect =
                                Instantiate(ContentManager.I.Effects[b.GetUnsignedByte()]);
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
                    var customs = new int[len];
                    for (int i = 0; i < len; i++)
                    {
                        customs[i] = b.GetUnsignedByte();
                    }

                    Display.SetCharacterCustoms(customs);
                }

                gameObject.SetActive(_visible);

                transform.localScale = Vector3.one * size;
            }

            if (combatUpdate)
            {
                BitArray mask = b.GetBitArray();

                bool _hpUpdate = mask[0], _enUpdate = mask[1], _attributesUpdate = mask[2];

                if (_hpUpdate)
                    PlayerUnitAttributes.CurrentHealth = b.GetUnsignedByte();

                if (_enUpdate)
                    PlayerUnitAttributes.CurrentEnergy = b.GetUnsignedByte();

                if (_attributesUpdate)
                {
                    int count = b.GetUnsignedByte();

                    for (int i = 0; i < count; i++)
                    {
                        var property = (UnitAttributeProperty)b.GetByte();
                        float value = b.GetShort() / 100f;

                        PlayerUnitAttributes.SetAttribute(property, value);
                    }
                }

            }

            if (animUpdate)
            {
                var bitArray2 = b.GetBitArray();
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

                Display.LookAtUnit = lookingAtUnitID == -1 ? null : UnitManager.Instance[lookingAtUnitID];
            }

            if (equipmentUpdate)
                Display.EquipItems(b.GetShort(), b.GetShort(), b.GetShort(), b.GetShort(), b.GetShort(), b.GetShort());

            if (detailsUpdate)
            {
                Name = b.GetString();
                int _count = b.GetByte();
                ClearAllActions("Cancel");
                if (_count > 0)
                    for (int i = 0; i < _count; i++)
                    {
                        string action = b.GetString();
                        AddAction(new RightClickAction(
                            action,
                            delegate
                            {
                                // ReSharper disable once ConditionIsAlwaysTrueOrFalse
                                if (this != null)
                                {
                                    var packet = new UnitActionPacket { UnitId = Id, ActionName = action };
                                    ClientCommunicator.Instance.SendToServer(packet);
                                }
                            }
                            ));
                    }
            }

        }

        private void JoinParent(int obj)
        {
            JoinParent();
        }

        private void JoinParent()
        {
            var parent = UnitManager.Instance[_parentId];
            var plane = _parentPlaneId == -1 ? parent.transform : parent.Display.UnitPrefab.Planes[_parentPlaneId];

            gameObject.SetActive(true);
            StartCoroutine(Ease.Join(
                transform,
                plane,
                () =>
                {
                    StartCoroutine(Ease.Join(
                          transform,
                          plane,
                          () =>
                          {
                              transform.parent = plane;
                              transform.localPosition = Vector3.zero;
                              transform.localRotation = Quaternion.identity;
                              transform.localScale = Vector3.one;
                          },
                          0.3f));
                },
                0.3f));

            if (parent.Display.OnModelChange != null)
                parent.Display.OnModelChange -= JoinParent;
        }

        public static bool GetBit(int byt, int index)
        {
            if (index < 0 || index > 7)
                throw new ArgumentOutOfRangeException();

            int shift = 7 - index;

            // Get a single bit in the proper DirecionVector.
            var bitMask = (byte)(1 << shift);

            // Mask out the appropriate bit.
            var masked = (byte)(byt & bitMask);

            // If masked != 0, then the masked out bit is 1.
            // Otherwise, masked will be 0.
            return masked != 0;
        }

        protected void FixYOnTerrain(ref Vector3 position)
        {
            var ray = new Ray(position + new Vector3(0, 50, 0), Vector3.down);
            RaycastHit hit;
            const int layerMask = 1 << 8;
            //layerMask = ~layerMask;
            if (Physics.Raycast(ray, out hit, 100.0f, layerMask))
            {
                position.y = hit.point.y;
            }
        }

        public void OnUnprecieseMovement(UDPUnprecieseMovement p)
        {

            if (p.Mask[0])
            {
                _movementTargetPosition = _movementTargetPosition + p.Difference;
            }
            else
            {
                float angleY = p.YAngle / (180 / Mathf.PI);
                _movementTargetPosition = _movementTargetPosition + (new Vector3(Mathf.Cos(angleY), 0, Mathf.Sin(angleY)) * p.Distance);
                TargetRotation = p.Face;
            }
        }

        public Action OnBeforeDestroy;
        private Item item;
        private readonly PlayerUnitAttributes _playerUnitAttributes = new PlayerUnitAttributes();
    }
}
