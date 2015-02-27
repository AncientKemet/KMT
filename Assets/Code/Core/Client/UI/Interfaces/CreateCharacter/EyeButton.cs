using UnityEngine;

namespace Client.UI.Interfaces.CreateCharacter
{
    public class EyeButton : SelectionButton {

        public MeshRenderer EyeRender;

        public tk2dBaseSprite Locked;
        
        protected override void OnSelected()
        {
        }
    }
}
