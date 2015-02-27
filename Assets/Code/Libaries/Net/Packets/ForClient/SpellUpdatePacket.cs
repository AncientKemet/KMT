using Code.Code.Libaries.Net;
using Code.Libaries.Generic.Managers;
using Shared.Content.Types;

namespace Libaries.Net.Packets.ForClient
{
    public class SpellUpdatePacket : BasePacket
    {

        public int Index { get; set; }
        public Spell Spell { get; set; }
        public bool IsEnabled { get; set; }

        protected override int GetOpCode()
        {
            return 17;
        }

        protected override void enSerialize(ByteStream bytestream)
        {
            bytestream.AddByte(Index);
            bytestream.AddShort(Spell == null ? -1 : Spell.InContentManagerId);
            bytestream.AddFlag(IsEnabled);
        }

        protected override void deSerialize(ByteStream bytestream)
        {
            Index = bytestream.GetByte();
            int spellID = bytestream.GetShort();
            Spell = spellID == -1 ? null : ContentManager.I.Spells[spellID];
            var mask = bytestream.GetBitArray();
            IsEnabled = mask[0];
        }
    }
}
