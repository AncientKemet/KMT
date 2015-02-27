using Client.Net;
using Libaries.Net.Packets.Data;
using UnityEngine;

namespace Client.UI.Interfaces.Lobby
{
    public abstract class LobbyPage : MonoBehaviour 
    {
        public string Data { get; set; }

        protected virtual void OnEnable()
        {
            RequestPageData();
        }

        protected abstract void OnDataWereLoaded();

        private void RequestPageData()
        {
            DataServerConnection.DataRequest dataRequest = new DataServerConnection.DataRequest("lobby/"+name+"/Data",
                (b, s) =>
                {
                    if (b)
                    {
                        Data = s;
                    }
                    OnDataWereLoaded();
                });
            ClientCommunicator.Instance.DataServerConnection.RequestData(dataRequest);
        }
    }
}
