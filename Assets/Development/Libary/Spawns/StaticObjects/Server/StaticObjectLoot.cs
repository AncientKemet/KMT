using System.Collections.Generic;
using System.Linq;
using Server.Model.Entities;
using Server.Model.Entities.Human;
using Server.Model.Extensions.UnitExts;
using Shared.Content;
using Shared.Content.Types;
using UnityEngine;

namespace Development.Libary.Spawns.StaticObjects.Server
{
    public class StaticObjectLoot : ServerStaticObjectExtension
    {
        public float RespawnTime = 25f;
        public List<Item.ItemInstance> Items;
        
        public override void Apply(ServerUnit serverUnit)
        {
            var loot = serverUnit.GetExt<UnitInventory>();
            var access = serverUnit.Access;
            var details = serverUnit.Details;

            if (access == null)
                access = serverUnit.AddExt<UnitAccessOwnership>();
            if (loot == null)
                loot = serverUnit.AddExt<UnitInventory>();
            if (details == null)
                details = serverUnit.AddExt<UnitDetails>();

            serverUnit.Combat.OnDeath += delegate(Dictionary<ServerUnit, float> damages)
            {
                List<KeyValuePair<ServerUnit, float>> myList = damages.ToList();

                myList.Sort(
                    delegate(KeyValuePair<ServerUnit, float> firstPair,
                    KeyValuePair<ServerUnit, float> nextPair)
                    {
                        return firstPair.Value.CompareTo(nextPair.Value);
                    }
                );

                var looter = myList.Last().Key;

                if (looter is Player)
                {
                    Player p = looter as Player;
                    
                    loot.Clear();
                    loot.AddItem(Items[Random.Range(0, Items.Count)]);

                    access.ClearAccesses();
                    access.SetAccess(p, new UnitAccess() {Take_From_Inventory = true, View_Inventory = true});

                    details.RemoveAction("Loot");
                    details.AddAction("Loot");
                }

                serverUnit.StartCoroutine(SEase.Action(() =>
                {
                    serverUnit.Combat.Revive(100);

                    loot.Clear();
                    access.ClearAccesses();
                    details.RemoveAction("Loot");
                }, -1, RespawnTime));
            };


        }
    }
}
