//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Runtime Version:4.0.30319.18449
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace Code.Code.Libaries.Net.Packets
{
    public class EnterWorldPacket : BasePacket
    {
        public string worldId = "1";
        public ushort myUnitID;
        public UnityEngine.Vector3 Position;

        #region implemented abstract members of BasePacket

        protected override int GetOpCode()
        {
            return 19;
        }

        protected override void enSerialize(ByteStream bytestream)
        {
            bytestream.AddString(worldId);
            bytestream.AddShort(myUnitID);
            bytestream.AddPosition6B(Position);
        }

        protected override void deSerialize(ByteStream bytestream)
        {
            worldId = bytestream.GetString();
            myUnitID = bytestream.GetUnsignedShort();
            Position = bytestream.GetPosition6B();
        }

        #endregion
    }
}

