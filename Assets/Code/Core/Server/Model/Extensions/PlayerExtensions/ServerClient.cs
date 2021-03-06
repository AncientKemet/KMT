using System.Net;
using Libaries.Net;
using UnityEngine;
#if SERVER
using Server.Servers;

using System;
using Server.Model.ContentHandling;
using Server.Model.Extensions.PlayerExtensions.UIHelpers;

using System.Net.Sockets;
using Server.Model.Entities.Human;
using Server.Net;

namespace Server.Model.Extensions.PlayerExtensions
{
    public class ServerClient : EntityExtension
    {

        private Socket socket;

        private Player player;

        private ConnectionHandler connectionHandler;

        public ConnectionHandler ConnectionHandler { get { return connectionHandler; } }

        public AServer Server { get; set; }

        public Action OnDisconnect;

        public Player Player
        {
            get
            {
                return player;
            }
            set
            {
                player = value;
            }
        }

        public bool Authenticated = false;

        public ClientUI UI;

        public UserAccount UserAccount { get; set; }
        public UserChat UserChat { get; set; }
        public string IpAdress { get; private set; }
        
        public void Initialize(Socket _socket)
        {
            this.socket = _socket;
            IpAdress = (socket.RemoteEndPoint as IPEndPoint).Address.ToString();
            connectionHandler = new ConnectionHandler(socket, new ServerClientPacketExecutor(this));
            name = "C " + IpAdress;
        }

        public override void Progress(float time)
        {
            try
            {
                connectionHandler.ReadAndExecute();
                connectionHandler.FlushOutPackets();
            }
            catch (Exception e)
            {
                Debug.LogException(e);
                Disconnect();
            }
        }

        public void Disconnect()
        {
            socket.Close();
            if (OnDisconnect != null)
                OnDisconnect();
        }

        protected override void OnExtensionWasAdded()
        {
        }
    }
}
#endif
