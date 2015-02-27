using System;
using System.Net;
using System.Net.Sockets;
using Code.Code.Libaries.Net;
using Libaries.Net;
using Server.Model.Entities.Human;
using Shared.NET;
using UnityEngine;

namespace Server.Model.Extensions.PlayerExtensions
{
    public class PlayerUDP : EntityExtension
    {

        public UDPConnectionHandler ConnectionHandler;

        public override void Progress()
        {
            if (ConnectionHandler == null)
            {
                Connect();
                return;
            }
            ConnectionHandler.FlushOutPackets();
        }

        public void Send(DatagramPacket packet)
        {
            if (ConnectionHandler == null)
            {
                Connect();
                return;
            }
            ConnectionHandler.SendPacket(packet);
        }

        private void Connect()
        {
            try
            {
                Socket socket = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);
                socket.Connect(IPAddress.Parse((entity as Player).Client.IpAdress),
                    NetworkConfig.I.WorldUnprecieseMovmentPort);
                ConnectionHandler = new UDPConnectionHandler(socket, NetworkConfig.I.WorldUnprecieseMovmentPort, false, true);
            }
            catch (Exception e)
            {
                Debug.LogException(e);
            }
        }

        public override void Serialize(ByteStream bytestream)
        {
        }

        public override void Deserialize(ByteStream bytestream)
        {
        }
    }
}
