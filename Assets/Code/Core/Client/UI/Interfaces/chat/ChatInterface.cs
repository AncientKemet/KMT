using Client.UI.Controls;
using Client.UI.Scripts;
using Code.Core.Client.Controls;
using Code.Core.Client.UI.Controls;
namespace Code.Core.Client.UI.Interfaces
{
    public class ChatInterface : UIInterface<ChatInterface> {

        public CircleButton MainButton;

        #region implemented abstract members of Interface

        protected override void OnStart()
        {
            MainButton.AddAction(new RightClickAction("Public", delegate() { ChatPanel.I.CurrentChatType = Libaries.Net.Packets.ForServer.ChatPacket.ChatType.Public; }));
            MainButton.AddAction(new RightClickAction("Private", delegate() { ChatPanel.I.CurrentChatType = Libaries.Net.Packets.ForServer.ChatPacket.ChatType.Private; }));
            MainButton.AddAction(new RightClickAction("Party", delegate() { ChatPanel.I.CurrentChatType = Libaries.Net.Packets.ForServer.ChatPacket.ChatType.Party;}));
            MainButton.AddAction(new RightClickAction("Talk", delegate() { ChatPanel.I.EnterPressed(); }));
            
        }

        protected override void OnUpdate()
        {

        }

        protected override void OnFixedUpdate()
        {

        }

        protected override void OnLateUpdate()
        {

        }

        protected override void OnVisibiltyChanged()
        {

        }

        #endregion
        	
    }
}
