using System;
using System.Collections.Generic;
using Code.Libaries.Generic.Managers;
using UnityEngine;

namespace Shared.Content.Types
{
    public enum UnitAttributeProperty
    {
        PhysicalDamage = 0,
        MagicalDamage=1,
        Armor=2,
        MagicResist=3,
        Health=4,
        HealthRegen=5,
        Energy=6,
        EnergyRegen = 7,
        MovementSpeed = 8,
        ChargeSpeed = 10,
        CriticalDamage = 11,
        CriticalArea = 12,
        WeaponReach = 13,
        ArmorPenetration = 14,
        MagicResistPenetration = 15,
        Offence = 125,
        Toughtness = 126
    }

    [Serializable]
    public class UnitAttributePropertySerializable
    {
        public UnitAttributeProperty Property;
        public float Value;

        public static string GetLabeledString(UnitAttributeProperty property)
        {
            string s;
            try
            {
                 s = HexConverter(UIContentManager.I.AttributeColors.Find(color => color.Property == property).Color) + property.ToString();
            }
            catch (NullReferenceException)
            {
                Debug.LogError("Missing UIContentManager.I.AttributeColors "+property);
                return "missing "+property;
            }
            return s;
        }

        private static String HexConverter(Color c)
        {
            String rtn = String.Empty;
            try
            {
                rtn = "^C" + ((int)(c.r * 255f)).ToString("X2") + ((int)(c.g * 255f)).ToString("X2") + ((int)(c.b * 255f)).ToString("X2") + "FF";
            }
            catch (Exception ex)
            {
                //doing nothing
            }

            return rtn;
        }

        public static string GetLabeledString(UnitAttributeProperty property, float f)
        {
            string s;
            switch (property)
            {
                case UnitAttributeProperty.Health:
                case UnitAttributeProperty.Energy:
                case UnitAttributeProperty.Toughtness:
                    s = f.ToString("##.#");
                    break;
                case UnitAttributeProperty.WeaponReach:
                    s = f.ToString("#.# 'm'");
                    break;
                case UnitAttributeProperty.HealthRegen:
                case UnitAttributeProperty.EnergyRegen:
                case UnitAttributeProperty.Offence:
                    s = f.ToString("##.## '/s'");
                    break;
                default:
                    s = (f * 100f).ToString("##.# '%'");
                    break;
            }
            return (f > 0 ? "+": "")+s;
        }
    }
}
