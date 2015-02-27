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

        private EquipmentItem _head;
        private EquipmentItem _body;
        private EquipmentItem _legs;
        private EquipmentItem _boots;
        private EquipmentItem _mainHand;
        private EquipmentItem _offHand;

        public EquipmentItem Head
        {
            get { return _head; }
        }

        public EquipmentItem Body
        {
            get { return _body; }
        }

        public EquipmentItem Legs
        {
            get { return _legs; }
        }

        public EquipmentItem Boots
        {
            get { return _boots; }
        }

        public EquipmentItem MainHand
        {
            get { return _mainHand; }
        }

        public EquipmentItem OffHand
        {
            get { return _offHand; }
        }

        public void EquipItem(Item item)
        {
            EquipmentItem equipmentItem = item.GetComponent<EquipmentItem>();
            if (equipmentItem != null)
            {
                EquipItem(equipmentItem);
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

        public void EquipItem(EquipmentItem item)
        {
            switch (item.EquipType)
            {
                case EquipmentItem.Type.Helm:
                    _EquipItem(item, ref _head);
                    break;
                case EquipmentItem.Type.Boots:
                    _EquipItem(item, ref _boots);
                    break;
                case EquipmentItem.Type.Body:
                    _EquipItem(item, ref _body);
                    break;
                case EquipmentItem.Type.Legs:
                    _EquipItem(item, ref _legs);
                    break;
                case EquipmentItem.Type.MainHand:
                    _EquipItem(item, ref _mainHand);
                    break;
                case EquipmentItem.Type.OffHand:
                    _EquipItem(item, ref _offHand);
                    break;
                case EquipmentItem.Type.TwoHand:
                    _EquipItem(item, ref _mainHand);
                    break;
            }
        }

        #region Private Handling
        /// <summary>
        /// Sets _wasUpdate to True;
        /// </summary>
        /// <param name="item"></param>
        /// <param name="_itemRef"></param>
        private void _EquipItem(EquipmentItem item, ref EquipmentItem _itemRef, bool unequip = true)
        {
            if (_itemRef != null)
            {

                if (_UnequipItem(ref _itemRef, unequip))
                {
                    _itemRef = item;
                    if (item != null)
                    {
                        Unit.Actions.EquipSpells(_itemRef.Spells);
                    }
                    _wasUpdate = true;
                }
                else
                {
                    //todo couldnt unequip item
                   Debug.Log("Couldnt unequip item.");
                }
            }
            else
            {
                _itemRef = item;
                
                Unit.Actions.EquipSpells(_itemRef.Spells);

                _wasUpdate = true;
            }
        }

        private bool _UnequipItem(ref EquipmentItem _itemRef, bool unequip)
        {
            if (_itemRef != null)
            {
                if (unequip)
                {
                    if (_itemRef.CanBeStoredInInventory)
                    {
                        UnitInventory inventory = Unit.GetExt<UnitInventory>();
                        if (inventory != null)
                        {
                            if (inventory.HasSpace(1))
                            {
                                inventory.AddItem(_itemRef.Item);
                                Unit.Actions.UnEquipSpells(_itemRef.Spells);
                                _itemRef = null;
                                _wasUpdate = true;
                                return true;
                            }
                        }
                    }
                    else
                    {
                        Unit.Actions.UnEquipSpells(_itemRef.Spells);
                        _itemRef = null;
                        DroppedItem droppedItem = ServerMonoBehaviour.CreateInstance<DroppedItem>();
                        droppedItem.Item = _itemRef.Item;
                        droppedItem.Movement.Teleport(Unit.Movement.Position);
                        _wasUpdate = true;
                        return true;
                    }
                    return false;
                }
                else
                {
                    Unit.Actions.UnEquipSpells(_itemRef.Spells);
                    _itemRef = null;
                    _wasUpdate = true;
                    return true;
                }
            }
            else
            {
                return true;
            }
        }
        #endregion

        public override byte UpdateFlag()
        {
            return 0x10;
        }

        protected override void pSerializeState(ByteStream p)
        {
            p.AddShort(_head == null ? -1 : _head.Item.InContentManagerIndex);
            p.AddShort(_body == null ? -1 : _body.Item.InContentManagerIndex);
            p.AddShort(_legs == null ? -1 : _legs.Item.InContentManagerIndex);
            p.AddShort(_boots == null ? -1 : _boots.Item.InContentManagerIndex);
            p.AddShort(_mainHand == null ? -1 : _mainHand.Item.InContentManagerIndex);
            p.AddShort(_offHand == null ? -1 : _offHand.Item.InContentManagerIndex);
        }

        protected override void pSerializeUpdate(ByteStream p)
        {
            p.AddShort(_head == null ? -1 : _head.Item.InContentManagerIndex);
            p.AddShort(_body == null ? -1 : _body.Item.InContentManagerIndex);
            p.AddShort(_legs == null ? -1 : _legs.Item.InContentManagerIndex);
            p.AddShort(_boots == null ? -1 : _boots.Item.InContentManagerIndex);
            p.AddShort(_mainHand == null ? -1 : _mainHand.Item.InContentManagerIndex);
            p.AddShort(_offHand == null ? -1 : _offHand.Item.InContentManagerIndex);
        }

        protected override void OnExtensionWasAdded()
        {
            base.OnExtensionWasAdded();
            Unit = entity as ServerUnit;
        }

        public override void Serialize(ByteStream bytestream)
        {
            bytestream.AddEqItem(Head);
            bytestream.AddEqItem(Body);
            bytestream.AddEqItem(Legs);
            bytestream.AddEqItem(Boots);
            bytestream.AddEqItem(MainHand);
            bytestream.AddEqItem(OffHand);
        }

        public override void Deserialize(ByteStream bytestream)
        {
            bytestream.GetEqItem(ref _head);
            bytestream.GetEqItem(ref _body);
            bytestream.GetEqItem(ref _legs);
            bytestream.GetEqItem(ref _boots);
            bytestream.GetEqItem(ref _mainHand);
            bytestream.GetEqItem(ref _offHand);
        }
    }
}
#endif
