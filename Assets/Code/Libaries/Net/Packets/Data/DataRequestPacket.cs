using Code.Code.Libaries.Net;

namespace Libaries.Net.Packets.Data
{
    public class DataRequestPacket : DataPacket {

        public int ID { get; set; }

        protected override int GetOpCode()
        {
            return 251;
        }

        protected override void deSerialize(ByteStream bytestream)
        {
            base.deSerialize(bytestream);
            ID = bytestream.GetInt();
        }

        protected override void enSerialize(ByteStream bytestream)
        {
            base.enSerialize(bytestream);
            bytestream.AddInt(ID);
        }
    }
}
