using System;
using Libaries.IO;
using Server.Model.Content.Spawns.NpcSpawns;
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
        private ProfileInterfaceUpdatePacket.PacketTab _tab;

        public ProfileInterface(ClientUI ui)
            : base(ui)
        {
        }

        public override InterfaceType GetInterfaceType
        {
            get { return InterfaceType.ProfileInterface; }
        }


        public ProfileInterfaceUpdatePacket.PacketTab Tab
        {
            get { return _tab; }
            private set
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
                if (player.ClientUi.Inventories == null)
                    throw new Exception("Null player.ClientUi.Inventories");
                if (Unit.GetExt<UnitInventory>() == null)
                    throw new Exception("Null Unit.GetExt<UnitInventory>()");
                player.ClientUi.Inventories.ShowInventory(Unit.GetExt<UnitInventory>());
            }
            else if (value == ProfileInterfaceUpdatePacket.PacketTab.Vendor)
            {
            }
            else if (value == ProfileInterfaceUpdatePacket.PacketTab.Levels)
            {
            }
            else if (value == ProfileInterfaceUpdatePacket.PacketTab.Equipment)
            {
            }
            else if (value == ProfileInterfaceUpdatePacket.PacketTab.Access)
            {
            }
            else if (value == ProfileInterfaceUpdatePacket.PacketTab.Dialogue)
            {
            }
            else if (value == ProfileInterfaceUpdatePacket.PacketTab.Main)
            {
            }
            else if (value == ProfileInterfaceUpdatePacket.PacketTab.Trade)
            {
            }
            else
            {
                UnityEngine.Debug.LogError("Unhandle profile tab opening: " + value);
            }
        }

        private void CloseCurrentTab()
        {
            if (_tab == ProfileInterfaceUpdatePacket.PacketTab.Inventory)
                player.ClientUi.Inventories.CloseInventory(Unit.GetExt<UnitInventory>());
            //else if (_tab == ProfileInterfaceUpdatePacket.PacketTab.Vendor)
                //player.ClientUi.Shop.CloseInventory(Unit.GetExt<UnitInventory>());


            else if (_tab == ProfileInterfaceUpdatePacket.PacketTab.Vendor)
            {
            }
            else if (_tab == ProfileInterfaceUpdatePacket.PacketTab.Levels)
            {
            }
            else if (_tab == ProfileInterfaceUpdatePacket.PacketTab.Equipment)
            {
            }
            else if (_tab == ProfileInterfaceUpdatePacket.PacketTab.Access)
            {
            }
            else if (_tab == ProfileInterfaceUpdatePacket.PacketTab.Dialogue)
            {
            }
            else if (_tab == ProfileInterfaceUpdatePacket.PacketTab.Main)
            {
            }
            else if (_tab == ProfileInterfaceUpdatePacket.PacketTab.Trade)
            {
            }
            else
            {
                UnityEngine.Debug.LogError("Unhandled profile tab closing: " + _tab);
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
            Unit = __otherUnit;
            Tab = tab;
            Opened = true;
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

            if (packet.Tab == ProfileInterfaceUpdatePacket.PacketTab.Levels)
            {
                packet.JsonObject = new JSONObject();
                packet.JsonObject.AddField("Levels", _unit.GetExt<PlayerLevels>().ToJsonObject());
            }

            if (_unit.Access != null) packet.Access = _unit.Access.GetAccessFor(player);

            player.Client.ConnectionHandler.SendPacket(packet);
        }
        
        public override void OnEvent(string action, int controlId)
        {
            base.OnEvent(action, controlId);

            if (controlId >= 0 && controlId <= 7)
                Open(_unit, (ProfileInterfaceUpdatePacket.PacketTab)controlId);

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
