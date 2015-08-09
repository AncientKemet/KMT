#if UNITY_EDITOR
#endif
using System;
using System.Collections.Generic;
using Code.Libaries.Generic.Managers;
using Shared.Content.Types;
using Shared.Content.Types.ItemExtensions;
using UnityEngine;

namespace Code.Core.Shared.Content.Types.ItemExtensions
{
    [Serializable]
    public class EquipmentItem : ItemExtension
    {
        public enum Type 
        {
            MainHand,
            OffHand,
            Helm,
            Body,
            Legs,
            Boots,
        }

        public Type EquipType;

        public bool CanBeStoredInInventory = true;

        public Spell[] Spells;

        public string StandAnim = "Idle";
        public string WalkAnim = "Walk";

        public Class Class;
        public Role Role;
        [Range(1, 30)]
        public int RequiredLevel = 1;

        public List<UnitAttributePropertySerializable> Attributes;

        private void OnValidate()
        {
            Rebalance();
        }

#if UNITY_EDITOR

        public void Rebalance()
        {
            Attributes = new List<UnitAttributePropertySerializable>();

            if (Class != Class.NONE)
            {


                ItemBalance.ItemGrowth growth = ItemBalance.I.Growths.Find(itemGrowth => itemGrowth.Class == Class);
                ItemBalance.RoleMultiplier multiplier = ItemBalance.I.Multipliers.Find(m => m.Role == Role);
                if (growth == null)
                {
                    Debug.LogError("Item "+name+" has missing class "+Class);
                    return;
                }
                foreach (var a in growth.MaxLevel)
                {
                    var find = Attributes.Find(serializable => serializable.Property == a.Property);
                    if (find != null)
                        find.Value += (a.Value * ((10 + RequiredLevel) / 40f)) *
                                      (multiplier == null
                                          ? 1f
                                          : 1f +
                                            multiplier.Multiplier.Find(
                                                serializable => serializable.Property == a.Property).Value);
                    else
                        Attributes.Add(new UnitAttributePropertySerializable()
                        {
                            Property = a.Property,
                            Value = (a.Value*((10+RequiredLevel)/40f))*
                                      (multiplier == null
                                          ? 1f
                                          : 1f +
                                            multiplier.Multiplier.Find(
                                                serializable => serializable.Property == a.Property).Value)
                        });
                }
                for (int i = 0; i < Attributes.Count; i++)
                {
                    if(Attributes[i].Property != UnitAttributeProperty.CriticalArea)
                    if (Attributes[i].Value < 0.05 && Attributes[i].Value > -0.05)
                    {
                        Attributes.Remove(Attributes[i]);
                        i--;
                    }
                }
            }
        }
#endif
    
    }
}
