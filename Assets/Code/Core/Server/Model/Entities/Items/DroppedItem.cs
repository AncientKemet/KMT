#if SERVER
using System.Collections.Generic;
using Code.Libaries.Generic.Managers;
using UnityEngine;

using Code.Core.Shared.Content.Types;
using Code.Core.Shared.Content.Types.ItemExtensions;
using Server.Model.Extensions.UnitExts;

namespace Server.Model.Entities.Items
{
    public class DroppedItem : ServerUnit
    {
        public enum DroppedItemAccessType
        {
            ALL,
            List
        }

        private Item _item;

        protected override ServerUnitPrioritization GetPrioritization()
        {
            return ServerUnitPrioritization.Low;
        }

        public override bool IsStatic()
        {
            return false;
        }

        public readonly List<ServerUnit> AccessList = new List<ServerUnit>();
        public DroppedItemAccessType AccessType = DroppedItemAccessType.ALL;

        /// <summary>
        /// The time that has to be gone, before this item becomes public.
        /// </summary>
        public float AccessDelay { get; set; }

        public Item Item
        {
            get { return _item; }
            set
            {
                _item = value;

                name = Item.name;

                Display.Item = _item;
                Movement.CanMove = false;
                Movement.CanRotate = false;

                ItemWithInventory withInventory = value.GetComponent<ItemWithInventory>();

                if (withInventory != null)
                {
                    UnitInventory inventory;

                    AddExt(inventory = new UnitInventory());

                    inventory.Width = withInventory.Width;
                    inventory.Height = withInventory.Height;
                }
            }
        }

        public override void Progress()
        {
            base.Progress();
            if (AccessDelay > 0)
            {
                AccessDelay -= Time.fixedDeltaTime;
                if (AccessDelay < 0)
                {
                    AccessType = DroppedItemAccessType.ALL;
                }
            }
        }

        public static DroppedItem Spawn(int _itemId, Vector3 position, World currentWorld)
        {
            DroppedItem i = CreateInstance<DroppedItem>();
            i.Item = ContentManager.I.Items[_itemId];
            i.Movement.Teleport(position);
            currentWorld.AddEntity(i);
            return i;
        }
    }
}

#endif
