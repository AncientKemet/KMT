using System.Collections.Generic;
using Code.Core.Shared.Content.Types.ItemExtensions;
using Code.Libaries.Generic.Managers;
using UnityEngine;

namespace Shared.Content.Types.ItemExtensions
{

    public enum WeaponClass
    {
        Axe,Pickaxe,Spear
    }

    [ExecuteInEditMode]
    public class Weapon : EquipmentItem
    {
        public WeaponClass Class;
        [Range(1,30)]
        public int RequiredLevel = 1;

        public List<UnitAttributePropertySerializable> Secondary;
        

#if UNITY_EDITOR
        void Update () {
            if (Application.isPlaying) { 
	            Attributes = new List<UnitAttributePropertySerializable>();
                WeaponBalance.WeaponGrowth growth = WeaponBalance.I.Growths.Find(weaponGrowth => weaponGrowth.Class == Class);

                foreach (var a in growth.Minimum)
                {
                    Attributes.Add(new UnitAttributePropertySerializable(){Property = a.Property, Value = a.Value});
                }
                foreach (var a in growth.PerLevel)
                {
                    Attributes.Add(new UnitAttributePropertySerializable(){Property = a.Property, Value =  a.Value * RequiredLevel});
                }
            }
        }
#endif
    }
}
