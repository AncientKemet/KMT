#if SERVER
using Server.Model.Extensions;
using Server.Model.Extensions.UnitExts;

namespace Server.Model.Entities.Human
{
    public class Human : ServerUnit
    {
        public UnitInventory Inventory;

        public UnitEquipment Equipment;

        protected override ServerUnitPrioritization GetPrioritization()
        {
            return ServerUnitPrioritization.Realtime;
        }

        /// <summary>
        /// An human is can move from chunk to chunk so he's not static.
        /// </summary>
        /// <returns></returns>
        public override bool IsStatic()
        {
            return false;
        }

        public override void Awake()
        {
            Anim = AddExt<UnitAnim>();
            Attributes = AddExt<UnitAttributes>();
            Combat = AddExt<UnitCombat>();
            Inventory = AddExt<UnitInventory>();
            Equipment = AddExt<UnitEquipment>();

            AddExt<CollisionExt>();

            Inventory.Width = 3;
            Inventory.Height = 5;

            base.Awake();
        }

    }
}

#endif
