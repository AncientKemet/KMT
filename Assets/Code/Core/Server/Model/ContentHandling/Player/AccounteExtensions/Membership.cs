#if SERVER
using Libaries.IO;

namespace Server.Model.ContentHandling.Player.AccounteExtensions
{
    public class Membership : AccountExtension
    {

        public bool IsMember { get; set; }

        public override void LoadFromJson(JSONObject o)
        {
            if (o.HasField("IsMember"))
            {
                IsMember = o.GetField("IsMember").str == "True";
            }
        }

        public override void SaveToJson(JSONObject o)
        {
            o.AddField("IsMember", IsMember ? "True" : "False");
        }
    }
}
#endif
