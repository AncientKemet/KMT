using Client.Net;
using Client.UI.Controls;
using Client.UI.Controls.Inputs;
using Client.UI.Controls.Tool;
using Code.Libaries.Net.Packets.ForServer;
using UnityEngine;

namespace Client.UI.Interfaces.Lobby
{
    public class LobbyChatPanel : TextButton
    {

        public Slider Slider;
        public TextButton CloseButton;


        public TextField TextField;
        public tk2dTextMesh _chat;
        public GameObject Panel;

        public tk2dTextMesh NewMessages;
        private bool _opened;
        private int _newMessAm = 0;

        public bool Opened
        {
            get { return _opened; }
            set
            {
                if (_opened != value)
                {
                    _opened = value;
                    Panel.gameObject.SetActive(value);
                    if (value)
                    {
                        _newMessAm = 0;
                        NewMessages.gameObject.SetActive(false);
                    }
                }
            }
        }

        protected override void Start()
        {
            base.Start();

            _chat.text = "";

            CloseButton.OnLeftClick += () => Destroy(gameObject);
            OnLeftClick += () => Opened = !Opened;
            
            TextField.LoseFocusOnEnter = false;

            TextField.OnEnter += () =>
            {
                AddMessage("^cD77FMe: " + TextField.Text);

                ChatPacket packet = new ChatPacket();
                packet.type = ChatPacket.ChatType.Private;
                packet.User = _textMesh.text;
                packet.text = TextField.Text;

                ClientCommunicator.Instance.LoginServerConnection.SendPacket(packet);
                TextField.Text = "";
            };
        }

        private void OnDestroy()
        {
            LobbyChatBar.instance.OnPanelWasDestroyed(this);
        }

        public void AddMessage(string message)
        {
            if (!Opened)
            {
                _newMessAm++;
                NewMessages.text = ""+ _newMessAm;
                NewMessages.gameObject.SetActive(true);
            }
            _chat.text += message + "\n";
        }

    }
}
