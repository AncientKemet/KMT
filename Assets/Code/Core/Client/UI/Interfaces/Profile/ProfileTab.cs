using Client.Units;
using Code.Core.Client.UI.Controls;
using Code.Core.Client.Units;
using UnityEngine;

namespace Client.UI.Interfaces.Profile
{
    public class ProfileTab : UIControl
    {
        public GameObject ContentGameObject;

        protected override void Start()
        {
            base.Start();
            OnLeftClick += delegate { ProfileInterface.I.CurrentTab = this; };
        }

        public virtual void ReloadFromUnit(PlayerUnit unit)
        {
            
        }
    }
}
