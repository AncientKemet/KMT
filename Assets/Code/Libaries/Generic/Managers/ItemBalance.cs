using System;
using System.Collections.Generic;
using Code.Core.Shared.Content.Types.ItemExtensions;
using Shared.Content.Types;
using Shared.Content.Types.ItemExtensions;
using UnityEngine;
using System.Collections;

#if UNITY_EDITOR
using UnityEditor;
#endif

namespace Code.Libaries.Generic.Managers
{
    public class ItemBalance : SIAsset<ItemBalance>
    {
        public List<ItemGrowth> Growths;
        public List<RoleMultiplier> Multipliers;

#if UNITY_EDITOR
        [MenuItem("Kemet/Items/Weapon balance")]
        private static void SelectAsset()
        {
            Selection.activeObject = I;
            I.Rebalance();
        }

        private void Rebalance()
        {
            foreach (var item in ContentManager.I.Items)
            {
                if (item.EQ != null)
                {
                    item.EQ.Rebalance();
                }
            }
        }

#endif

        [Serializable]
        public class ItemGrowth
        {
            public Class Class;
            public List<UnitAttributePropertySerializable> MaxLevel;
        }

        [Serializable]
        public class RoleMultiplier
        {
            public Role Role;
            public List<UnitAttributePropertySerializable> Multiplier;
        }
    }
}
