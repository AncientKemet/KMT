using Code.Code.Libaries.Net;

namespace Libaries.Net.Packets.Data
{
    public class DataReplacePacket : DataPacket
    {

        public string OldValue { get; set; }
        public string NewValue { get; set; }

        protected override int GetOpCode()
        {
            return 254;
        }

        protected override void enSerialize(ByteStream bytestream)
        {
            base.enSerialize(bytestream);
            bytestream.AddString(OldValue);
            bytestream.AddString(NewValue);
        }

        protected override void deSerialize(ByteStream bytestream)
        {
            base.deSerialize(bytestream);
            OldValue = bytestream.GetString();
            NewValue = bytestream.GetString();
        }
    }
}
