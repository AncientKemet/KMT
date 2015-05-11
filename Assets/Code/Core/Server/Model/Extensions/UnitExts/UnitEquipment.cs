using Shared.Content.Types;
#if SERVER
using Server.Model.Content;

using System.Diagnostics;
using Code.Code.Libaries.Net;
using Code.Core.Shared.Content.Types;
using Code.Core.Shared.Content.Types.ItemExtensions;
using Code.Libaries.UnityExtensions;
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
                    _EquipItem(null, ref _head);
                    break;
                case EquipmentItem.Type.Boots:
                    _EquipItem(null, ref _boots);
                    break;
                case EquipmentItem.Type.Body:
                    _EquipItem(null, ref _body);
                    break;
                case EquipmentItem.Type.Legs:
                    _EquipItem(null, ref _legs);
                    break;
                case EquipmentItem.Type.MainHand:
                    _EquipItem(null, ref _mainHand);
                    break;
                case EquipmentItem.Type.OffHand:
                    _EquipItem(null, ref _offHand);
                    break;
                case EquipmentItem.Type.TwoHand:
                    _EquipItem(null, ref _mainHand);
                    break;
            }
        }

        public void DestroyItem(EquipmentItem.Type type)
        {
            switch (type)
            {
                case EquipmentItem.Type.Helm:
                    _EquipItem(null, ref _head, false);
                    break;
                case EquipmentItem.Type.Boots:
                    _EquipItem(null, ref _boots, false);
                    break;
                case EquipmentItem.Type.Body:
                    _EquipItem(null, ref _body, false);
                    break;
                case EquipmentItem.Type.Legs:
                    _EquipItem(null, ref _legs, false);
                    break;
                case EquipmentItem.Type.MainHand:
                    _EquipItem(null, ref _mainHand, false);
                    break;
                case EquipmentItem.Type.OffHand:
                    _EquipItem(null, ref _offHand, false);
                    break;
                case EquipmentItem.Type.TwoHand:
                    _EquipItem(null, ref _mainHand, false);
                    break;
            }
        }

        public bool EquipItem(DroppedItem unit)
        {
            EquipmentItem item = unit == null ? null : unit.Item.Item.EQ;
            switch (item.EquipType)
            {
                case EquipmentItem.Type.Helm:
                    return _EquipItem(unit, ref _head);
                case EquipmentItem.Type.Boots:
                    return _EquipItem(unit, ref _boots);
                case EquipmentItem.Type.Body:
                    return _EquipItem(unit, ref _body);
                case EquipmentItem.Type.Legs:
                    return _EquipItem(unit, ref _legs);
                case EquipmentItem.Type.MainHand:
                    return _EquipItem(unit, ref _mainHand);
                case EquipmentItem.Type.OffHand:
                    return _EquipItem(unit, ref _offHand);
                case EquipmentItem.Type.TwoHand:
                    return _EquipItem(unit, ref _mainHand);
            }
            return false;
        }

        #region Private Handling
        /// <summary>
        /// Sets _wasUpdate to True;
        /// </summary>
        /// <param name="item"></param>
        /// <param name="_itemRef"></param>
        private bool _EquipItem(DroppedItem unit, ref DroppedItem _itemRef, bool unequip = true)
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
                        unit.Movement.Parent = Unit.Movement;
                        if (item.EquipType == EquipmentItem.Type.MainHand)
                        {
                            unit.Movement.ParentPlaneID = 1;
                        }
                        if (item.EquipType == EquipmentItem.Type.OffHand)
                        {
                            unit.Movement.ParentPlaneID = 2;
                        }
                    }

                    if (item != null && item.Spells != null)
                        Unit.Actions.EquipSpells(item.Spells);
                    
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
                    Unit.Actions.EquipSpells(item.Spells);

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
                                Unit.Actions.UnEquipSpells(_itemRef.Item.Item.EQ.Spells);
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
                            Unit.Actions.UnEquipSpells(_itemRef.Item.Item.EQ.Spells);

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
                    Unit.Actions.UnEquipSpells(_itemRef.Item.Item.EQ.Spells);
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
            p.AddShort(_head == null ? -1 : _head.ID);
            p.AddShort(_body == null ? -1 : _body.Item.Item.InContentManagerIndex);
            p.AddShort(_legs == null ? -1 : _legs.Item.Item.InContentManagerIndex);
            p.AddShort(_boots == null ? -1 : _boots.Item.Item.InContentManagerIndex);
            p.AddShort(_mainHand == null ? -1 : _mainHand.ID);
            p.AddShort(_offHand == null ? -1 : _offHand.ID);
        }

        protected override void pSerializeUpdate(ByteStream p)
        {
            p.AddShort(_head == null ? -1 : _head.ID);
            p.AddShort(_body == null ? -1 : _body.Item.Item.InContentManagerIndex);
            p.AddShort(_legs == null ? -1 : _legs.Item.Item.InContentManagerIndex);
            p.AddShort(_boots == null ? -1 : _boots.Item.Item.InContentManagerIndex);
            p.AddShort(_mainHand == null ? -1 : _mainHand.ID);
            p.AddShort(_offHand == null ? -1 : _offHand.ID);
        }

        protected override void OnExtensionWasAdded()
        {
            base.OnExtensionWasAdded();
            Unit = entity as ServerUnit;
        }

    }
}
#endif
