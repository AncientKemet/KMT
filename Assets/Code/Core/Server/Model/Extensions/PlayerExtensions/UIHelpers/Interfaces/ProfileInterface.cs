using System;
using Libaries.IO;
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
        public ProfileInterface(ClientUI ui)
            : base(ui)
        {
        }

        public override void OnClose()
        {
            base.OnClose();
            CloseCurrentTab();
            ViewingUnit = null;
            Tab = ProfileInterfaceUpdatePacket.PacketTab.Main;
        }

        public override InterfaceType GetInterfaceType
        {
            get { return InterfaceType.ProfileInterface; }
        }


        public ProfileInterfaceUpdatePacket.PacketTab Tab { get; private set; }

        private void OpenNewTab(ProfileInterfaceUpdatePacket.PacketTab value)
        {
            if (value == ProfileInterfaceUpdatePacket.PacketTab.Inventory)
            {
                if (player.ClientUi.Inventories == null)
                    throw new Exception("Null player.ClientUi.Inventories");
                if (ViewingUnit.GetExt<UnitInventory>() == null)
                    throw new Exception("Null Unit.GetExt<UnitInventory>()");
                player.ClientUi.Inventories.ShowInventory(ViewingUnit.GetExt<UnitInventory>());
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
            Tab = value;
        }

        private void CloseCurrentTab()
        {
            if (Tab == ProfileInterfaceUpdatePacket.PacketTab.Inventory)
                if(ViewingUnit != player)
                player.ClientUi.Inventories.CloseInventory(ViewingUnit.GetExt<UnitInventory>());


            else if (Tab == ProfileInterfaceUpdatePacket.PacketTab.Vendor)
            {
            }
            else if (Tab == ProfileInterfaceUpdatePacket.PacketTab.Levels)
            {
            }
            else if (Tab == ProfileInterfaceUpdatePacket.PacketTab.Equipment)
            {
            }
            else if (Tab == ProfileInterfaceUpdatePacket.PacketTab.Access)
            {
            }
            else if (Tab == ProfileInterfaceUpdatePacket.PacketTab.Dialogue)
            {
            }
            else if (Tab == ProfileInterfaceUpdatePacket.PacketTab.Main)
            {
            }
            else if (Tab == ProfileInterfaceUpdatePacket.PacketTab.Trade)
            {
            }
            else
            {
                UnityEngine.Debug.LogError("Unhandled profile tab closing: " + Tab);
            }
        }

        public ServerUnit ViewingUnit { get; private set; }

        public void Open(ServerUnit __otherUnit, ProfileInterfaceUpdatePacket.PacketTab newTab)
        {
            if (Opened && Tab != newTab)
                CloseCurrentTab();

            ViewingUnit = __otherUnit;
            OpenNewTab(newTab);

            
            if (!Opened)
                Opened = true;
            
            var packet = new ProfileInterfaceUpdatePacket();

            packet.HasEquipmentTab = ViewingUnit == player;
            packet.HasLevelsTab = ViewingUnit is Player;
            packet.HasMainTab = ViewingUnit is Player;
            packet.HasDialogueTab = false;
            packet.HasVendorTradeTab = ViewingUnit.GetComponent<NpcShop>() != null;
            packet.HasInventoryTab = ViewingUnit.GetExt<UnitInventory>() != null && ViewingUnit.Access != null && ViewingUnit.Access.GetAccessFor(player).View_Inventory;
            packet.HasAccessTab = ViewingUnit.Access != null;
            packet.HasTradeTab = ViewingUnit is Player;
            packet.Tab = Tab;
            packet.UnitID = ViewingUnit.ID;

            if (packet.Tab == ProfileInterfaceUpdatePacket.PacketTab.Levels)
            {
                packet.JsonObject = new JSONObject();
                packet.JsonObject.AddField("Levels", ViewingUnit.GetExt<PlayerLevels>().ToJsonObject());
            }

            if (ViewingUnit.Access != null) packet.Access = ViewingUnit.Access.GetAccessFor(player);

            player.Client.ConnectionHandler.SendPacket(packet);
        }
        
        public override void OnEvent(string action, int controlId)
        {
            base.OnEvent(action, controlId);

            if (controlId >= 0 && controlId <= 7)
                Open(ViewingUnit, (ProfileInterfaceUpdatePacket.PacketTab)controlId);

            if (ViewingUnit == player)
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

            if (Tab == ProfileInterfaceUpdatePacket.PacketTab.Inventory)
            {
                if (action.StartsWith("Take"))
                {
                    var inventory = ViewingUnit.GetExt<UnitInventory>();
                    var index = controlId - 501;
                    var amount = int.Parse(action.Split(' ')[1]);
                    if (inventory != null)
                    {
                        if (inventory[index] != null)
                        {
                            var ii = new Item.ItemInstance(inventory[index].Item, amount);

                            if (inventory.RemoveItem(ii))
                            {
                                ui.Player.Inventory.AddItem(ii);
                            }
                        }
                    }
                }
            }

            if (Tab == ProfileInterfaceUpdatePacket.PacketTab.Vendor)
            {
                if (action.StartsWith("Buy"))
                {
                    var shop = ViewingUnit.GetComponents<NpcShop>();
                    var index = controlId - 1001;
                    var amount = int.Parse(action.Split(' ')[1]);
                    if (shop != null)
                    {
                    }
                }
            }

            if (action == "Close interface")
            {
                if (Opened)
                    Opened = false;
            }

        }

    }
}
#endif
