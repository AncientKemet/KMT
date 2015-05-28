#if UNITY_EDITOR
#endif
using System;
using Shared.Content.Types;

namespace Code.Core.Shared.Content.Types.ItemExtensions
{
    [Serializable]
    public class EquipmentItem : ItemExtension
    {
        public enum Type 
        {
            MainHand,
            OffHand,
            TwoHand,
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
    }
}
