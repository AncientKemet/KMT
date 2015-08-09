using System.Collections.Generic;
using Code.Core.Shared.Content.Types.ItemExtensions;
using Server.Model.Entities;
using Server.Model.Entities.Human;
using Server.Model.Entities.Items;
using Shared.Content.Types;

namespace Server.Model.Content.Spawns.NpcSpawns
{
    public class NpcEquip : NpcSpawnExtension
    {

        public List<EquipmentItem> Items; 

        public override void Apply(NPC n)
        {
            base.Apply(n);
            foreach (var eqItem in Items)
            {
                DroppedItem droppedItem = ServerMonoBehaviour.CreateInstance<DroppedItem>();

                droppedItem.Movement.Teleport(((ServerUnit) n).Movement.Position + ((ServerUnit) n).Movement.Forward);
                droppedItem.Item = new Item.ItemInstance(eqItem.Item);
                n.CurrentWorld.AddEntity(droppedItem);
            
                n.Equipment.EquipItem(droppedItem);
            }
        }
    }
}
