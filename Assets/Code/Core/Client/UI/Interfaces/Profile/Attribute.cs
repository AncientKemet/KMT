using Code.Core.Client.UI.Controls;
using UnityEngine;

namespace Client.UI.Interfaces.Profile
{
    public class Attribute : Clickable
    {

        public string AttributeName = "";
        [Multiline(5)]
        public string AttributeDesc = "";

        public tk2dTextMesh Title, Text;

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
    }
}
