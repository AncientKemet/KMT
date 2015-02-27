using Code.Code.Libaries.Net;
using Libaries.IO;

namespace Libaries.Net.Packets.ForServer
{
    public class AccountPacket : BasePacket
    {
        public enum Action
        {
            CreateAccount,
        }

        public Action action = Action.CreateAccount;

        public JSONObject JsonObject;

        protected override int GetOpCode()
        {
            return 3;
        }

        protected override void enSerialize(ByteStream bytestream)
        {
            bytestream.AddByte((int) action);
            bytestream.AddString(JsonObject.ToString());
        }

        protected override void deSerialize(ByteStream bytestream)
        {
            action = (Action) bytestream.GetByte();
            JsonObject = new JSONObject(bytestream.GetString());
        }
    }
}
