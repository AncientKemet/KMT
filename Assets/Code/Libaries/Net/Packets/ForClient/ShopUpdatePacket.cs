using Code.Code.Libaries.Net;
using Code.Libaries.Generic.Managers;
using Shared.Content.Types;

namespace Libaries.Net.Packets.ForClient
{
    public class ShopUpdatePacket : BasePacket
    {

        public Item.ItemInstance Instance;
        public int Index;
        public int UnitId;

        protected override int GetOpCode()
        {
            return 56;
        }

        protected override void enSerialize(ByteStream bytestream)
        {
            bytestream.AddShort(UnitId);
            bytestream.AddShort(Instance == null ? -1 : Instance.Item.InContentManagerIndex);
            bytestream.AddShort(Instance == null ? 0 : Instance.Amount);
            bytestream.AddShort(Index);
        }

        protected override void deSerialize(ByteStream bytestream)
        {
            UnitId = bytestream.GetUnsignedShort();
            int itemId = bytestream.GetShort();
            int amount = bytestream.GetUnsignedShort();
            if (itemId == -1)
                Instance = null;
            else
                Instance = new Item.ItemInstance(ContentManager.I.Items[itemId], amount);
            Index = bytestream.GetUnsignedShort();
        }
    }
}
