using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using Client.UI.Interfaces.Lobby;
using Code.Code.Libaries.Net;
using Code.Libaries.Generic;
using Code.Libaries.Generic.Managers;
using Libaries.Net;
using Libaries.Net.Packets.Data;
using Libaries.Net.Packets.ForServer;
using Server;
using Shared.NET;
using UnityEngine;

namespace Client.Net
{
    [ExecuteInEditMode]
    public class ClientCommunicator : MonoSingleton<ClientCommunicator>
    {
        [SerializeField]
        private string LoginServerIP = "127.0.0.1";
        [SerializeField]
        private string DataServerIP = "127.0.0.1";

        private Socket loginSocket;

        public string LastUsernameUsedToLogin { get; set; }

        public ConnectionHandler LoginServerConnection;
        public DataServerConnection DataServerConnection;
        public ConnectionHandler WorldServerConnection;
        public UDPConnectionHandler WorldServerUnprecieseMovementConnection;

        public bool debug = false;
        private float _lastDebug = 0f;

        public int LoginServerInBytes = 0;
        public int DataServerInBytes = 0;
        public int WorldServerInBytes = 0;
        public int LoginServerInBytesS = 0;
        public int DataServerInBytesS = 0;
        public int WorldServerInBytesS = 0;

        public string[] opcodes = new string[256];

        void Start()
        {
            if(Application.isPlaying)
            try
            {
                
                if (NetworkConfig.I != null)
                {
                    int LoginServerPort = NetworkConfig.I.LoginServerPort;
                    int DataServerPort = NetworkConfig.I.DataServerPort;

                    Thread thread = new Thread(() =>
                        {
                            //Create login server connection
                            loginSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
                            loginSocket.Connect(IPAddress.Parse(LoginServerIP), LoginServerPort);
                            LoginServerConnection = new ConnectionHandler(loginSocket, new PlayerPacketExecutor());

                            //Create data server connection
                            Socket dataSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
                            dataSocket.Connect(IPAddress.Parse(DataServerIP), DataServerPort);
                            DataServerConnection = new DataServerConnection(dataSocket, new DataServerConnection.DataPacketExecutor());

                            if (loginSocket.Connected)
                            {
                            }
                            else
                            {
                                Debug.LogError("Couldnt connect to server.");
                            }

                        });
                    thread.Start();
                }
                else
                {
                    Debug.LogError("Null network config, can't know the IP adresses.");
                }



            }
            catch (Exception e)
            {
                Debug.LogException(e);
            }
        }

        private void FixedUpdate()
        {
            if (Application.isPlaying)
            {
                if (LoginServerConnection != null)
                {
                    LoginServerConnection.ReadAndExecute();
                    LoginServerConnection.FlushOutPackets();
                    LoginServerInBytes += (int) LoginServerConnection.BytesRecieved;
                    LoginServerInBytesS = (int) (LoginServerConnection.BytesRecieved*(1.0f/Time.fixedDeltaTime));
                    LoginServerConnection.BytesRecieved = 0;
                }
                if (DataServerConnection != null)
                {
                    DataServerConnection.ReadAndExecute();
                    DataServerConnection.FlushOutPackets();
                    DataServerInBytes += (int) DataServerConnection.BytesRecieved;
                    DataServerInBytesS = (int) (DataServerConnection.BytesRecieved*(1.0f/Time.fixedDeltaTime));
                    DataServerConnection.BytesRecieved = 0;
                }
                if (WorldServerConnection != null)
                {
                    WorldServerConnection.ReadAndExecute();
                    WorldServerConnection.FlushOutPackets();
                    WorldServerUnprecieseMovementConnection.ReadAndExecute();
                    WorldServerInBytes += (int) WorldServerConnection.BytesRecieved +
                                          (int) WorldServerUnprecieseMovementConnection.BytesRecieved;
                    WorldServerInBytesS =
                        (int)
                            ((WorldServerConnection.BytesRecieved +
                              WorldServerUnprecieseMovementConnection.BytesRecieved)*(1.0f/Time.fixedDeltaTime));
                    WorldServerConnection.BytesRecieved = 0;
                    WorldServerUnprecieseMovementConnection.BytesRecieved = 0;
                }
                if (debug)
                {
                    if (WorldServerConnection != null)
                        if (_lastDebug < Time.time - 1f)
                        {
                            _lastDebug = Time.time;
                            string s = "";
                            foreach (var packet in WorldServerConnection.PacketExecutor.ExtecutedPackets)
                            {
                                s += packet.Key.Name + " = " + packet.Value + "\n";
                            }
                            Debug.Log(s);
                        }
                    if (LoginServerConnection != null)
                        if (_lastDebug < Time.time - 1f)
                        {
                            _lastDebug = Time.time;
                            string s = "";
                            foreach (var packet in LoginServerConnection.PacketExecutor.ExtecutedPackets)
                            {
                                s += packet.Key.Name + " = " + packet.Value + "\n";
                            }
                            Debug.Log(s);
                        }

                }
            }
        }
        void Update()
        {
            if (!Application.isPlaying)
            {
                //Fill inspector opcode names
                if (string.IsNullOrEmpty(opcodes[1]))
                {
                    PacketManager.PacketForOpcode(1);

                    foreach (KeyValuePair<int, Type> pair in PacketManager.TCPpacketTypes)
                    {
                        opcodes[pair.Key] = pair.Value.Name;
                    }
                }
            }
        }

        public void SendToServer(BasePacket packet)
        {
            if (WorldServerConnection != null && packet != null)
            {
                WorldServerConnection.SendPacket(packet);
            }
            else if (LoginServerConnection != null && packet != null)
            {
                LoginServerConnection.SendPacket(packet);
            }
            else
            {
                Debug.LogError("Invalid connection");
            }
        }

        public void ConnectToWorld(string ipAdress)
        {
            Debug.Log("Connecting to world: "+ipAdress);
            //creating main connection
            {
                Socket socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
                socket.Connect(IPAddress.Parse(ipAdress), NetworkConfig.I.WorldServerPort);
                WorldServerConnection = new ConnectionHandler(socket, new PlayerPacketExecutor());

                SecuredDataPacket p = new SecuredDataPacket { DataKey = LobbyInterface.I.DataServerKey };

                WorldServerConnection.SendPacket(p);
            }

            //creating unpreciese connection
            {
                Socket socket = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);
                socket.Connect(IPAddress.Parse(ipAdress), NetworkConfig.I.WorldUnprecieseMovmentPort);
                WorldServerUnprecieseMovementConnection = new UDPConnectionHandler(socket, NetworkConfig.I.WorldUnprecieseMovmentPort, true, false);
            }

            
        }
    }
}
