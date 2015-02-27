#if SERVER

using Shared.NET;
using UnityEngine;

namespace Server.Servers
{
    public class MasterServer : AServer {

        /// <summary>
        /// An index of next client to be progressed.
        /// </summary>
        private int _currentIndex = 0;


        public override void StartServer()
        {
            base.StartServer();
            socket = CreateServerSocket(NetworkConfig.I.MasterServerPort);
            Debug.Log("Master server running.");
        }

        public override void ServerUpdate()
        {
            base.ServerUpdate();
            scm.Get.AcceptConnections(socket);
        }

        public override void Stop()
        {
            socket.Close();
            Debug.Log("Stopping Master server.");
        }
    }
}
#endif
