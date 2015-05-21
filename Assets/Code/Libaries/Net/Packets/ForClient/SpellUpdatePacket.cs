using Code.Code.Libaries.Net;
using Code.Libaries.Generic.Managers;
using Shared.Content.Types;

namespace Libaries.Net.Packets.ForClient
{
    public class SpellUpdatePacket : BasePacket
    {
        /// <summary>
        /// Index of spell
        /// </summary>
        public int Index { get; set; }

        public bool IsEnabled { get; set; }
        public bool SetSpell { get; set; }
        public bool IsCasting { get; set; }

        /// <summary>
        /// Only used if SetSpell is True
        /// </summary>
        public Spell Spell { get; set; }

        /// <summary>
        /// Only used if IsCasting is True
        /// </summary>
        public float Strenght { get; set; }

        protected override int GetOpCode()
        {
            return 17;
        }

        protected override void enSerialize(ByteStream bytestream)
        {
            bytestream.AddByte(Index);
            bytestream.AddFlag(IsEnabled, SetSpell, IsCasting);

            if(SetSpell)
                bytestream.AddShort(Spell == null ? -1 : Spell.InContentManagerId);

            if(IsCasting)
                bytestream.AddFloat2B(Strenght);
        }

        protected override void deSerialize(ByteStream bytestream)
        {
            Index = bytestream.GetByte();
            var mask = bytestream.GetBitArray();

            IsEnabled = mask[0];
            SetSpell = mask[1];
            IsCasting = mask[2];

            if (SetSpell)
            {
                int spellID = bytestream.GetShort();
                Spell = spellID == -1 ? null : ContentManager.I.Spells[spellID];
            }

            if (IsCasting)
                Strenght = bytestream.GetFloat2B();
        }
    }
}
