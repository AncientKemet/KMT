using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using Code.Code.Libaries.Net;
using UnityEngine;

namespace Libaries.Net
{
    public class UDPConnectionHandler
    {
        private const int MAX_PACKETS_PROCEED_AT_ONCE = 500;

        private Socket socket;
        private List<DatagramPacket> outgoingPackets = new List<DatagramPacket>();

        private long bytesRecieved = 0;
        private int port = -1;
        private int _packetSize = -1;

        private IPAddress broadcast;
        private IPEndPoint ep;
        private UdpClient listener;
        private IPEndPoint groupEP;

        ByteStream _in = new ByteStream();

        public UDPConnectionHandler(Socket socket, int Port, bool recieve, bool send)
        {
            this.socket = socket;
            this.port = Port;
            if (send)
            {
                broadcast = (socket.RemoteEndPoint as IPEndPoint).Address;
                ep = new IPEndPoint(broadcast, port);
            }
            if (recieve)
            {
                listener = new UdpClient(port);
                groupEP = new IPEndPoint(IPAddress.Any, port);
            }
            DatagramPacket packet = PacketManager.UDPPacketForPort(port);
            _packetSize = packet.Size;
        }

        public long BytesRecieved
        {
            get { return bytesRecieved; }
            set { bytesRecieved = value; }
        }

        public void ReadAndExecute()
        {
            int available = listener.Available;
            if (available > 0)
            {
                byte[] bytes = listener.Receive(ref groupEP);

                BytesRecieved += bytes.Length;
                _in.AddBytes(bytes);
                int amoutOfPacketsExecuted = 0;

                for (int i = 0; _in.Offset + _packetSize <= _in.Length; i++)
                {
                    DatagramPacket packet = PacketManager.UDPPacketForPort(port);
                    packet.Deserialize(_in);
                    packet.Execute();
                    amoutOfPacketsExecuted++;
                }

                _in.Offset = 0;
                _in.RemoveFirstBytes(_packetSize*amoutOfPacketsExecuted);
            }
        }

        public void FlushOutPackets()
        {
            if (outgoingPackets.Count > 0)
            {
                ByteStream bytestream = new ByteStream(outgoingPackets.Count * _packetSize);

                foreach (var packet in outgoingPackets)
                {
                    packet.Serialize(bytestream);
                }

                //send
                socket.SendTo(bytestream.GetBuffer(), ep);

                outgoingPackets.Clear();
            }
        }

        public void SendPacket(DatagramPacket packet)
        {
            outgoingPackets.Add(packet);
        }

        public void Disconnect()
        {
            socket.Disconnect(false);
        }
    }
}
