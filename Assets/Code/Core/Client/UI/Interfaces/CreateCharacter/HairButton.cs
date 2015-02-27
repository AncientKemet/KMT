using UnityEngine;

namespace Client.UI.Interfaces.CreateCharacter
{
    public class HairButton : SelectionButton
    {

        public MeshRenderer HairRender;
        public MeshFilter HairFilter;
        public MeshRenderer EarRender;
        public tk2dBaseSprite Locked;
        public tk2dBaseSprite BackGround;

        protected override void Start()
        {
            base.Start();
        }

        protected override void OnSelected()
        {
        }
    }
}
