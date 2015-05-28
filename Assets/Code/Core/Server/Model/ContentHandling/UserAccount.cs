using Server.Servers;
#if SERVER
using Code.Libaries.Generic;
using Server.Model.ContentHandling.Player.AccounteExtensions;

using Libaries.Net.Packets.Data;
using System;
using Libaries.IO;
using Server.IO.Encryption;
using UnityEngine;

namespace Server.Model.ContentHandling
{
    public class UserAccount
    {
        public UserAccount(DataServerConnection dataServerConnection, string _key)
        {
            DSC = dataServerConnection;
            _dataServerKey = _key;
            LoadAccount();
            OnNewAccountCreated += () => CharacterCustomalizations.Get.UnlockDefaults();
        }

        public UserAccount(DataServerConnection dataServerConnection, int _dbID)
        {
            DSC = dataServerConnection;
            DatabaseID = _dbID;
            LoadAccount();
            OnNewAccountCreated += () => CharacterCustomalizations.Get.UnlockDefaults();
        }

        private string _dataServerKey;
        private DataServerConnection DSC;
        private Entities.Human.Player _player;
        
        public string Username { get; set; }
        public string Password { get; set; }
        public int DatabaseID { get; set; }

        public Action OnNewAccountCreated;

        public GenProperty<CharacterCustomalizations> CharacterCustomalizations = new GenProperty<CharacterCustomalizations>();
        public GenProperty<Membership> Member = new GenProperty<Membership>();

        public JSONObject ToJson()
        {
            JSONObject o = new JSONObject();

            o.AddField("username",Username);

            CharacterCustomalizations.Get.SaveToJson(o);
            Member.Get.SaveToJson(o);
            
            return o;
        }

        public void Deserialize(JSONObject o)
        {
            Username = o.GetField("username").str;

            CharacterCustomalizations.Get.LoadFromJson(o);
            Member.Get.LoadFromJson(o);
        }
        
        public string DataServerKey
        {
            get
            {
                if (_dataServerKey == null)
                {
                    GenerateDataServerKey();
                }
                return _dataServerKey;
            }
            set
            {
                _dataServerKey = value;
            }
        }

        private void GenerateDataServerKey()
        {
            _dataServerKey = new AES().EncryptToString("AK"+DatabaseID*5);
        }

        public void AddFriend(string text)
        {
            string dataPath = "s/" + DataServerKey + "/friendsList";
            DSC.RequestData(dataPath, (b, s) =>
            {
                if (b)
                {
                    s += "," + text;
                    DSC.SetData(dataPath, s);
                }
                else
                {
                    //so its probably a new user without any friends list yet
                    DSC.SetData(dataPath, text);
                }
            });
        }

        public void RemoveFriend(string text)
        {
            string dataPath = "s/" + DataServerKey + "/friendsList";
            DSC.RequestData(dataPath, (b, s) =>
            {
                if (b)
                {
                    DSC.SetData(dataPath, s.Replace("," + text, ""));
                }
                else
                {
                    //so its probably a new user without any friends list yet
                    DSC.SetData(dataPath, "");
                }
            });
        }

        public void LoadUnit(WorldServer worldServer, Entities.Human.Player player)
        {
            _player = player;
            _player.name = "Requesting data...";
            DSC.RequestData("s/" + DataServerKey + "/unit", (b, s) =>
            {
                if (b)
                {
                    JSONObject j = new JSONObject(s);

                    player.Deserialize(j);

                    worldServer.World.AddEntity(player);
                }
                else
                {
                    player.SetupNewPlayer(worldServer);
                }

                player.name = Username;

            });
        }

        public void LoadAccount()
        {
            DSC.RequestData("s/" + DataServerKey + "/account", (b, s) =>
            {
                if (b)
                    Deserialize(new JSONObject(s));
                else
                {
                    Debug.Log("raw new account has connected");
                    if (OnNewAccountCreated != null)
                        OnNewAccountCreated();

                    SaveAccount();
                }

            });
        }

        public void SaveAccount()
        {
            DSC.SetData("s/" + DataServerKey + "/account", ToJson().ToString());
        }

        public void SaveUnit(Entities.Human.Player p)
        {
            JSONObject j = new JSONObject();
            p.Serialize(j);
            
            DSC.SetData("s/"+DataServerKey+"/unit", j.ToString());
        }
    }
}
#endif
