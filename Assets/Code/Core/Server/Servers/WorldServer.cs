#if SERVER

using Libaries.IO;
using Server.Model;
using Server.Model.Extensions.PlayerExtensions;
using Shared.NET;
using UnityEngine;

namespace Server.Servers
{
    public class WorldServer : AServer
    {
        [SerializeField]
        private World _world;

        [SerializeField]
        private string _ipAdress;
        
        public string IpAdress
        {
            get { return _ipAdress; }
            private set { _ipAdress = value; }
        }

        public World World
        {
            get { return _world; }
            private set { _world = value; }
        }
        
        public override void StartServer()
        {
            base.StartServer();
            socket = CreateServerSocket(NetworkConfig.I.WorldServerPort);
            AddServerToPublicList();
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

        public override void AddClient(ServerClient client)
        {
            base.AddClient(client);
        }

        private void RemoveServerFromPublicList()
        {
            DataServerConnection.ReplaceData("lobby/PlayPage/Data", "$" + GetServerInfo(), "");
        }
        
        private void AddServerToPublicList()
        {
            DataServerConnection.AppendData("lobby/PlayPage/Data", "$" + GetServerInfo());
        }

        private JSONObject GetServerInfo()
        {
            JSONObject o = new JSONObject();

            o.AddField("name", "beta world");
            o.AddField("type", "Beta testing");
            o.AddField("online", ""+World.Players.Count);
            o.AddField("IP", IpAdress);

            return o;
        }
    }
 }
#endif
