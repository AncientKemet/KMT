using Code.Code.Libaries.Net;

namespace Code.Libaries.Net.Packets.InGame
{
    public class TargetUpdatePacket : BasePacket 
    {

        public int UnitId { get; set; }

        protected override int GetOpCode()
        {
            return 22;
        }

        protected override void enSerialize(ByteStream bytestream)
        {
            bytestream.AddShort(UnitId);
        }

        protected override void deSerialize(ByteStream bytestream)
        {
            UnitId = bytestream.GetUnsignedShort();
        }
    }
}
