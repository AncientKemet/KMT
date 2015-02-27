#if SERVER
using System.Collections.Generic;
using Libaries.IO;

namespace Server.Model.ContentHandling.Player.AccounteExtensions
{
    public abstract class AccountExtension
    {
        public abstract void LoadFromJson(JSONObject o);
        public abstract void SaveToJson(JSONObject o);

        protected void LoadListFromJson(ref List<int> _ref, string fieldName, JSONObject o)
        {
            if (o.HasField(fieldName))
            {
                JSONObject listJson = o.GetField(fieldName);

                int count = int.Parse(listJson.GetField("Count").str);
                _ref = new List<int>(count);

                for (int i = 0; i < count; i++)
                {
                    _ref.Add(int.Parse(listJson.GetField("" + i).str));
                }
            }
        }

        protected void SaveListToJson(ref List<int> _ref, string fieldName, JSONObject o)
        {
            JSONObject newJsonObject = new JSONObject();

            newJsonObject.AddField("Count", ""+_ref.Count);

            int counter = 0;

            foreach (int i in _ref)
                newJsonObject.AddField("" + counter++, "" + i);

            o.AddField(fieldName, newJsonObject);
        }
    }
}
#endif
