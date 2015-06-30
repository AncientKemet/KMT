using System;
using System.Net;
using System.Net.Sockets;
using Server.Model.Entities;
#if SERVER
using Libaries.IO;
using Server.Model;
using Shared.NET;
using UnityEngine;

namespace Server.Servers
{
    public class WorldServer : AServer
    {
        [SerializeField]
        private World _world;
        
        public World World
        {
            get { return _world; }
            private set { _world = value; }
        }

        public override void StartServer()
        {
            base.StartServer();
            socket = CreateServerSocket(NetworkConfig.I.WorldServerPort);
            RemoveServerFromPublicList();
            StartCoroutine(SEase.Action(AddServerToPublicList

    , -1, 3f));
            Debug.Log("WorldServer running.");
        }
        
        public override void ServerUpdate(float f)
        {
            base.ServerUpdate(f);
            World.Progress(f);
            scm.Get.AcceptConnections(socket);
            foreach (var client in Clients)
            {
                client.Progress(f);
            }
        }

        public override void Stop()
        {
            socket.Close();
            World.Save();
            RemoveServerFromPublicList();
            Debug.Log("Stopping WorldServer server.");
        }

        private void RemoveServerFromPublicList()
        {
            DataServerConnection.ReplaceData("lobby/PlayPage/Data", "$" + GetServerInfo(), "");
            Debug.Log("WorldServer Cleared.");
        }
        
        private void AddServerToPublicList()
        {
            DataServerConnection.AppendData("lobby/PlayPage/Data", "$" + GetServerInfo());
            Debug.Log("WorldServer Added.");
        }

        private JSONObject GetServerInfo()
        {
            JSONObject o = new JSONObject();

            o.AddField("name", "w" + LocalIPAddress());
            o.AddField("type", "Beta testing");
            o.AddField("online", ""+World.Players.Count);
            o.AddField("IP", LocalIPAddress());

            return o;
        }
    }
 }
#endif
