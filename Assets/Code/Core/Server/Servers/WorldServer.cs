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
        public string IpAdress { get; private set; }

        public World World { get; private set; }

        public WorldServer(int worldId)
        {
            World = ServerSingleton.Instance.GetComponent<World>();

            //WebClient webClient = new WebClient();
            //IpAdress = webClient.DownloadString("http://ifconfig.me/ip");
            IpAdress = "192.168.1.6";
        }

        public override void StartServer()
        {
            base.StartServer();
            socket = CreateServerSocket(NetworkConfig.I.WorldServerPort);
            AddServerToPublicList();
            Debug.Log("WorldServer running.");
        }

        public override void ServerUpdate()
        {
            base.ServerUpdate();
            World.Progress();
            scm.Get.AcceptConnections(socket);
            foreach (var client in Clients)
            {
                client.Progress();
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
