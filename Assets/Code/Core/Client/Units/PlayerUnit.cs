using System;
using System.Collections;
using System.Collections.Generic;
using Client.Enviroment;
using Client.Net;
using Client.UI.Interfaces;
using Client.Units.UnitControllers;
using Code.Code.Libaries.Net;
using Code.Core.Client.UI.Controls;
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
    public class PlayerUnit : Clickable
    {
        
        public static PlayerUnit MyPlayerUnit { get; set; }

        [SerializeField]
        private int _id = -1,_parentId = -1,_parentPlaneId = -1;
        private UnitDisplay _display;
        private Vector3 _movementTargetPosition, _smoothedTargetPosition;
        private float _targetRotation, _distanceToTarget, _visualSpeed = 0.2f;
        private Projector _projector,_fractionProjector;
        private Item item;
        private readonly PlayerUnitAttributes _playerUnitAttributes = new PlayerUnitAttributes();
        private Fraction _fraction;
        private List<BuffInstance> BuffInstances { get; set; }
        public event Action<BuffInstance> OnBuffWasAdded;
        public event Action<BuffInstance> OnBuffWasRemoved;
        public Fraction Fraction
        {
            get { return _fraction; }
            set
            {
                _fraction = value;
                if(_fractionProjector != null)
                    Destroy(_fractionProjector.gameObject);
                if(value != Fraction.Neutral)
                    _fractionProjector = UnitFactory.Instance.CreateFractionProjector(this);
            }
        }
        public ushort Id
        {
            get
            {
                return (ushort)_id;
            }
            set
            {
                _id = value;
            }
        }
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
        }
        public float VisualSpeed
        {
            get
            {
                return _visualSpeed;
            }
        }
        public UnitDisplay Display
        {
            get { return _display; }
        }
        public Projector Projector
        {
            get
            {
                if (_projector == null)
                {
                    _projector = UnitFactory.Instance.CreateTargetProjector(this);
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
                if (Actions != null && Actions.Count >= 1)
                    if (Actions[0] != null)
                        Actions[0].Action();
            };

            OnRightClick += () =>
            {
                OpenRightClickMenu();
            };


            OnMouseIn += () => DescriptionInterface.I.Show(Name);
            OnMouseOff += () => DescriptionInterface.I.Hide();

            _movementTargetPosition = transform.position;
        }
        protected virtual void Update()
        {

            if (Mathf.Abs(_targetRotation - transform.localEulerAngles.y) > 1f)
            {
                var calculatedRotation = Quaternion.Euler(new Vector3(0, _targetRotation, 0));
                calculatedRotation = Quaternion.Lerp(transform.localRotation, calculatedRotation, Time.deltaTime * 30);
                transform.localRotation = calculatedRotation;
            }
            if (_parentId != -1)
                return;
            _distanceToTarget = Vector2.Distance(new Vector2(transform.position.x, transform.position.z), new Vector2(_movementTargetPosition.x, _movementTargetPosition.z));
            _visualSpeed = Mathf.Clamp(_distanceToTarget, 0f, 1f);
            _smoothedTargetPosition = Vector3.Lerp(_smoothedTargetPosition, _movementTargetPosition, 1.3f);
            Vector3 calculatedPosition = transform.position;
            if (_distanceToTarget > 0.017f)
            {// Process DirecionVector
                calculatedPosition = Vector3.Lerp(calculatedPosition, _smoothedTargetPosition, Time.deltaTime * 7.5f);
                transform.position = calculatedPosition;
            }
        }
        private void OnDrawGizmos()
        {
            if (_parentId == -1)
            {
                Gizmos.color = Color.red;
                Gizmos.DrawLine(transform.position, MovementTargetPosition);
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

                Vector3 pos = b.GetPosition12B();
                if (!teleported)
                {
                    _movementTargetPosition = pos;
                }
                else
                {
                    _movementTargetPosition = pos;
                    transform.position = pos;
                }

                float rotation = b.GetAngle2B();
                _targetRotation = rotation;


                if (parentUpdate)
                {
                    _parentId = b.GetShort();
                    _parentPlaneId = b.GetByte();
                    if (_parentId != -1)
                    {
                        var unit = UnitManager.Instance[_parentId];
                        var display = UnitManager.Instance[_parentId]._display;
                        if (display != null && display.Model != -1)
                        {
                            JoinParent();
                        }
                        else
                        {
                            if (unit._display == null)
                                unit._display = unit.gameObject.AddComponent<UnitDisplay>();

                            unit._display.OnModelChange += JoinParent;
                        }
                    }
                    else
                    {
                        transform.parent = KemetMap.Instance.GetComponent<MapQuadTree>().GetTreeFor(new Vector2(MovementTargetPosition.x, MovementTargetPosition.z)).transform;
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
                //IsStatic = displayMask[2];
                bool hasEffects = displayMask[3];
                bool _hasCharacterCustoms = displayMask[4];
                bool _visible = displayMask[5];

                if (_display == null)
                    _display = gameObject.AddComponent<UnitDisplay>();

                if (!isItem)
                {
                    if (Display.Model != modelId)
                    {
                        Display.Model = modelId;
                    }
                }

                else
                {
                    if (item != null)
                        Destroy(item.gameObject);
                    else
                        transform.localPosition += Vector3.up;

                    try
                    {
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
                    catch (ArgumentOutOfRangeException e)
                    {
                        Debug.LogError("Unexisting item in contentmanager.i.items[" + modelId + "]");
                        Debug.LogError(e.Message);
                    }
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

                bool _hpUpdate = mask[0], _enUpdate = mask[1], _attributesUpdate = mask[2], _fractionUpdate = mask[3];

                if (_hpUpdate)
                    PlayerUnitAttributes.CurrentHealth = b.GetUnsignedShort();

                if (_enUpdate)
                    PlayerUnitAttributes.CurrentEnergy = b.GetUnsignedByte();

                if (_attributesUpdate)
                {
                    byte count = b.GetUnsignedByte();

                    for (int i = 0; i < count; i++)
                    {
                        var property = (UnitAttributeProperty)b.GetByte();
                        float value = b.GetShort() / 100f;

                        PlayerUnitAttributes.SetAttribute(property, value);
                    }
                }
                if (_fractionUpdate)
                {
                    Fraction = (Fraction)b.GetByte();
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
            ClientCommunicator.Instance.StartCoroutine(Ease.Join(
                transform,
                plane,
                () =>
                {
                    transform.parent = plane;
                },
                0.3f));


            if (parent.Display.OnModelChange != null)
                parent.Display.OnModelChange -= JoinParent;
        }

        public void OnUnprecieseMovement(UDPUnprecieseMovement p)
        {
            _movementTargetPosition += p.Difference;
            _targetRotation = p.Face;
        }

       
    }
}
