using System;
using System.Collections.Generic;
using Code.Core.Shared.Content;
using Development.Libary.Spells.Codes;
using Shared.SharedTypes;
using Shared.StructClasses;
using UnityEditorInternal.VersionControl;
using UnityEngine;

namespace Shared.Content.Types
{
    public class ItemRecipe : ContentItem
    {
        public List<LevelRequirement> Requirements;
        public List<ExperienceReward> Rewards;
        public List<ItemRequirement> ItemRequirements;
        public Item.ItemInstance Result;
        public List<Item.ItemInstance> SideProducts;
        public CraftingSpell CraftingSpell;
        public float CraftTime = 1f;

        //next properties are used only for editor
#if UNITY_EDITOR
        public bool _foldOutRequirements { get; set; }
        public bool _foldOutRewards { get; set; }
        public bool _foldOutItemRequirements { get; set; }
        public bool _foldOutSideProducts { get; set; }
#endif

        [Serializable]
        public class ItemRequirement
        {
            public Item.ItemInstance Item;
            public bool IsConsumed = false;
        }
    }
}
