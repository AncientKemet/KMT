using Client.Units;
using Code.Core.Client.UI.Controls;
using Code.Core.Client.Units;
using UnityEngine;

namespace Client.UI.Interfaces.Profile
{
    public class ProfileTab : UIControl
    {
        public GameObject ContentGameObject;
        public string Describtion;

        protected override void Start()
        {
            base.Start();
            OnLeftClick += delegate { ProfileInterface.I.CurrentTab = this; };
            OnMouseIn += () =>
            {
                DescriptionInterface.I.Show("", Describtion);
            };
            OnMouseOff += () =>
            {
                DescriptionInterface.I.Hide();
            };
        }

        public virtual void ReloadFromUnit(PlayerUnit unit)
        {
            
        }
    }
}
