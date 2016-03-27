using System.Collections.Generic;
using Client.Units;
using Code.Core.Shared.Content;
using Pathfinding;
using Server.Model.Content.Spawns;
using Shared.Content.Types;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace Code.Libaries.Generic.Managers
{
    public class ContentManager : SIAsset<ContentManager>
    {
        public List<Item> Items;
        public List<UnitPrefab> Models;
        public List<Spell> Spells;
        public List<Buff> Buffs;
        public List<Effect> Effects;
        public List<ASpellRadius> SpellRadiuses;
        public List<NPCSpawn> Npcs; 
        public List<ItemRecipe> Recipes; 
        public Spell RestSpell;
        public Buff OverpowerDebuff;


#if UNITY_EDITOR
        [MenuItem("Kemet/ContentManager")]
        private static void SelectAsset()
        {
            Selection.activeObject = I;
        }
        [UnityEditor.MenuItem("Kemet/Pathfinding/Scan All recast graphs")]
        public static void MenuScan()
        {

            if (AstarPath.active == null)
            {
                AstarPath.active = FindObjectOfType(typeof(AstarPath)) as AstarPath;
                if (AstarPath.active == null)
                {
                    return;
                }
            }

            foreach (var graph in AstarPath.active.graphs)
            {

                if (graph is RecastGraph)
                {
                    graph.ScanInternal();
                }
            }
        }

#endif
    }
}

