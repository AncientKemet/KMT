using Client.Net;
using Code.Core.Client.UI.Controls;
using Code.Libaries.Net.Packets.ForServer;

namespace Client.UI.Controls
{
    public class CloseInterfaceButton : UIControl {
        protected override void Start()
        {
            base.Start();
            AddAction(new RightClickAction("Close", delegate
            {
                UIInterfaceEvent e = new UIInterfaceEvent();
                e.Action = "Close interface";
                e.interfaceId = this.InterfaceId;
                ClientCommunicator.Instance.SendToServer(e);
            }));
        }
    }
}
