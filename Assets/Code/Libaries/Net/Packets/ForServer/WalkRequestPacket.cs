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
    public class WalkRequestPacket : BasePacket
    {
        public Vector3 DirecionVector;

        #region implemented abstract members of BasePacket

        protected override int GetOpCode()
        {
            return 80;
        }

        protected override void enSerialize(ByteStream bytestream)
        {
            bytestream.AddPosition6B(DirecionVector);
        }

        protected override void deSerialize(ByteStream bytestream)
        {
            DirecionVector = bytestream.GetPosition6B();
        }

        #endregion
    }

}
