using System;
using Code.Code.Libaries.Net;
using Shared.Content.Types;

namespace Libaries.Net.Packets.ForClient
{
    public class DamagePacket : BasePacket
    {

        protected override int GetOpCode()
        {
            return 16;
        }

        protected override void enSerialize(ByteStream b)
        {
            b.AddShort(UnitId);
            b.AddByte((int) DamageType);
            b.AddByte((int) HitType);
            b.AddByte((int) Strenght);
            b.AddShort((int) Damage);
        }

        protected override void deSerialize(ByteStream b)
        {
            UnitId = b.GetUnsignedShort();
            DamageType = (Spell.DamageType) b.GetUnsignedByte();
            HitType = (Spell.HitType) b.GetUnsignedByte();
            Strenght = (Spell.HitStrenght) b.GetUnsignedByte();
            Damage = b.GetUnsignedShort();
        }

        public int UnitId { get; set; }
        public Spell.DamageType DamageType { get; set; }
        public Spell.HitType HitType { get; set; }
        public float Damage { get; set; }
        public Spell.HitStrenght Strenght { get; set; }
    }
}
