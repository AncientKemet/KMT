using Code.Code.Libaries.Net;
using Shared.Content.Types;

namespace Libaries.Net.Packets.ForClient
{
    public class BuffUpdatePacket : BasePacket
    {
        public int UnitId { get; set; }
        public bool AddOrRemove { get; set; }
        public BuffInstance BuffInstance { get; set; }

        protected override int GetOpCode()
        {
            return 18;
        }

        protected override void enSerialize(ByteStream bytestream)
        {
            bytestream.AddShort(UnitId);
            bytestream.AddByte(AddOrRemove == true ? 1 : 0);

            BuffInstance.Serialize(bytestream);
        }

        protected override void deSerialize(ByteStream bytestream)
        {
            UnitId = bytestream.GetUnsignedShort();
            AddOrRemove = bytestream.GetByte() == 1;

            BuffInstance = new BuffInstance(0,0,null,0);
            BuffInstance.Deserialize(bytestream);
        }
    }
}
