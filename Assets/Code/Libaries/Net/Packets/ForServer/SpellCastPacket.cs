using System;
using Code.Code.Libaries.Net;
using UnityEngine;
using UnityEngine.UI;

namespace Libaries.Net.Packets.ForServer
{
    public class SpellCastPacket : BasePacket
    {
        public enum CastAction
        {
            StartCasting,
            FinishCasting
        }

        public int buttonIndex { get; set; }
        public Vector3 TargetPosition { get; set; }
        public CastAction Action;

        protected override int GetOpCode()
        {
            return 82;
        }

        protected override void enSerialize(ByteStream bytestream)
        {
            bytestream.AddByte(buttonIndex);
            bytestream.AddByte((int) Action);
            bytestream.AddPosition12B(TargetPosition);
        }

        protected override void deSerialize(ByteStream bytestream)
        {
            buttonIndex = bytestream.GetUnsignedByte();
            Action = (CastAction) bytestream.GetByte();
            TargetPosition = bytestream.GetPosition12B();
        }
    }
}
