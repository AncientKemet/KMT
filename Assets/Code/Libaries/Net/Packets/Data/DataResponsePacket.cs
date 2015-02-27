using Code.Code.Libaries.Net;

namespace Libaries.Net.Packets.Data
{
    public class DataResponsePacket : DataPacket {

        public string Value { get; set; }
        public bool Success { get; set; }
        public int ID { get; set; }

        protected override int GetOpCode()
        {
            return 250;
        }

        protected override void enSerialize(ByteStream bytestream)
        {
            base.enSerialize(bytestream);
            bytestream.AddByte(Success ? 1 : 0);
            bytestream.AddString(Value);
            bytestream.AddInt(ID);
        }

        protected override void deSerialize(ByteStream bytestream)
        {
            base.deSerialize(bytestream);
            Success = bytestream.GetByte() == 1;
            Value = bytestream.GetString();
            ID = bytestream.GetInt();
        }
    }
}
