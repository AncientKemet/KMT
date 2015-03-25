using UnityEngine;
#if SERVER
using Server.Servers;

using System;
using System.Net.Sockets;
using Code.Libaries.UnityExtensions;
using Server.Model.Entities.Human;
using Server.Model.Extensions.PlayerExtensions;
using Server.Net;

namespace Server
{
    public class ServerConnectionManager
    {

        public AServer Server { get; set; }

        public void AcceptConnections(Socket socket)
        {
            if(Server != null)
            if(socket != null)
                socket.BeginAccept(new AsyncCallback(acceptCallback), socket);
        }

        public void acceptCallback(IAsyncResult ar)
        {
            var listener = (Socket)ar.AsyncState;
            var newConnection = listener.EndAccept(ar);

            if (newConnection != null)
            {
                ServerSingleton.StuffToRunOnUnityThread.Add(() =>
                {
                    var client = Server.GetFreeServerClient();
                    client.Initialize(newConnection);
                    client.Server = Server;
                    Server.AddClient(client);
                });
                
            }
        }

        
    }
}

#endif
