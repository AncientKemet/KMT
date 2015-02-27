using Code.Code.Libaries.Net;

namespace Libaries.Net.Packets.ForServer
{
    public class CharacterChangePacket : BasePacket {

        public enum CharAction
        {
            HairType,
            HairColor,
            FaceType,
            FaceColor,
            SkinType,
            SkinColor,
            UnderwearType,
            UnderwearColor,
            Gender
        }

        public CharAction Action;
        public int value;

        protected override int GetOpCode()
        {
            return 55;
        }

        protected override void enSerialize(ByteStream bytestream)
        {
            bytestream.AddByte((int) Action);
            bytestream.AddByte(value);
        }

        protected override void deSerialize(ByteStream bytestream)
        {
            Action = (CharAction) bytestream.GetUnsignedByte();
            value = bytestream.GetUnsignedByte();
        }
    }
}
