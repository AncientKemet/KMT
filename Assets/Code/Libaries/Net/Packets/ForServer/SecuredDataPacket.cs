using Code.Code.Libaries.Net;

namespace Libaries.Net.Packets.ForServer
{
    /// <summary>
    /// This packet is used when user selects an world to connect.
    /// It sends the user datakey to the world server, which then loads the server unit for him, responding with enter world packet.
    /// </summary>
    public class SecuredDataPacket : BasePacket {

        public string DataKey { get; set; }

        protected override int GetOpCode()
        {
            return 79;
        }

        protected override void enSerialize(ByteStream bytestream)
        {
            bytestream.AddString(DataKey);
        }

        protected override void deSerialize(ByteStream bytestream)
        {
            DataKey = bytestream.GetString();
        }
    }
}
