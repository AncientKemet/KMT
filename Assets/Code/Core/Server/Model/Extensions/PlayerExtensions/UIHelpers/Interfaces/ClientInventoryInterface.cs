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

        private Dictionary<int, bool> InventoriesOpened = new Dictionary<int, bool>();

        public Item this[int id, int x, int y]
        {
            set
            {
                bool isOpened = false;

                InventoriesOpened.TryGetValue(id, out isOpened);

                if (!isOpened)
                {
                    ShowInventory(id);
                }

                var packet = new UIInventoryInterfacePacket();

                packet.type = UIInventoryInterfacePacket.PacketType.SetItem;
                packet.UnitID = id;
                packet.Value = value == null ? -1 : value.InContentManagerIndex;
                packet.X = x;
                packet.Y = y;

                try
                {
                    Player.Client.ConnectionHandler.SendPacket(packet);
                }
                catch
                { }
            }
        }

        private void ShowInventory(int id)
        {
            if(id == Player.ID)
                return;

            UnitInventory unitInventory = Player.CurrentWorld[id].GetExt<UnitInventory>();
            if (unitInventory == null)
                Debug.LogError("Not an inventory.");
            ShowInventory(unitInventory);
        }

        public void ShowInventory(UnitInventory inventory)
        {
            if (!inventory.ListeningPlayers.Contains(Player))
            {
                inventory.ListeningPlayers.Add(Player);
            }

            var packet = new UIInventoryInterfacePacket();

            packet.type = UIInventoryInterfacePacket.PacketType.SHOW;
            packet.UnitID = inventory.Unit.ID;
            packet.X = inventory.Width;
            packet.Y = inventory.Height;

            Player.Client.ConnectionHandler.SendPacket(packet);

            if (InventoriesOpened.ContainsKey(inventory.Unit.ID))
                InventoriesOpened[inventory.Unit.ID] = true;
            else
                InventoriesOpened.Add(inventory.Unit.ID, true);

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

            if (InventoriesOpened.ContainsKey(inventory.Unit.ID))
                InventoriesOpened[inventory.Unit.ID] = false;
            else
                InventoriesOpened.Add(inventory.Unit.ID, false);
        }
    }
}

#endif
