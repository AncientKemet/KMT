using System.Collections.Generic;
using Code.Core.Shared.Content;
using Shared.SharedTypes;
using Shared.StructClasses;
using UnityEngine;

namespace Shared.Content.Types
{
    public class ItemRecipe : ContentItem
    {
        public List<LevelRequirement> Requirements;
        public List<ExperienceReward> Rewards;
        public Item.ItemInstance Item1;
        public bool isConsumed1 = false;
        public Item.ItemInstance Item2;
        public bool isConsumed2 = false;
        public Item.ItemInstance Result;

        //next properties are used only for editor
#if UNITY_EDITOR
        public bool _foldOutRequirements { get; set; }
        public bool _foldOutRewards { get; set; }
#endif
    }
}
