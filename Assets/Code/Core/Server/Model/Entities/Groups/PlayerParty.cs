#if SERVER
using System;
using System.Collections.Generic;
using Server.Model.Entities.Human;

namespace Server.Model.Entities.Groups
{
    public class PlayerParty : UnitGroup
    {
        public List<Player> Players = new List<Player>();

        public Player Owner
        {
            get
            {
                if (Players.Count > 0)
                {
                    return Players[0];
                }
                return null;
            }
        }

        public Action<Player> OnPlayerJoined, OnPlayerLeft;

        public void Join(Player p)
        {
            if (p.Party != null)
                p.Party.Leave(p);

            OnPlayerJoined(p);
            Players.Add(p);
            p.Party = this;
        }

        public void SayInParty(string message)
        {
            foreach (var p in Players)
            {
                if(p != null)
                    p.Client.UserChat.RecievePartyMessage(message);
            }
        }

        public void Leave(Player p)
        {
            if(Players.Contains(p))
            {
                Players.Remove(p);
                OnPlayerLeft(p);
                p.Party = null;
            }
        }

        public PlayerParty CreatePlayerParty(Player owner)
        {
            PlayerParty party =  new PlayerParty();
            party.OnPlayerJoined += (p) => party.SayInParty(p.name + " has joined your party.");
            party.OnPlayerLeft += (p) => party.SayInParty(p.name + " has left your party.");  
            party.Join(Owner);

            return party;
        }

        private PlayerParty() { }
    }

}
#endif
