using Code.Code.Libaries.Net;

namespace Libaries.Net.Packets.ForClient
{
    public class LoginResponsePacket : BasePacket
    {

        public string DataServerKey { get; set; }

        public LoginResponsePacket()
        { }

        public LoginResponsePacket(string message)
        {
            this.Text = message;
        }
        public LoginResponsePacket(string message, string dataKey)
        {
            this.Text = message;
            this.DataServerKey = dataKey;
        }

        public string Text { get; set; }

        protected override int GetOpCode()
        {
            return 2;
        }

        protected override void enSerialize(ByteStream bytestream)
        {
            bytestream.AddString(Text);
            bytestream.AddString(DataServerKey);
        }

        protected override void deSerialize(ByteStream bytestream)
        {
            Text = bytestream.GetString();
            DataServerKey = bytestream.GetString();
        }
    }
}
