#if SERVER
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using Code.Libaries.Generic;
using Libaries.Net.Packets.Data;
using Server.Model.Extensions.PlayerExtensions;
using Shared.NET;


namespace Server.Servers
{
    public abstract class AServer
    {

        protected Socket socket;
        protected List<ServerClient> Clients = new List<ServerClient>();

        public GenProperty<ServerConnectionManager> scm = new GenProperty<ServerConnectionManager>();

        public DataServerConnection DataServerConnection;

        protected Socket CreateServerSocket(int port)
        {
            Socket newSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            newSocket.Blocking = false;
            newSocket.Bind(new IPEndPoint(IPAddress.Parse("192.168.1.6"), port));
            newSocket.Listen(10);
            return newSocket;
        }

        public virtual void StartServer()
        {
            //data server doesnt need an connection to itself.
            if (!(this is DataServer))
            {
                //Create dataserver connection
                socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
                socket.Connect(IPAddress.Parse(ServerSingleton.Instance.DataServerIPAdress),
                    NetworkConfig.I.DataServerPort);
                DataServerConnection = new DataServerConnection(socket, new DataServerConnection.DataPacketExecutor());
            }

            scm.Get.Server = this;
        }

        public abstract void Stop();

        public virtual void ServerUpdate()
        {
            //data server doesnt need an connection to itself.
            if (!(this is DataServer))
            {
                DataServerConnection.ReadAndExecute();
                DataServerConnection.FlushOutPackets();
            }
        }

        public virtual void AddClient(ServerClient client)
        {
            lock (Clients)
            {
                Clients.Add(client);
            }
        }
    }
}
#endif
