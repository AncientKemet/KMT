using Code.Code.Libaries.Net;
using Code.Libaries.Generic.Managers;
using Shared.Content.Types;

namespace Libaries.Net.Packets.ForClient
{

    public enum SpellUpdateState
    {
        Enable,
        Disable,
        SetSpell,
        StartedCasting,
        FinishedCasting,
        StrenghtChange
    }

    public class SpellUpdatePacket : BasePacket
    {
        public int Index { get; set; }
        public Spell Spell { get; set; }
        public float Strenght { get; set; }

        public SpellUpdateState UpdateState { get; set; }

        protected override int GetOpCode()
        {
            return 17;
        }

        protected override void enSerialize(ByteStream bytestream)
        {
            bytestream.AddByte(Index);
            bytestream.AddByte((int) UpdateState);

            if (UpdateState == SpellUpdateState.SetSpell)
                bytestream.AddShort(Spell == null ? -1 : Spell.InContentManagerId);

            if (UpdateState == SpellUpdateState.StrenghtChange)
                bytestream.AddFloat2B(Strenght);
        }

        protected override void deSerialize(ByteStream bytestream)
        {
            Index = bytestream.GetByte();
            UpdateState = (SpellUpdateState) bytestream.GetUnsignedByte();

            if (UpdateState == SpellUpdateState.SetSpell)
            {
                int spellID = bytestream.GetShort();
                Spell = spellID == -1 ? null : ContentManager.I.Spells[spellID];
            }

            if (UpdateState == SpellUpdateState.StrenghtChange)
                Strenght = bytestream.GetFloat2B();
        }
    }
}
