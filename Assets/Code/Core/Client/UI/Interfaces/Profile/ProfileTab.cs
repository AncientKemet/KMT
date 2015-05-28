using Client.Net;
using Client.Units;
using Code.Core.Client.UI.Controls;
using Code.Libaries.Net.Packets.ForServer;
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
            OnLeftClick += delegate
            {
                ProfileInterface.I.CurrentTab = this;
                UIInterfaceEvent packetEvent = new UIInterfaceEvent();

                packetEvent.controlID = Index;
                packetEvent.interfaceId = InterfaceId;
                packetEvent._eventType = UIInterfaceEvent.EventType.CLICK;
                packetEvent.Action = "View "+name;

                ClientCommunicator.Instance.SendToServer(packetEvent);
            };
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
