using Server.Model.Extensions.UnitExts;
#if SERVER
using Code.Core.Shared.Content.Types.ItemExtensions;

using Code.Core.Client.UI;
using Libaries.Net.Packets.ForClient;
using Server.Model.Entities;
using Server.Model.Entities.Human;

namespace Server.Model.Extensions.PlayerExtensions.UIHelpers.Interfaces
{
    public class ProfileInterface : AInterface
    {
        private ServerUnit _unit;

        public ProfileInterface(ClientUI ui) : base(ui)
        {
        }

        public override InterfaceType GetInterfaceType
        {
            get { return InterfaceType.ProfileInterface;}
        }

        public ServerUnit Unit
        {
            get { return _unit; }
            set
            {
                if (_unit != value)
                {
                    _unit = value;
                    UnitWasChanged();
                }
                
            }
        }

        private void UnitWasChanged()
        {
            if (_unit == null)
            {
                Opened = false;
                return;
            }

            var packet = new ProfileInterfaceUpdatePacket();

            packet.HasEquipmentTab = _unit == player;
            packet.HasLevelsTab = _unit is Player;
            packet.HasMainTab = _unit is Player;
            packet.HasDialogueTab = _unit is Player;
            packet.HasVendorTradeTab = false;
            packet.HasInventoryTab = _unit.GetExt<UnitInventory>() != null && _unit.Access != null && _unit.Access.GetAccessFor(player).View_Inventory;
            packet.HasAccessTab = _unit.Access != null;
            packet.HasTradeTab = _unit is Player;
            packet.UnitID = Unit.ID;

            if (_unit.Access != null) packet.Access = _unit.Access.GetAccessFor(player);

            player.Client.ConnectionHandler.SendPacket(packet);
        }

        public override void OnOpen()
        {
            base.OnOpen();
            UnitWasChanged();
        }

        public override void OnEvent(string action, int controlId)
        {
            base.OnEvent(action, controlId);

            if(Unit== player)
            if (action == "Unequip")
            {
                if (controlId == 101)
                {
                    player.Equipment.UnequipItem(EquipmentItem.Type.Helm);
                }
                if (controlId == 102)
                {
                    player.Equipment.UnequipItem(EquipmentItem.Type.Body);
                }
                if (controlId == 103)
                {
                    player.Equipment.UnequipItem(EquipmentItem.Type.Legs);
                }
                if (controlId == 104)
                {
                    player.Equipment.UnequipItem(EquipmentItem.Type.Boots);
                }
                if (controlId == 105)
                {
                    player.Equipment.UnequipItem(EquipmentItem.Type.MainHand);
                }
                if (controlId == 106)
                {
                    player.Equipment.UnequipItem(EquipmentItem.Type.OffHand);
                }
            }
        }
    }
}
#endif
