using Client.Net;
using Libaries.IO;
using UnityEngine;

namespace Client.UI.Interfaces.Lobby
{
    public class PlayPage : LobbyPage
    {

        public WorldSelectButton ButtonPrefab;
        private Vector3 buttonOffset;

        protected override void OnDataWereLoaded()
        {
            string[] servers = Data.Split("$"[0]);

            foreach (var server in servers)
            {
                if(!string.IsNullOrEmpty(server))
                    AddServerButton(server);
            }
        }

        private void AddServerButton(string server)
        {

            JSONObject o = new JSONObject(server);

            WorldSelectButton newButton =
                ((GameObject) Instantiate(ButtonPrefab.gameObject)).GetComponent<WorldSelectButton>();
            
            newButton.Name = o.GetField("name").str;
            newButton.Type = o.GetField("type").str;
            newButton.OnlinePlayers = int.Parse(o.GetField("online").str);
            newButton.IpAdress = o.GetField("IP").str;

            newButton.gameObject.SetActive(true);

            newButton.transform.parent = ButtonPrefab.transform.parent;
            newButton.transform.position = ButtonPrefab.transform.position+ buttonOffset;
            
            ButtonPrefab.gameObject.SetActive(false);

            buttonOffset+= new Vector3(0, -2f, 0);
            SelectedWorld = newButton;
        }

        protected override void OnEnable()
        {
            base.OnEnable();
            //ButtonPrefab.gameObject.SetActive(false);
        }

        public WorldSelectButton SelectedWorld { get; set; }

        private void EnterSelectedWorld()
        {
            ClientCommunicator.Instance.ConnectToWorld(SelectedWorld.IpAdress);
        }
    }
}
