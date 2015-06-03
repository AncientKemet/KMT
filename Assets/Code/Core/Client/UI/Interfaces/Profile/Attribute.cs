using Code.Core.Client.UI.Controls;
using Code.Libaries.Generic.Managers;
using Shared.Content.Types;
using UnityEngine;

namespace Client.UI.Interfaces.Profile
{
    [ExecuteInEditMode]
    public class Attribute : Clickable
    {

        public UnitAttributeProperty Property;
        public string AttributeName = "";
        [Multiline(5)]
        public string AttributeDesc = "";

        public tk2dTextMesh Title, Text, ValueLabel;

        protected override void Start()
        {
            base.Start();
            OnMouseIn += ShowDescription;
            OnMouseOff += HideDescription;
        }

        private void HideDescription()
        {
            Title.text = "";
            Text.text = "";
        }

        private void ShowDescription()
        {
            Title.text = AttributeName;
            Text.text = AttributeDesc;
        }

#if UNITY_EDITOR
        private void Update()
        {
            if (!Application.isPlaying)
            {
                ValueLabel = GetComponentInChildren<tk2dTextMesh>();
                GetComponent<tk2dSprite>().color =
                    UIContentManager.I.AttributeColors.Find(color => color.Property == Property).Color;
                GetComponentInChildren<tk2dSlicedSprite>().color =
                    UIContentManager.I.AttributeColors.Find(color => color.Property == Property).Color;
            }
        }
#endif
    }
}
