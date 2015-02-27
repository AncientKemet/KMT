using System;
using System.Collections.Generic;
using Client.UI.Interfaces;
using Client.UI.Scripts;
using Code.Core.Client.UI;
using Code.Core.Client.UI.Interfaces;
using Code.Core.Client.UI.Interfaces.UpperLeft;
using Code.Core.Shared.Content.Types;
using Shared.Content.Types;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace Code.Libaries.Generic.Managers
{
    public class UIContentManager : SIAsset<UIContentManager>
    {
        public tk2dSlicedSprite ItemButtonBackGround;

        public List<SpellColor> SpellColors;
        public List<BuffColor> BuffColors;
        public List<AttributeColor> AttributeColors;

        [Serializable]
        public class AttributeColor
        {
            public UnitAttributeProperty Property;
            public Color Color = Color.white;
        }

        [Serializable]
        public class BuffColor
        {
            public Buff.Type Type;
            public Color Color = Color.white;
        }

        [Serializable]
        public class SpellColor
        {
            public SpellType Type;
            public Color Color = Color.white;
        }

#if UNITY_EDITOR
        [MenuItem("Kemet/Open/UIContentManager")]
        private static void SelectAsset()
        {
            Selection.activeObject = I;
        }
#endif
    }
}

