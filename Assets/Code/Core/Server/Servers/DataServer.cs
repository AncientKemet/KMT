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


        public override void StartServer()
        {
            base.StartServer();
            DataProvider = new MySQLDataProvider();
            
            socket = CreateServerSocket(NetworkConfig.I.DataServerPort);
            Debug.Log("Data server running.");
        }

        public override void ServerUpdate()
        {
            base.ServerUpdate();
            
                scm.Get.AcceptConnections(socket);
                lock (Clients)
                {
                    foreach (var client in Clients)
                    {
                        client.Progress();
                    }
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
