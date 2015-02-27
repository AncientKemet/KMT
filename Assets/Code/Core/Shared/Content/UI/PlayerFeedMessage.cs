using System;
using System.Globalization;
using Libaries.IO;

namespace Shared.Content.UI
{
    public class PlayerFeedMessage
    {
        public string Title { get; private set; }
        public DateTime Time { get; private set; }
        public string Description { get; private set; }
        public string Username { get; private set; }
        public string Rarity { get; private set; }

        private PlayerFeedMessage()
        { }

        public PlayerFeedMessage(string title, DateTime time, string description, string username, string rarity)
        {
            Title = title;
            Time = time;
            Description = description;
            Username = username;
            Rarity = rarity;
        }

#if SERVER
        public string Serialize()
        {
            JSONObject jsonObject = new JSONObject();

            jsonObject.AddField("Title", Title);
            jsonObject.AddField("Time", Time.ToString("yyyy-MM-dd HH:mm"));
            jsonObject.AddField("Description", Description);
            jsonObject.AddField("Username", Username);
            jsonObject.AddField("Rarity", Rarity);

            return jsonObject.ToString();
        }
#endif

        public static PlayerFeedMessage Deserialize(string input)
        {
            PlayerFeedMessage message = new PlayerFeedMessage();

            JSONObject jsonObject = new JSONObject(input);

            message.Title = jsonObject.GetField("Title").str;
            message.Time = DateTime.ParseExact(jsonObject.GetField("Time").str, "yyyy-MM-dd HH:mm", CultureInfo.InvariantCulture);
            message.Description = jsonObject.GetField("Description").str;
            message.Username = jsonObject.GetField("Username").str;
            message.Rarity = jsonObject.GetField("Rarity").str;

            return message;
        }
    }
}
