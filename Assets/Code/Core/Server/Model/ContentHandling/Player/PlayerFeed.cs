#if SERVER
using Libaries.Net.Packets.Data;

using System;
using Shared.Content.UI;

namespace Server.Model.ContentHandling.Player
{
    //Class that helps handling the Player Feed.
    public static class PlayerFeed
    {

        public enum FeedRarity
        {
            Normal,
            Rare,
            Epic,
            Legendary
        }

        public static void AddFeed(Entities.Human.Player _player, string title, string description, FeedRarity rarity)
        {
            PlayerFeedMessage message = new PlayerFeedMessage(title, DateTime.UtcNow, description, _player.Client.UserAccount.Username, rarity.ToString());
            PushAllFeedsIndexBelow(_player);
            _player.Client.Server.DataServerConnection.SetData("u/" + _player.Client.UserAccount.Username + "/feed0", message.Serialize());
        }

        /// <summary>
        /// OffSets all feeds a index over one,
        /// Example:
        ///  content of u/xxx/feed0 will be now in u/xxx/feed1
        /// 
        /// </summary>
        /// <param name="_player"></param>
        private static void PushAllFeedsIndexBelow(Entities.Human.Player _player)
        {
            for (int i = 9; i > -1; i--)
            {
                _player.Client.Server.DataServerConnection.RequestData(
                    new DataServerConnection.DataRequest("u/" + _player.Client.UserAccount.Username + "/feed" + i, (b, s) =>
                    {
                        _player.Client.Server.DataServerConnection.SetData(
                            "u/" + _player.Client.UserAccount.Username + "/feed" + i + 1,
                            s);
                    }));
            }
            
        }
    }
}
#endif
