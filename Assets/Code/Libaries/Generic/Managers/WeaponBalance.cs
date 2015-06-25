using System;
using System.Collections.Generic;
using Shared.Content.Types;
using Shared.Content.Types.ItemExtensions;
using UnityEngine;
using System.Collections;

#if UNITY_EDITOR
using UnityEditor;
#endif

namespace Code.Libaries.Generic.Managers
{
    public class WeaponBalance : SIAsset<WeaponBalance>
    {
        public List<WeaponGrowth> Growths;

#if UNITY_EDITOR
        [MenuItem("Kemet/Items/Weapon balance")]
        private static void SelectAsset()
        {
            Selection.activeObject = I;
        }

#endif

        [Serializable]
        public class WeaponGrowth
        {
            public WeaponClass Class;
            public List<UnitAttributePropertySerializable> Minimum;
            public List<UnitAttributePropertySerializable> PerLevel;
        }
    }
}
