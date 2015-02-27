using Code.Code.Libaries.Net;
using Libaries.IO;

namespace Libaries.Net.Packets.ForClient
{
    public class CharacterCustomalizationDataPacket : BasePacket
    {

        public JSONObject JsonObject;

        protected override int GetOpCode()
        {
            return 54;
        }

        protected override void enSerialize(ByteStream bytestream)
        {
           bytestream.AddString(JsonObject.ToString());
        }

        protected override void deSerialize(ByteStream bytestream)
        {
            JsonObject = new JSONObject(bytestream.GetString());
        }
    }
}
