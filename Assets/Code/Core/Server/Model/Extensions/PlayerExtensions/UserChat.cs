#if SERVER
using Server.Model.Entities.Minerals;
using Server.Model.Entities.Minerals.Configs;
using Server.Model.Entities.Minerals.IMPL;

using UnityEngine;

using Code.Libaries.Generic.Managers;
using Server.Model.ContentHandling;
using Server.Model.Entities.Animals.Birds;
using Server.Model.Entities.Human.Mythical;
using Code.Libaries.Net.Packets.ForServer;
using Server.Model.Entities.Human;

namespace Server.Model.Extensions.PlayerExtensions
{
    public class UserChat
    {
        public UserChat(ServerClient client)
        {
            Client = client;
        }

        public ServerClient Client { get; set; }

        public Player Player
        {
            get { return Client.Player; }
        }

        internal void HandlePacket(ChatPacket p)
        {
            if(p.type == ChatPacket.ChatType.Public)
            {
                if (p.text.StartsWith("."))
                {
                    if (p.text.Contains("item"))
                    {
                        Player.Inventory.AddItem(ContentManager.I.Items[int.Parse(p.text.Split("."[0])[2])]);
                    }
                    if (p.text.Contains("chick"))
                    {
                        for (int i = 0; i < int.Parse(p.text.Split("."[0])[2]); i++)
                        {
                            ServerSpawnManager.Instance(Player.CurrentWorld).SpawnAnimal<Chicken>(Player.Movement.Position);
                        }
                        
                    }
                    if (p.text.Contains("zombie"))
                    {
                        ServerSpawnManager.Instance(Player.CurrentWorld).Spawn<Zombie>(Player.Movement.Position);
                    }
                    if (p.text.Contains("limestone"))
                    {
                        ServerSpawnManager.Instance(Player.CurrentWorld).Spawn<LimestoneMother>(Player.Movement.Position);
                    }
                    if (p.text.Contains("kill"))
                    {
                        if (Player.Focus.FocusedUnit != null)
                        {
                            if (Player.Focus.FocusedUnit.Combat != null)
                            {
                                Player.Focus.FocusedUnit.Combat.ReduceHealth(Player.Combat, 50000);
                            }
                        }
                    }
                    if (p.text.Contains("revive"))
                    {
                        if (Player.Focus.FocusedUnit != null)
                        {
                            if (Player.Focus.FocusedUnit.Combat != null)
                            {
                                Player.Focus.FocusedUnit.Combat.Revive(100);
                            }
                        }
                    }
                }
                else
                    Say(p.text);
            }else if (p.type == ChatPacket.ChatType.Party)
            {
                SendPartyMessage(Player.name + ": " + p.text);
            }
            else if (p.type == ChatPacket.ChatType.AddFriend)
            {
                if(p.text.Length > 6)
                Client.UserAccount.AddFriend(p.text);
            }
            else if (p.type == ChatPacket.ChatType.RemoveFriend)
            {
                if (p.text.Length > 6)
                    Client.UserAccount.RemoveFriend(p.text);
            }
            else if (p.type == ChatPacket.ChatType.Private)
            {
                Client.UserChat.SendPrivateMessageTo(p.User, p.text);
            }
            else
            {
                Debug.LogError("unknown chat packet type: "+p.type);
            }
        }

        public void SendPrivateMessageTo(string toUser, string text)
        {
            var loginServer = Client.Server as LoginServer;

            if (loginServer != null)
            {
                var user = loginServer.GetClient(toUser);
                if(user != null)
                user.UserChat.RecievePrivateMessage(Client.UserAccount.Username,text);
            }
            else
            {
                Debug.LogError("TODO private message for non login server ?");
            }
        }

        private void RecievePrivateMessage(string fromUser, string text)
        {
            ChatPacket packet = new ChatPacket();
            packet.type = ChatPacket.ChatType.Private;
            packet.User = fromUser;
            packet.text = text;
            Client.ConnectionHandler.SendPacket(packet);
        }

        private void Say(string message)
        {

            ChatPacket p = new ChatPacket();

            p.FROM_SERVER_UnitID = Player.ID;
            p.type = ChatPacket.ChatType.Public;
            p.text = Player.name+": "+message;

            foreach (var e in Player.CurrentBranch.ObjectsVisible)
            {
                if(e is Player)
                {
                    Player other = e as Player;

                    other.Client.ConnectionHandler.SendPacket(p);
                }
            }
        }

        public void RecievePartyMessage(string message)
        {
            ChatPacket p = new ChatPacket();

            p.type = ChatPacket.ChatType.Party;
            p.text = message;

            Player.Client.ConnectionHandler.SendPacket(p);
        }

        public void SendPartyMessage(string message)
        {
            if (Player.Party != null)
            {
                Player.Party.SayInParty(message);
            }
        }

        public void SendGameMessage(string message)
        {
            ChatPacket p = new ChatPacket();

            p.type = ChatPacket.ChatType.GAME;
            p.text = message;

            Player.Client.ConnectionHandler.SendPacket(p);
        }
    }
}
#endif
