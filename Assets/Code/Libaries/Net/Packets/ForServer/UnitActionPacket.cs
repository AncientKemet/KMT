//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Runtime Version:4.0.30319.18449
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

using Code.Code.Libaries.Net;
using UnityEngine;

namespace Code.Libaries.Net.Packets.ForServer
{
    public class UnitActionPacket : BasePacket
    {
        public int UnitId { get; set; }
        public string ActionName { get; set; }

        #region implemented abstract members of BasePacket

        protected override int GetOpCode()
        {
            return 21;
        }

        protected override void enSerialize(ByteStream bytestream)
        {
            bytestream.AddShort(UnitId);
            bytestream.AddString(ActionName);
        }

        protected override void deSerialize(ByteStream bytestream)
        {
            UnitId = bytestream.GetUnsignedShort();
            ActionName = bytestream.GetString();
        }

        #endregion
    }

}
