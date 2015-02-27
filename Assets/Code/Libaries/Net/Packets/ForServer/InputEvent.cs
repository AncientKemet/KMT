
using Code.Code.Libaries.Net;
using Code.Core.Shared.NET;
using UnityEngine;

namespace Code.Libaries.Net.Packets.ForServer
{
    public class InputEventPacket : BasePacket
    {
        public PacketEnums.INPUT_TYPES type;

        public InputEventPacket() { }

        public InputEventPacket(PacketEnums.INPUT_TYPES _type)
        {
            this.type = _type;
        }

        #region implemented abstract members of BasePacket

        protected override int GetOpCode()
        {
            return 81;
        }

        protected override void enSerialize(ByteStream bytestream)
        {
            bytestream.AddByte((int) type);
        }

        protected override void deSerialize(ByteStream bytestream)
        {
            type = (PacketEnums.INPUT_TYPES) bytestream.GetUnsignedByte();
        }

        #endregion
    }

}

