using Code.Code.Libaries.Net;

namespace Libaries.Net.Packets
{
    public class AuthenticationPacket : BasePacket
    {

        public static AuthenticationPacket LastSentPacket;

        public string Username { get; set; }
        public string Password { get; set; }

        protected override int GetOpCode()
        {
            return 1;
        }

        protected override void enSerialize(ByteStream bytestream)
        {
            bytestream.AddString(Username);
            bytestream.AddString(Password);

            LastSentPacket = this;
        }

        protected override void deSerialize(ByteStream bytestream)
        {
            Username = bytestream.GetString();
            Password = bytestream.GetString();
        }
    }
}
