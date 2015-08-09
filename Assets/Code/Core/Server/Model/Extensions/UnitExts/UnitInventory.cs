using System.Collections.ObjectModel;
using System.Linq;
using Libaries.IO;
using Server.Model.ContentHandling;
using Shared.Content.Types;
#if SERVER
using Server.Model.Content;

using System;
using Code.Libaries.Generic.Managers;
using Code.Libaries.Net.Packets.ForServer;
using Server.Model.Entities.Items;
using UnityEngine;
using System.Collections.Generic;
using Server.Model.Entities;
using Server.Model.Entities.Human;

namespace Server.Model.Extensions.UnitExts
{
    public class UnitInventory : EntityExtension
    {
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
            private set
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
                    Debug.LogError("Invalid ArgumentOutOfRangeException. [" + x + ", " + y + "] while Width = " + Width + " Height = " + Height);
                    return null;
                }
            }
            private set
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
            for (int i = 0; i < ListeningPlayers.Count; i++)
            {
                if (ListeningPlayers[i] == null)
                    continue;

                ListeningPlayers[i].ClientUi.Inventories[Unit.ID, x, y] = item;
            }
        }

        private void SendUpdateToPlayers(int index)
        {
            int y = index / Width;
            int x = index - y * Width;
            SendUpdateToPlayers(x, y);
        }

        /// <summary>
        /// Gets an null item in the list.
        /// </summary>
        /// <returns>An index of items list that contains null.,</returns>
        private int GetFreeIndex()
        {
            for (int i = 0; i < _items.Count; i++)
            {
                if (_items[i] == null)
                    return i;
            }
            throw new Exception("There is no free space. Check for HasSpace in your code!");
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
                int id = int.Parse(inventory.GetField("id " + i).str);
                this[i] = (id == -1 ? null : new Item.ItemInstance(ContentManager.I.Items[id], int.Parse(inventory.GetField("am " + i).str)));
            }
        }


        public bool AddItem(Item.ItemInstance item)
        {
            if (item.Item.Stackable)
            {
                int max = item.Item.MaxStacks;

                var sameType = _items.FindAll(instance => instance != null && instance.Item == item.Item);
                var ascending = sameType.OrderBy(instance => instance.Amount);
                foreach (var instance in ascending)
                {
                    if (item.Amount == 0)
                        return true;
                    if (instance.Amount < max)
                    {
                        if (instance.Amount + item.Amount > max)
                        {
                            item.Amount -= max - instance.Amount;
                            instance.Amount = max;
                        }
                        else
                        {
                            item.Amount = 0;
                            instance.Amount += item.Amount;
                        }
                        SendUpdateToPlayers(_items.IndexOf(instance));
                    }
                }
            }

            if (item.Amount > 0)
            {
                int max = item.Item.MaxStacks;

                while (HasSpace(1) && item.Amount > 0)
                {
                    int free = GetFreeIndex();
                    if (item.Amount > max)
                    {
                        this[free] = new Item.ItemInstance(item.Item, max);
                        item.Amount -= max;
                    }
                    else
                    {
                        this[free] = new Item.ItemInstance(item.Item, item.Amount);
                        item.Amount = 0;
                        return true;
                    }
                }
            }

            return false;
        }

        public bool RemoveItem(Item.ItemInstance item)
        {
            var findAll = _items.FindAll(instance => instance != null && instance.Item == item.Item);
            int AmountRemoved = 0;
            foreach (var i in findAll)
            {

                int index = _items.IndexOf(i);
                if (i.Amount == item.Amount - AmountRemoved)
                {
                    AmountRemoved += i.Amount;
                    _items[index] = null;
                    SendUpdateToPlayers(index);
                    return true;
                }
                else if (i.Amount > item.Amount - AmountRemoved)
                {
                    i.Amount -= item.Amount;
                    SendUpdateToPlayers(index);
                    return true;
                }
                else
                {
                    AmountRemoved += i.Amount;
                    _items[index] = null;
                    SendUpdateToPlayers(index);
                }
            }
            return false;
        }

        public bool HasSpace(int amount)
        {
            return GetSpace() >= amount;
        }

        public bool HasSpace(Item.ItemInstance i)
        {
            return GetSpace() >= SpaceRequiredFor(i);
        }

        public int GetSpace()
        {
            int space = Width * Height;
            for (int i = 0; i < _items.Count; i++)
            {
                if (_items[i] != null)
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
                if (e.Action == "Open craft")
                {
                    player.ClientUi.CraftingInterface.Open(this);
                    return;
                }

                int itemPtr = e.controlID;
                Item.ItemInstance selectedItem = this[itemPtr];
                Debug.Log("action:" + e.Action + " item: " + selectedItem);

                if (selectedItem == null)
                    return;

                if (e.Action == "Drop")
                {
                    DroppedItem droppedItem = ServerMonoBehaviour.CreateInstance<DroppedItem>();

                    droppedItem.Movement.Teleport(player.Movement.Position);
                    droppedItem.Item = selectedItem;
                    player.CurrentWorld.AddEntity(droppedItem);

                    this[itemPtr] = null;
                }
                
                if (e.Action == "Equip")
                {
                    if (selectedItem.Item.EQ != null)
                    {
                        UnitEquipment equipment = player.Equipment;

                        DroppedItem droppedItem = CreateInstance<DroppedItem>();

                        droppedItem.Movement.Teleport(player.Movement.Position + player.Movement.Forward);
                        droppedItem.Item = selectedItem;
                        player.CurrentWorld.AddEntity(droppedItem);

                        if (equipment.EquipItem(droppedItem))
                        {
                            player.Anim.SetDefaults();
                            this[itemPtr] = null;
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

        public int GetCoinValue()
        {
            int r = 0;
            foreach (var item in _items)
            {
                int id = item.Item.InContentManagerIndex;
                if (id >= 0 && id <= 6)
                {
                    r += item.Item.Value * item.Amount;
                }
            }
            return r;
        }

        public void RefreshFull()
        {
            for (int i = 0; i < Width * Height; i++)
            {
                SendUpdateToPlayers(i);
            }
        }

        public void Clear()
        {
            RecreateInventory();
            RefreshFull();
        }

        public bool HasItem(Item.ItemInstance item)
        {
            var findAll = _items.FindAll(instance => instance != null && instance.Item == item.Item);
            int amount = findAll.Sum(i => i.Amount);
            return amount >= item.Amount;
        }

        public int SpaceFreedAfterRemoving(Item.ItemInstance item)
        {
            var findAll = _items.FindAll(instance => instance != null && instance.Item == item.Item);
            int amount = findAll.Sum(i => i.Amount);
            if (amount < item.Amount)
                return 0;
            return item.Amount / item.Item.MaxStacks + item.Amount % item.Item.MaxStacks == 0 ? 0 : 1;
        }

        public int SpaceRequiredFor(Item.ItemInstance item)
        {
            if (item.Item.Stackable)
            {
                int amount = item.Amount;
                var findAll = _items.FindAll(instance => instance != null && instance.Item == item.Item);
                foreach (var instance in findAll)
                {
                    if (instance.Amount < instance.Item.MaxStacks)
                    {
                        amount -= instance.Item.MaxStacks - instance.Amount;
                    }
                }
                return amount / item.Item.MaxStacks + amount % item.Item.MaxStacks == 0 ? 0 : 1;
            }
            else
            {
                return item.Amount;
            }
        }

    }
}
#endif
