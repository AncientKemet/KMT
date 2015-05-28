using Code.Libaries.Generic.Managers;
using Libaries.IO;
using Shared.Content.Types;
#if SERVER
using Server.Model.Content;
using Code.Code.Libaries.Net;
using Code.Core.Shared.Content.Types.ItemExtensions;
using Server.Model.Entities;
using Server.Model.Entities.Items;
using Debug = UnityEngine.Debug;

namespace Server.Model.Extensions.UnitExts
{
    public class UnitEquipment : UnitUpdateExt
    {

        public ServerUnit Unit { get; private set; }

        private DroppedItem _head;
        private DroppedItem _body;
        private DroppedItem _legs;
        private DroppedItem _boots;
        private DroppedItem _mainHand;
        private DroppedItem _offHand;

        public EquipmentItem Head
        {
            get
            {
                if (_head == null)
                    return null;
                return _head.Item.Item.EQ;
            }
        }

        public EquipmentItem Body
        {
            get
            {
                if (_body == null)
                    return null;
                return _body.Item.Item.EQ; ;
            }
        }

        public EquipmentItem Legs
        {
            get
            {
                if (_legs == null)
                    return null;
                return _legs.Item.Item.EQ; ;
            }
        }

        public EquipmentItem Boots
        {
            get
            {
                if (_boots == null)
                    return null;
                return _boots.Item.Item.EQ; ;
            }
        }

        public EquipmentItem MainHand
        {
            get
            {
                if (_mainHand == null)
                    return null;
                return _mainHand.Item.Item.EQ; ;
            }
        }

        public EquipmentItem OffHand
        {
            get
            {
                if (_offHand == null)
                    return null;
                return _offHand.Item.Item.EQ; ;
            }
        }

        public DroppedItem OffHandUnit
        {
            get
            {
                return _offHand;
            }
        }

        public DroppedItem MainHandUnit
        {
            get
            {
                return _mainHand;
            }
        }

        public DroppedItem HeadUnit
        {
            get
            {
                return _head;
            }
        }

        public void UnequipItem(EquipmentItem.Type type)
        {
            switch (type)
            {
                case EquipmentItem.Type.Helm:
                    _EquipItemSafe(null, ref _head);
                    break;
                case EquipmentItem.Type.Boots:
                    _EquipItemSafe(null, ref _boots);
                    break;
                case EquipmentItem.Type.Body:
                    _EquipItemSafe(null, ref _body);
                    break;
                case EquipmentItem.Type.Legs:
                    _EquipItemSafe(null, ref _legs);
                    break;
                case EquipmentItem.Type.MainHand:
                    _EquipItemSafe(null, ref _mainHand);
                    break;
                case EquipmentItem.Type.OffHand:
                    _EquipItemSafe(null, ref _offHand);
                    break;
                case EquipmentItem.Type.TwoHand:
                    _EquipItemSafe(null, ref _mainHand);
                    break;
            }
        }

        public void DestroyItem(EquipmentItem.Type type)
        {
            switch (type)
            {
                case EquipmentItem.Type.Helm:
                    _EquipItemSafe(null, ref _head, false);
                    break;
                case EquipmentItem.Type.Boots:
                    _EquipItemSafe(null, ref _boots, false);
                    break;
                case EquipmentItem.Type.Body:
                    _EquipItemSafe(null, ref _body, false);
                    break;
                case EquipmentItem.Type.Legs:
                    _EquipItemSafe(null, ref _legs, false);
                    break;
                case EquipmentItem.Type.MainHand:
                    _EquipItemSafe(null, ref _mainHand, false);
                    break;
                case EquipmentItem.Type.OffHand:
                    _EquipItemSafe(null, ref _offHand, false);
                    break;
                case EquipmentItem.Type.TwoHand:
                    _EquipItemSafe(null, ref _mainHand, false);
                    break;
            }
        }

        public bool EquipItem(DroppedItem unit)
        {
            EquipmentItem item = unit == null ? null : unit.Item.Item.EQ;
            switch (item.EquipType)
            {
                case EquipmentItem.Type.Helm:
                    return _EquipItemSafe(unit, ref _head);
                case EquipmentItem.Type.Boots:
                    return _EquipItemSafe(unit, ref _boots);
                case EquipmentItem.Type.Body:
                    return _EquipItemSafe(unit, ref _body);
                case EquipmentItem.Type.Legs:
                    return _EquipItemSafe(unit, ref _legs);
                case EquipmentItem.Type.MainHand:
                    return _EquipItemSafe(unit, ref _mainHand);
                case EquipmentItem.Type.OffHand:
                    return _EquipItemSafe(unit, ref _offHand);
                case EquipmentItem.Type.TwoHand:
                    return _EquipItemSafe(unit, ref _mainHand);
            }
            return false;
        }

        public bool EquipItem(Item.ItemInstance _item)
        {
            if (_item != null)
            {
                DroppedItem droppedItem = ServerMonoBehaviour.CreateInstance<DroppedItem>();

                droppedItem.Movement.Teleport(Unit.Movement.Position + Unit.Movement.Forward);
                droppedItem.Item = _item;
                Unit.CurrentWorld.AddEntity(droppedItem);

                return EquipItem(droppedItem);
            }
            return false;
        }

        #region Private Handling
        /// <summary>
        /// Sets _wasUpdate to True;
        /// </summary>
        /// <param name="item"></param>
        /// <param name="_itemRef"></param>
        private bool _EquipItemSafe(DroppedItem unit, ref DroppedItem _itemRef, bool unequip = true)
        {
            EquipmentItem item = null;

            if (unit != null)
                item = unit.Item.Item.EQ;

            if (_itemRef != null)
            {
                if (_UnequipItem(ref _itemRef, unequip))
                {
                    _itemRef = unit;

                    if (unit != null)
                    {
                        if (item.EquipType == EquipmentItem.Type.MainHand)
                        {
                            unit.Movement.Parent = Unit.Movement;
                            unit.Movement.ParentPlaneID = 1;
                        }
                        else if (item.EquipType == EquipmentItem.Type.OffHand)
                        {
                            unit.Movement.Parent = Unit.Movement;
                            unit.Movement.ParentPlaneID = 2;
                        }
                        else if (item.EquipType == EquipmentItem.Type.Helm)
                        {
                            unit.Movement.Parent = Unit.Movement;
                            unit.Movement.ParentPlaneID = 0;
                        }
                        else
                        {
                            unit.Movement.Parent = Unit.Movement;
                            unit.Display.Visible = false;
                        }
                    }

                    if (item != null && item.Spells != null)
                        if (item.EquipType == EquipmentItem.Type.OffHand || item.EquipType == EquipmentItem.Type.MainHand)
                            Unit.Spells.EquipSpells(item.Spells);
                    
                    _wasUpdate = true;
                    return true;
                }
                else
                {
                    //todo couldnt unequip item
                   Debug.Log("Couldnt unequip item.");
                    return false;
                }
            }
            else
            {
                _itemRef = unit;

                if (unit != null)
                {
                    unit.Movement.Parent = Unit.Movement;
                    if (item.EquipType == EquipmentItem.Type.MainHand)
                    {
                        unit.Movement.ParentPlaneID = 1;
                    } if (item.EquipType == EquipmentItem.Type.OffHand)
                    {
                        unit.Movement.ParentPlaneID = 2;
                    }
                }

                if(item != null && item.Spells != null)
                    Unit.Spells.EquipSpells(item.Spells);

                _wasUpdate = true;
                return true;
            }
            return false;
        }

        private bool _UnequipItem(ref DroppedItem _itemRef, bool unequip)
        {
            if (_itemRef != null)
            {
                if (unequip)
                {
                    if (_itemRef.Item.Item.EQ.CanBeStoredInInventory)
                    {
                        UnitInventory inventory = Unit.GetExt<UnitInventory>();
                        if (inventory != null)
                        {
                            if (inventory.HasSpace(1))
                            {
                                inventory.AddItem(_itemRef.Item);
                                Unit.Spells.UnEquipSpells(_itemRef.Item.Item.EQ.Spells);
                                _itemRef.Display.Destroy = true;
                                _itemRef = null;
                                _wasUpdate = true;
                                return true;
                            }
                        }
                    }
                    else
                    {
                        if (_itemRef.Item.Item.EQ.Spells != null)
                            Unit.Spells.UnEquipSpells(_itemRef.Item.Item.EQ.Spells);

                        _itemRef.Movement.Parent = null;
                        _itemRef.Movement.ParentPlaneID = -1;
                       
                        _itemRef = null;
                        _wasUpdate = true;
                        return true;
                    }
                    return false;
                }
                else
                {
                    Unit.Spells.UnEquipSpells(_itemRef.Item.Item.EQ.Spells);
                    _itemRef = null;
                    _wasUpdate = true;
                    return true;
                }
            }
                return true;
        }
        #endregion

        public override byte UpdateFlag()
        {
            return 0x10;
        }

        protected override void pSerializeState(ByteStream p)
        {
            p.AddShort(_head == null ? -1 : _head.Item.Item.InContentManagerIndex);
            p.AddShort(_body == null ? -1 : _body.Item.Item.InContentManagerIndex);
            p.AddShort(_legs == null ? -1 : _legs.Item.Item.InContentManagerIndex);
            p.AddShort(_boots == null ? -1 : _boots.Item.Item.InContentManagerIndex);
            p.AddShort(_mainHand == null ? -1 : _mainHand.Item.Item.InContentManagerIndex);
            p.AddShort(_offHand == null ? -1 : _offHand.Item.Item.InContentManagerIndex);
        }

        protected override void pSerializeUpdate(ByteStream p)
        {
            p.AddShort(_head == null ? -1 : _head.Item.Item.InContentManagerIndex);
            p.AddShort(_body == null ? -1 : _body.Item.Item.InContentManagerIndex);
            p.AddShort(_legs == null ? -1 : _legs.Item.Item.InContentManagerIndex);
            p.AddShort(_boots == null ? -1 : _boots.Item.Item.InContentManagerIndex);
            p.AddShort(_mainHand == null ? -1 : _mainHand.Item.Item.InContentManagerIndex);
            p.AddShort(_offHand == null ? -1 : _offHand.Item.Item.InContentManagerIndex);
        }

        protected override void OnExtensionWasAdded()
        {
            base.OnExtensionWasAdded();
            Unit = entity as ServerUnit;
        }

        public override void Serialize(JSONObject j)
        {
            JSONObject equipment = new JSONObject();

            equipment.AddField("MainHand", SerializeInstance(MainHandUnit == null ? new Item.ItemInstance(null, 0) : MainHandUnit.Item));
            equipment.AddField("OffHand", SerializeInstance(OffHandUnit == null ? new Item.ItemInstance(null, 0) : OffHandUnit.Item));
            equipment.AddField("Head", SerializeInstance(HeadUnit == null ? new Item.ItemInstance(null, 0) : HeadUnit.Item));
            equipment.AddField("Boots", SerializeInstance(_boots == null ? new Item.ItemInstance(null, 0) : _boots.Item));
            equipment.AddField("Body", SerializeInstance(_body == null ? new Item.ItemInstance(null, 0) : _body.Item));
            equipment.AddField("Legs", SerializeInstance(_legs == null ? new Item.ItemInstance(null, 0) : _legs.Item));
            
            j.AddField("equipment", equipment);
        }

        public override void Deserialize(JSONObject j)
        {
            JSONObject equipment = j.GetField("equipment");

            var mainHand = DeserializeInstance(equipment.GetField("MainHand").str);
            var offHand = DeserializeInstance(equipment.GetField("OffHand").str);
            var head = DeserializeInstance(equipment.GetField("Head").str);
            var boots = DeserializeInstance(equipment.GetField("Boots").str);
            var body = DeserializeInstance(equipment.GetField("Body").str);
            var legs = DeserializeInstance(equipment.GetField("Legs").str);

            Unit.OnFinishDeserialization += () =>
            {
                EquipItem(mainHand);
                EquipItem(offHand);
                EquipItem(head);
                EquipItem(boots);
                EquipItem(body);
                EquipItem(legs);
            };

        }

        private static string SerializeInstance(Item.ItemInstance instance)
        {
            return (instance.Item == null ? "-1" : "" + instance.Item.InContentManagerIndex) + "," + instance.Amount;
        }

        private static Item.ItemInstance DeserializeInstance(string s)
        {
            string[] splitStrings = s.Split(',');
            int id = int.Parse(splitStrings[0]);
            if (id != -1)
            {
                int am = int.Parse(splitStrings[1]);
                return new Item.ItemInstance(ContentManager.I.Items[id], am);
            }
            else
            {
                return null;
            }
        }
    }
}
#endif
