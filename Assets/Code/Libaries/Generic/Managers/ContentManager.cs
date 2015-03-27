using System.Collections.Generic;
using Code.Core.Shared.Content.Types;
using Pathfinding;
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
        public List<GameObject> Effects;
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

        public void CreateEffect(int i, Vector3 position)
        {
            Instantiate(Effects[i], position, Quaternion.identity);
        }
    }
}

