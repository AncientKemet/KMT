#if SERVER
using Server.Model.Content;

using System;
using System.Configuration;
using Code.Code.Libaries.Net;
using Code.Libaries.Generic.Managers;
using Code.Libaries.Net.Packets.ForServer;
using Code.Libaries.UnityExtensions;
using Server.Model.Entities.Items;
using UnityEngine;
using System.Collections.Generic;
using Code.Core.Shared.Content.Types;
using Server.Model.Entities;
using Server.Model.Entities.Human;

namespace Server.Model.Extensions.UnitExts
{
    public class UnitInventory : EntityExtension
    {
        public enum AccessType
        {
            ALL,
            ONLY_THIS_UNIT
        }

        private int _width = 1;

        private int _height = 1;

        private List<Item> _items;

        public ServerUnit Unit { get; private set; }

        public AccessType AccesType { get; set; }

        public List<Player> ListeningPlayers = new List<Player>();

        public int Width
        {
            get { return _width; }
            set
            {
                _width = value;
                RecreateInventory();
            }
        }

        public int Height
        {
            get { return _height; }
            set
            {
                _height = value;
                RecreateInventory();
            }
        }

        private void RecreateInventory()
        {
            _items = new List<Item>(Width * Height);
            for (int i = 0; i < Width * Height; i++)
            {
                _items.Add(null);
            }
        }

        public Item this[int x, int y]
        {
            get
            {
                try
                {
                    return _items[y * Width + x];
                }
                catch (ArgumentOutOfRangeException e)
                {
                    Debug.LogError("Invalid inventory access. [" + x + ", " + y + "] while Width = " + Width + " Height = " + Height);
                    return null;
                }
            }
            set
            {
                _items[y * Width + x] = value;
                if (ListeningPlayers.Count > 0)
                {
                    SendUpdateToPlayers(x, y);
                }
            }
        }

        private void SendUpdateToPlayers(int x, int y)
        {
            var item = this[x, y];
            foreach (var player in ListeningPlayers)
            {
                if (player == null)
                    continue;

                player.ClientUi.Inventories[Unit.ID, x, y] = item;
            }
        }

        private void SendUpdateToPlayers(int index)
        {
            int y = index/Width;
            int x = index - y * Width;
            SendUpdateToPlayers(x, y);
        }

        protected override void OnExtensionWasAdded()
        {
            base.OnExtensionWasAdded();
            Unit = entity as ServerUnit;
            RecreateInventory();
        }

        public override void Progress()
        {

        }

        public override void Serialize(ByteStream bytestream)
        {
            bytestream.AddInt((int)AccesType);
            for (int i = 0; i < Width * Height; i++)
            {
                bytestream.AddShort(_items[i] == null ? -1 : _items[i].InContentManagerIndex);
            }
        }

        public override void Deserialize(ByteStream bytestream)
        {
            AccesType = (AccessType) bytestream.GetInt();
            for (int i = 0; i < Width * Height; i++)
            {
                int id = bytestream.GetShort();
                AddItem(id == -1 ? null : ContentManager.I.Items[id]);
            }
        }

        public bool HasAcces(ServerUnit unit)
        {
            if (AccesType == AccessType.ALL)
                return true;
            if (AccesType == AccessType.ONLY_THIS_UNIT)
                return unit == Unit;
            return false;
        }

        public bool AddItem(Item item)
        {
            for (int y = 0; y < Height; y++)
            {
                for (int x = 0; x < Width; x++)
                {
                    if (this[x, y] == null)
                    {
                        this[x, y] = item;
                        return true;
                    }
                }
            }
            return false;
        }

        public bool HasSpace(int amount)
        {
            return GetSpace() >= amount;
        }

        public int GetSpace()
        {
            int space = Width * Height;
            foreach (var item in _items)
            {
                if (item != null)
                {
                    space--;
                }
            }
            return space;
        }

        public void OnInterfaceEvent(Player player, UIInterfaceEvent e)
        {
            if (HasAcces(player))
            {
                int itemPtr = e.controlID - 1;

                int x = itemPtr - (int)(itemPtr / Height) * Height;
                int y = itemPtr / Height;

                Item selectedItem = this[x, y];
                Debug.Log("action:" + e.Action + " x :" + x + " y: " + y + " item: " + selectedItem);

                if (selectedItem == null)
                    return;

                if (e.Action == "Drop")
                {
                    DroppedItem droppedItem = ServerMonoBehaviour.CreateInstance<DroppedItem>();

                    droppedItem.Movement.Teleport(player.Movement.Position);
                    droppedItem.Item = selectedItem;
                    player.CurrentWorld.AddEntity(droppedItem);

                    this[x, y] = null;
                }
                if (e.Action == "Equip")
                {
                    UnitEquipment equipment = player.Equipment;

                    this[x, y] = null;

                    equipment.EquipItem(selectedItem);
                }
            }
            else
            {
                Debug.LogError("Player has no access.");
            }

        }

        public void MoveItem(int index, int targetIndex)
        {
            index -= 1;
            targetIndex -= 1;
            if (_items[index] != null && _items[targetIndex] == null)
            {
                _items[targetIndex] = _items[index];
                _items[index] = null;
                if (ListeningPlayers.Count > 0)
                {
                    SendUpdateToPlayers(index);
                    SendUpdateToPlayers(targetIndex);
                }
            }
        }
    }
}
#endif