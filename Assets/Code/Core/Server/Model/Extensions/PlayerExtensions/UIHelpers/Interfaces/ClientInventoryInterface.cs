using Libaries.Net.Packets.ForClient;
using Shared.Content.Types;
#if SERVER
using System;
using System.Collections.Generic;
using Code.Code.Libaries.Net.Packets;
using Code.Core.Shared.Content.Types;
using Server.Model.Entities.Human;
using Server.Model.Extensions.UnitExts;
using UnityEngine;

namespace Server.Model.Extensions.PlayerExtensions.UIHelpers.Interfaces
{
    public class ClientInventoryInterface
    {
        public ClientInventoryInterface(Player player)
        {
            Player = player;
        }

        public Player Player { get; private set; }

        public Item.ItemInstance this[int id, int x, int y]
        {
            set
            {
                bool isOpened = Player.ClientUi.ProfileInterface.Unit != null && Player.ClientUi.ProfileInterface.Unit.ID == id;

                if (!isOpened)
                {
                    ShowInventory(id);
                }

                var packet = new UIInventoryInterfacePacket();

                packet.type = UIInventoryInterfacePacket.PacketType.SetItem;
                packet.UnitID = id;
                packet.Value = value == null ? -1 : value.Item.InContentManagerIndex;
                packet.Amount = value == null ? 0 : value.Amount;
                packet.X = x;
                packet.Y = y;

                Player.Client.ConnectionHandler.SendPacket(packet);
                
            }
        }

        private void ShowInventory(int id)
        {
            if(id == Player.ID)
                return;

            UnitInventory unitInventory = Player.CurrentWorld[id].GetExt<UnitInventory>();
            if (unitInventory == null)
            {
                Debug.LogError("Not an inventory.");
                return;
            }
            ShowInventory(unitInventory);
        }

        public void ShowInventory(UnitInventory inventory)
        {
            if (!inventory.ListeningPlayers.Contains(Player))
            {
                inventory.ListeningPlayers.Add(Player);
            }

            var packet = new UIInventoryInterfacePacket();

            Player.ClientUi.ProfileInterface.Open(inventory.Unit, ProfileInterfaceUpdatePacket.PacketTab.Inventory);

            packet.type = UIInventoryInterfacePacket.PacketType.SHOW;
            packet.UnitID = inventory.Unit.ID;
            packet.X = inventory.Width;
            packet.Y = inventory.Height;

            Player.Client.ConnectionHandler.SendPacket(packet);

            int index = 0;
            foreach (var item in inventory.GetItems())
            {
                var packet2 = new UIInventoryInterfacePacket();

                packet2.type = UIInventoryInterfacePacket.PacketType.SetItem;
                packet2.UnitID = inventory.Unit.ID;
                packet2.Value = item == null ? -1 : item.Item.InContentManagerIndex;
                packet.Amount = item == null ? 0 : item.Amount;
                int y = index / inventory.Width;
                int x = index - y * inventory.Width;
                packet2.X = x;
                packet2.Y = y;

                Player.Client.ConnectionHandler.SendPacket(packet2);
                index++;
            }
            
            if(inventory.OnWasOpened != null)
                inventory.OnWasOpened(Player);
        }

        public void CloseInventory(int id)
        {
            UnitInventory unitInventory = Player.CurrentWorld[id].GetExt<UnitInventory>();
            if (unitInventory == null)
                Debug.LogError("Not an inventory.");
            CloseInventory(unitInventory);
        }

        public void CloseInventory(UnitInventory inventory)
        {
            if (inventory.ListeningPlayers.Contains(Player))
            {
                inventory.ListeningPlayers.Remove(Player);
            }

            var packet = new UIInventoryInterfacePacket();

            packet.type = UIInventoryInterfacePacket.PacketType.HIDE;
            packet.UnitID = inventory.Unit.ID;

            Player.Client.ConnectionHandler.SendPacket(packet);

            if (inventory.OnWasOpened != null)
                inventory.OnWasOpened(Player);
        }
    }
}

#endif
