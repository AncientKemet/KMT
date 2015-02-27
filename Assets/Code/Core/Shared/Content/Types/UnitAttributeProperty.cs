using System;
using Code.Libaries.Generic.Managers;
using UnityEngine;

namespace Shared.Content.Types
{
    public enum UnitAttributeProperty
    {
        PhysicalDamage,
        MagicalDamage,
        Armor,
        MagicResist,
        Health,
        HealthRegen,
        Energy,
        EnergyRegen,
        MovementSpeed,
        Mobility,
        ChargeSpeed,
        CriticalDamage,
        CriticalArea,
        DamageToTreePalm,
        DamageToShield,
        DamageToAnimal,
        DamageToMineral,
        StrenghtBonus,
        WisdomBonus,
        DexterityBonus,
        WeaponReach,
    }

    [Serializable]
    public class UnitAttributePropertySerializable
    {
        public UnitAttributeProperty Property;
        [Range(-1f, 2f)]
        public float Value;

        public static string GetLabeledString(UnitAttributePropertySerializable a)
        {
            string s = 
                HexConverter(UIContentManager.I.AttributeColors.Find(color => color.Property == a.Property).Color)+
                (a.Value >= 0 ? "+" : "-") +
                " " +
                (int)(a.Value * 100) + "%" +
                " " +
                a.Property;
            return s;
        }

        private static String HexConverter(Color c)
        {
            String rtn = String.Empty;
            try
            {
                rtn = "^C" + ((int)(c.r * 256f)).ToString("X2") + ((int)(c.g * 256f)).ToString("X2") + ((int)(c.b * 256f)).ToString("X2") + "FF";
            }
            catch (Exception ex)
            {
                //doing nothing
            }

            return rtn;
        }

    }
}
