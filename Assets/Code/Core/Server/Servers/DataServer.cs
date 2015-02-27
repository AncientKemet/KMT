#if SERVER
using Server.Servers;

using Server.IO;
using Server.IO.File;
using Shared.NET;
using UnityEngine;

namespace Server
{
    public class DataServer : AServer
    {

        public IDataProvider DataProvider;

        private float _updateRate = 0.5f;

        public override void StartServer()
        {
            base.StartServer();
            DataProvider = new FileDataProvider();

            DataProvider.SetData("lobby/PlayPage/Data", "", b => {});

            socket = CreateServerSocket(NetworkConfig.I.DataServerPort);
            Debug.Log("Data server running.");
        }

        public override void ServerUpdate()
        {
            base.ServerUpdate();
            
            _updateRate -= Time.deltaTime;

            if (_updateRate <= 0)
            {
                scm.Get.AcceptConnections(socket);
                lock (Clients)
                {
                    foreach (var client in Clients)
                    {
                        client.Progress();
                    }
                }
                _updateRate = 0.5f;
            }
        }

        public override void Stop()
        {
            socket.Close();
            Debug.Log("Stopping Data server.");
        }
    }
}
#endif
