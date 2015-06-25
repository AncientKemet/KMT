using Server.Model.Extensions.PlayerExtensions;
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

        public bool debug = false;
        public IDataProvider DataProvider;

        private void Awake()
        {
            scm.Get.Server = this;

            for (int i = 0; i < 5; i++)
            {
                GameObject ob = new GameObject("Free");
                ob.AddComponent<ServerClient>();
                ob.transform.parent = ServerSingleton.Instance.GOPool;
                FreeClients.Add(ob.GetComponent<ServerClient>());
            }

            DataProvider = new MySQLDataProvider();
            
            socket = CreateServerSocket(NetworkConfig.I.DataServerPort);
            Debug.Log("Data server running.");
        }


        public override void StartServer()
        {
        }

        public override void ServerUpdate(float f)
        {
            base.ServerUpdate(f);

            if (socket == null)
                socket = CreateServerSocket(NetworkConfig.I.DataServerPort);
            if (DataProvider == null)
                DataProvider = new MySQLDataProvider();
            scm.Get.AcceptConnections(socket);
                lock (Clients)
                {
                    foreach (var client in Clients)
                    {
                        client.Progress(f);
                    }
                }
        }

        public override void Stop()
        {
            foreach (var client in Clients)
            {
                client.Progress(1f);
            }
            MySQLDataProvider.ExecuteAllSQL();
            socket.Close();
            DataProvider = null;
        }
    }
}
#endif
