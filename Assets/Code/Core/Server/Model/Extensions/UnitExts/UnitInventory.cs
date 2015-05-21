using System.Collections.ObjectModel;
using Libaries.IO;
using Shared.Content.Types;
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

        private List<Item.ItemInstance> _items;

        public ServerUnit Unit { get; private set; }

        public List<Player> ListeningPlayers = new List<Player>();

        public Action<ServerUnit> OnWasOpened;
        public Action<ServerUnit> OnWasClosed;


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
            _items = new List<Item.ItemInstance>(Width * Height);
            for (int i = 0; i < Width * Height; i++)
            {
                _items.Add(null);
            }
        }

        public Item.ItemInstance this[int index]
        {
            get
            {
                try
                {
                    return _items[index];
                }
                catch (ArgumentOutOfRangeException e)
                {
                    Debug.LogError("Invalid inventory access. [" + index + "] while Width = " + Width + " Height = " + Height);
                    return null;
                }
            }
            set
            {
                _items[index] = value;
                if (ListeningPlayers.Count > 0)
                {
                    SendUpdateToPlayers(index);
                }
            }
        }

        public Item.ItemInstance this[int x, int y]
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

        public override void Progress(float time)
        {

        }

        public override void Serialize(JSONObject json)
        {
            JSONObject inventory = new JSONObject();

            inventory.AddField("count", "" + (Width * Height));

            for (int i = 0; i < Width * Height; i++)
            {
                inventory.AddField("id " + i, "" + (_items[i] == null ? -1 : _items[i].Item.InContentManagerIndex));
                inventory.AddField("am " + i, "" + (_items[i] == null ? -1 : _items[i].Amount));
            }

            json.AddField("Inventory", inventory);
        }

        public override void Deserialize(JSONObject json)
        {
            JSONObject inventory = json.GetField("Inventory");

            for (int i = 0; i < int.Parse(inventory.GetField("count").str); i++)
            {
                int id = int.Parse(inventory.GetField("id "+i).str);
                this[i]=(id == -1 ? null : new Item.ItemInstance(ContentManager.I.Items[id], int.Parse(inventory.GetField("am "+i).str)));
            }
        }
        
        public bool AddItem(Item.ItemInstance item)
        {
            int nullIndexX = -1;
            int nullIndexY = -1;
            for (int y = 0; y < Height; y++)
            {
                for (int x = 0; x < Width; x++)
                {
                    var that = this[x, y];
                    if(that != null)
                    if (that.Item == item.Item)
                    {
                        if (that.Amount < that.Item.MaxStacks)
                        {
                            if (item.Amount > that.Item.MaxStacks - that.Amount)
                            {
                                item.Amount -= that.Item.MaxStacks - that.Amount;
                                that.Amount = that.Item.MaxStacks;
                                SendUpdateToPlayers(x, y);
                                return AddItem(item);
                            }
                            else
                            {
                                that.Amount += item.Amount;
                                SendUpdateToPlayers(x, y);
                                return true;
                            }
                        }
                    }
                    if (that == null && nullIndexX == -1)
                    {
                        nullIndexX = x;
                        nullIndexY = y;
                    }
                }
            }
            if (nullIndexX != -1)
            {
                this[nullIndexX, nullIndexY] = item;
                return true;
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
            if (Unit.Access == null || Unit.Access.GetAccessFor(player).Take_From_Inventory)
            {
                int itemPtr = e.controlID;

                int x = itemPtr - (int)(itemPtr / Height) * Height;
                int y = itemPtr / Height;

                Item.ItemInstance selectedItem = this[x, y];
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
                    if (selectedItem.Item.EQ != null)
                    {
                        UnitEquipment equipment = player.Equipment;

                        DroppedItem droppedItem = ServerMonoBehaviour.CreateInstance<DroppedItem>();

                        droppedItem.Movement.Teleport(player.Movement.Position + player.Movement.Forward);
                        droppedItem.Item = selectedItem;
                        player.CurrentWorld.AddEntity(droppedItem);

                        if (equipment.EquipItem(droppedItem))
                        {
                            player.Anim.SetDefaults();
                            this[x, y] = null;
                        }
                    }
                }
            }
            else
            {
                Debug.LogError("Player has no access.");
            }

        }

        public void MoveItem(int index, int targetIndex, UnitInventory inventory)
        {
            if (_items[index] != null && inventory._items[targetIndex] == null)
            {
                inventory._items[targetIndex] = _items[index];
                _items[index] = null;
                if (ListeningPlayers.Count > 0)
                {
                    SendUpdateToPlayers(index);
                    inventory.SendUpdateToPlayers(targetIndex);
                }
            }
        }

        public ReadOnlyCollection<Item.ItemInstance> GetItems()
        {
            return _items.AsReadOnly();
        }

        public int AmountOfItemsPossibleToBePutIn(Item.ItemInstance i)
        {
            int r = 0;
            if (i.Item.Stackable)
            {
                foreach (var item in _items)
                {
                    if (item.Item == i.Item)
                    {
                        if (item.Amount < item.Item.MaxStacks)
                        {
                            r += item.Item.MaxStacks - item.Amount;
                        }
                    }
                }
                r += GetSpace()*i.Item.MaxStacks;
            }
            else
            {
                r += GetSpace();
            }

            return r;
        }

        public int GetCoinValue()
        {
            int r = 0;
            foreach (var item in _items)
            {
                int id = item.Item.InContentManagerIndex;
                if (id >= 0 && id <= 6)
                {
                    r += item.Item.Value*item.Amount;
                }
            }
            return r;
        }

        public void RefreshFull()
        {
            for (int i = 0; i < Width*Height; i++)
            {
                SendUpdateToPlayers(i);
            }

        }
    }
}
#endif
