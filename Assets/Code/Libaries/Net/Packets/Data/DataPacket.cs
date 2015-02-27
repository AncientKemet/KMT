using Code.Code.Libaries.Net;

namespace Libaries.Net.Packets.Data
{
    public abstract class DataPacket : BasePacket {

        public string Certificate { get; set; }

        public string DataPath { get; set; }

        protected override void enSerialize(ByteStream bytestream)
        {
            bytestream.AddString(Certificate);
            bytestream.AddString(DataPath);
        }

        protected override void deSerialize(ByteStream bytestream)
        {
            Certificate = bytestream.GetString();
            DataPath = bytestream.GetString();
        }
    }
}
