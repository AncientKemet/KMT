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
    public class UIPacket : BasePacket
    {
        public enum UIPacketType
        {
            ERROR = 0,
            SEND_MESSAGE = 1,
        }

        public UIPacketType type {get;set;}
        public string textData = "missing text data";

        #region implemented abstract members of BasePacket

        protected override int GetOpCode()
        {
            return 50;
        }

        protected override void enSerialize(ByteStream bytestream)
        {
            bytestream.AddByte((int)type);
            if(type == UIPacketType.SEND_MESSAGE){
                bytestream.AddString(textData);
            }
        }

        protected override void deSerialize(ByteStream bytestream)
        {
            type = (UIPacketType) bytestream.GetByte();
            if(type == UIPacketType.SEND_MESSAGE){
                textData = bytestream.GetString();
            }
        }

        #endregion
    }
}

