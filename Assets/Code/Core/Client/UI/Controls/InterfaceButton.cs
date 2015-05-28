using System.Linq;
using Client.Net;
using Code.Libaries.Net.Packets.ForServer;
using UnityEngine;
using EventType = Code.Libaries.Net.Packets.ForServer.UIInterfaceEvent.EventType;

namespace Code.Core.Client.UI.Controls
{
    [RequireComponent(typeof (BoxCollider))]
    [ExecuteInEditMode]
    public class InterfaceButton : UIControl
    {

        protected virtual void Start()
        {
            base.Start();
            foreach (var rightClickAction in Actions)
            {
                RightClickAction action = rightClickAction;
                if(action.Name != "Cancel")
                rightClickAction.Action += () =>
                {
                    this.SendClickPacket(action.Name);
                };
            }

            Clickable clickable = this;
            OnLeftClick += delegate()
            {
                RightClickAction rightClickAction = clickable.Actions.Last();
                if (rightClickAction != null && rightClickAction.Action != null)
                    rightClickAction.Action();
            };
        }

        protected void SendClickPacket(string actionName)
        {
            UIInterfaceEvent packetEvent = new UIInterfaceEvent();

            packetEvent.controlID = Index;
            packetEvent.interfaceId = InterfaceId;
            packetEvent._eventType = EventType.CLICK;
            packetEvent.Action = actionName;

            ClientCommunicator.Instance.SendToServer(packetEvent);
        }

        public override void AddAction(RightClickAction action)
        {
            action.Action += () => SendClickPacket(action.Name);
            base.AddAction(action);
        }
   }
}
