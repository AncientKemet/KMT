using Code.Code.Libaries.Net;

namespace Code.Libaries.Net.Packets.ForClient
{
    public class UnitUpdatePacket : BasePacket
    {

        public ByteStream SubPacketData = new ByteStream();

        public int UnitID { get; set; }

        #region implemented abstract members of BasePacket

        protected override int GetOpCode()
        {
            return 20;
        }

        protected override void enSerialize(ByteStream bytestream)
        {
            bytestream.AddShort(UnitID);
            bytestream.AddShort(SubPacketData.Length);
            bytestream.AddBytes(SubPacketData.GetBuffer());
        }

        protected override void deSerialize(ByteStream bytestream)
        {
            UnitID = bytestream.GetUnsignedShort();
            int lenght = bytestream.GetUnsignedShort();
            SubPacketData.AddBytes(bytestream.GetSubBuffer(lenght));
        }

        #endregion

        public override string ToString()
        {
            return "Update packet subbuffer[" + SubPacketData.GetSize() + "]";
        }
    }
}

