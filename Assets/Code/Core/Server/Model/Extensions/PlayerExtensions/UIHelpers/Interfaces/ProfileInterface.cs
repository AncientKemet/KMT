using System;
using System.Collections.ObjectModel;
using Server.Model.Content.Spawns.NpcSpawns;
using Server.Model.Extensions.UnitExts;
using Shared.Content.Types;
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
        private ProfileInterfaceUpdatePacket.PacketTab _tab;

        public ProfileInterface(ClientUI ui)
            : base(ui)
        {
        }

        public override InterfaceType GetInterfaceType
        {
            get { return InterfaceType.ProfileInterface; }
        }


        private ProfileInterfaceUpdatePacket.PacketTab Tab
        {
            get { return _tab; }
            set
            {
                if (_tab != value)
                {
                    CloseCurrentTab();
                    OpenNewTab(value);
                }
                _tab = value;
            }
        }

        private void OpenNewTab(ProfileInterfaceUpdatePacket.PacketTab value)
        {
            if (value == ProfileInterfaceUpdatePacket.PacketTab.Inventory)
            {
                player.ClientUi.Inventories.ShowInventory(Unit.GetExt<UnitInventory>());
            }
            else if (value == ProfileInterfaceUpdatePacket.PacketTab.Vendor)
            {
                player.ClientUi.Inventories.ShowInventory(Unit.GetExt<UnitInventory>());
            }
            else
            {
                throw  new Exception("Unhandle profile tab opening: "+value);
            }
        }

        private void CloseCurrentTab()
        {
            if (_tab == ProfileInterfaceUpdatePacket.PacketTab.Inventory)
                player.ClientUi.Inventories.CloseInventory(Unit.GetExt<UnitInventory>());
            else
            {
                throw new Exception("Unhandled profile tab closing: "+_tab);
            }
        }

        public ServerUnit Unit
        {
            get { return _unit; }
            private set
            {
                if (_unit != value)
                {
                    _unit = value;
                }
            }
        }

        public void Open(ServerUnit __otherUnit, ProfileInterfaceUpdatePacket.PacketTab tab)
        {
            _tab = tab;
            Opened = true;
            Unit = __otherUnit;
            SendPacket();
        }

        private void SendPacket()
        {
            if (_unit == null)
            {
                Opened = false;
                return;
            }

            if (!Opened)
                Opened = true;
            
            var packet = new ProfileInterfaceUpdatePacket();

            packet.HasEquipmentTab = _unit == player;
            packet.HasLevelsTab = _unit is Player;
            packet.HasMainTab = _unit is Player;
            packet.HasDialogueTab = false;
            packet.HasVendorTradeTab = _unit.GetComponent<NpcShop>() != null;
            packet.HasInventoryTab = _unit.GetExt<UnitInventory>() != null && _unit.Access != null && _unit.Access.GetAccessFor(player).View_Inventory;
            packet.HasAccessTab = _unit.Access != null;
            packet.HasTradeTab = _unit is Player;
            packet.Tab = _tab;
            packet.UnitID = Unit.ID;

            if (_unit.Access != null) packet.Access = _unit.Access.GetAccessFor(player);

            player.Client.ConnectionHandler.SendPacket(packet);
        }
        
        public override void OnOpen()
        {
            base.OnOpen();
        }

        public override void OnClose()
        {
            base.OnClose();
        }

        public override void OnEvent(string action, int controlId)
        {
            base.OnEvent(action, controlId);

            if (controlId >= 0 && controlId <= 7)
                Tab = (ProfileInterfaceUpdatePacket.PacketTab) controlId;

            if (Unit == player)
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
