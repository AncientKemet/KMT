using System;
using Code.Code.Libaries.Net;
using Code.Libaries.Generic.Managers;
using Shared.Content.Types;

namespace Libaries.Net.Packets.ForServer
{
    public class CraftingPacket : BasePacket
    {
        public ItemRecipe ItemRecipe { get; set; }
        public ushort Amount { get; set; }

        protected override int GetOpCode()
        {
            return 78;
        }

        protected override void enSerialize(ByteStream bytestream)
        {
            bytestream.AddShort(ItemRecipe.InContentManagerIndex);
            bytestream.AddShort(Amount);
        }

        protected override void deSerialize(ByteStream bytestream)
        {
            ItemRecipe = ContentManager.I.Recipes[bytestream.GetUnsignedShort()];
            Amount = bytestream.GetUnsignedShort();
        }
    }
}
