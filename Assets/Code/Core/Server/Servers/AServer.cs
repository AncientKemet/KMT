using UnityEngine;
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
    public abstract class AServer : MonoBehaviour
    {
        public bool LimitProgressing = true;
        public int ProgressRate = 1;
        protected Socket socket;
        protected List<ServerClient> Clients = new List<ServerClient>();
        protected List<ServerClient> FreeClients = new List<ServerClient>();

        private float _progressCounter = 0;
        [SerializeField]
        private int _updatesCounter = 0;

        public GenProperty<ServerConnectionManager> scm = new GenProperty<ServerConnectionManager>();

        public DataServerConnection DataServerConnection;

        protected Socket CreateServerSocket(int port)
        {
            Socket newSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            newSocket.Blocking = false;
			newSocket.Bind(new IPEndPoint(IPAddress.Parse("127.0.0.1"), port));
            newSocket.Listen(10);
            return newSocket;
        }

        private void FixedUpdate()
        {
            if (LimitProgressing)
            {
                _progressCounter += Time.fixedDeltaTime;
                if (1f/ProgressRate < _progressCounter)
                {
                    for (int i = 0; i < (int) (_progressCounter/(1f/ProgressRate)); i++)
                    {
                        ServerUpdate(1f/ProgressRate);
                        _updatesCounter++;
                        _progressCounter -= 1f/ProgressRate;
                    }
                }
            }
            else
            {
                ServerUpdate(Time.fixedDeltaTime);
            }
        }

        private void OnEnable()
        {
            StartServer();
        }

        private void OnDisable()
        {
            Stop();
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

            for (int i = 0; i < 5; i++)
            {
                GameObject ob = new GameObject("Free");
                ob.AddComponent<ServerClient>();
                ob.transform.parent = ServerSingleton.Instance.GOPool;
                FreeClients.Add(ob.GetComponent<ServerClient>());
            }
        }

        public abstract void Stop();

        public virtual void ServerUpdate(float f)
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

        public ServerClient GetFreeServerClient()
        {
            var o = FreeClients[0];
            FreeClients.Remove(o);
            if(FreeClients.Count < 2)
            for (int i = 0; i < 5; i++)
            {
                GameObject ob = new GameObject("Free");
                ob.AddComponent<ServerClient>();
                ob.transform.parent = ServerSingleton.Instance.GOPool;
                FreeClients.Add(ob.GetComponent<ServerClient>());
            }
            return o;
        }
    }
}
#endif
