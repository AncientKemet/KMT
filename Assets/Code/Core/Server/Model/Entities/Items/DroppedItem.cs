using Shared.Content.Types;
#if SERVER
using System.Collections.Generic;
using UnityEngine;
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

        private Item.ItemInstance _item;

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
        private int _amount = 1;

        /// <summary>
        /// The time that has to be gone, before this item becomes public.
        /// </summary>
        public float AccessDelay { get; set; }

        public Item.ItemInstance Item
        {
            get { return _item; }
            set
            {
                _item = value;
                name = Item.Item.name;

                Display.Item = _item.Item;
                Movement.CanMove = false;
                Movement.CanRotate = false;

                ItemWithInventory withInventory = value.Item.GetComponent<ItemWithInventory>();

                if (withInventory != null)
                {
                    if(withInventory.HasAnimation)
                        Anim = AddExt<UnitAnim>();

                    Anim.StandAnimation = "Idle";
                    Anim.RunAnimation = "Idle";

                    UnitInventory inventory;

                    inventory = AddExt<UnitInventory>();

                    inventory.Width = withInventory.Width;
                    inventory.Height = withInventory.Height;
                }
            }
        }

        public int Amount
        {
            get { return _amount; }
            set
            {
                _amount = value;
                if (value <= 0)
                    Display.Destroy = true;
            }
        }

        public override void Progress(float time)
        {
            base.Progress(time);
            if (AccessDelay > 0)
            {
                AccessDelay -= Time.fixedDeltaTime;
                if (AccessDelay < 0)
                {
                    AccessType = DroppedItemAccessType.ALL;
                }
            }
        }
    }
}

#endif
