using System.Collections.Generic;
using System.Net.Sockets;
using Code.Code.Libaries.Net;

namespace Libaries.Net
{
    public class ConnectionHandler
    {
        private const int MAX_PACKETS_PROCEED_AT_ONCE = 500;

        private Socket socket;
        private PacketExecutor packetExecutor;
        private List<BasePacket> outgoingPackets = new List<BasePacket>();
        private int expectedNextLength = -1;

        private long bytesRecieved = 0;

        public ConnectionHandler(Socket socket, PacketExecutor packetExecutor) {
            this.socket = socket;
            this.packetExecutor = packetExecutor;
        }

        public long BytesRecieved
        {
            get { return bytesRecieved; }
            set { bytesRecieved = value; }
        }

        public PacketExecutor PacketExecutor
        {
            get { return packetExecutor; }
        }

        public Socket Socket
        {
            get { return socket; }
        }

        public void ReadAndExecute()
        {
            if (!socket.Connected)
            {
                return;
            }

            if (socket.Available > 0)
                
            for (int i = 0; i < MAX_PACKETS_PROCEED_AT_ONCE; i++)
            {
                int available = socket.Available;
#if DEBUG_NETWORK
                Debug.Log("AWAILABLE BYTES: "+available);
#endif

                if (available > 0)
                {
                    if(available > 2)
                    {
                        if (expectedNextLength == -1)
                        {
                            //ignore the two bytes for the future
                            available -= 2;

                            byte[] bytes = new byte[2];

                            BytesRecieved += socket.Receive(bytes, bytes.Length, 0);

                            ByteStream _in = new ByteStream(bytes);

                            _in.Offset = 0;

                            expectedNextLength = _in.GetUnsignedShort();
                        }
                    }
                    if (expectedNextLength != -1)
                    {
                        if (available >= expectedNextLength)
                        {
                            //ignore the expected lenght for the future
                            available -= expectedNextLength;

                            byte[] bytes = new byte[expectedNextLength];

                            BytesRecieved += socket.Receive(bytes, expectedNextLength, 0);

                            ByteStream _in = new ByteStream(bytes);
                            _in.Offset = 0;
                            int opcode = _in.GetUnsignedByte();

                            BasePacket packet = PacketManager.PacketForOpcode(opcode);

                            packet.Size = expectedNextLength;

                            packet.Deserialize(_in);

                            PacketExecutor.ExecutePacket(packet);

                            expectedNextLength = -1;
                        }
                    }
                }
            }
        }

        public void FlushOutPackets()
        {
            if (outgoingPackets.Count > 0)
            {
                
                foreach (var packet in outgoingPackets)
                {
                    ByteStream bytestream = new ByteStream();
                    packet.Serialize(bytestream);//send
                    socket.Send(bytestream.GetBuffer());
                }
                
                outgoingPackets.Clear();
            }
        }

        public void SendPacket(BasePacket packet)
        {
            outgoingPackets.Add(packet);
        }

        public void Disconnect()
        {
            socket.Disconnect(false);
        }
    }
}
