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
        WeaponReach,
        ArmorPenetration,
        MagicResistPenetration,
        Offence,
        Toughtness
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
            float val = f;
            string addition = "";
            switch (property)
            {

                case UnitAttributeProperty.Health:
                case UnitAttributeProperty.Energy:
                case UnitAttributeProperty.Toughtness:
                    break;
                case UnitAttributeProperty.WeaponReach:
                    addition = "m";
                    break;
                case UnitAttributeProperty.HealthRegen:
                case UnitAttributeProperty.EnergyRegen:
                case UnitAttributeProperty.Offence:
                    addition = "/s";
                    break;
                default:
                    val *= 100f;
                    addition = "%";
                    break;
                    
            }
            return val + addition;
        }
    }
}
