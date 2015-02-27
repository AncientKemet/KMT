using System;
using Client.Net;
using Client.UI.Scripts;
using Code.Core.Client.UI.Controls;

namespace Client.UI.Interfaces.Lobby
{
    public class LobbyInterface : UIInterface<LobbyInterface>
    {
        public LobbyContentController ContentController;
        public Clickable ProfileButton, HomePageButton, StoreButton, PlayButton, PreferencesButton, LoreButton, LogoutButton;

        public tk2dTextMesh Username;
        private string _dataServerKey;

        public Action<string> OnDataServerKeyRecieved;

        public string DataServerKey
        {
            get { return _dataServerKey; }
            set
            {
                _dataServerKey = value;
                if (OnDataServerKeyRecieved != null)
                {
                    OnDataServerKeyRecieved(value);
                }
            }
        }

        

        private void Start()
        {
            LogoutButton.OnLeftClick += () => ClientCommunicator.Instance.LoginServerConnection.Disconnect();
            PlayButton.OnLeftClick += () => ContentController.LoadPage(1);
            PreferencesButton.OnLeftClick += () => ContentController.LoadPage(1);
        }

    }
}
