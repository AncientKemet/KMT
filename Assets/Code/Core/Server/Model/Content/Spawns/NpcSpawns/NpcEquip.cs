using System.Collections.Generic;
using Code.Core.Shared.Content.Types.ItemExtensions;
using Server.Model.Content;
using Server.Model.Entities.Human;
using Server.Model.Entities.Items;
using Shared.Content.Types;
using UnityEngine;
using System.Collections;

public class NpcEquip : NpcSpawnExtension
{
    
    public List<EquipmentItem> Items; 

    public override void Apply(NPC n)
    {
        foreach (var eqItem in Items)
        {
            DroppedItem droppedItem = ServerMonoBehaviour.CreateInstance<DroppedItem>();

                        droppedItem.Movement.Teleport(n.Movement.Position + n.Movement.Forward);
                        droppedItem.Item = new Item.ItemInstance(eqItem.Item);
                        n.CurrentWorld.AddEntity(droppedItem);
            
            n.Equipment.EquipItem(droppedItem);
        }

        
    }
}
