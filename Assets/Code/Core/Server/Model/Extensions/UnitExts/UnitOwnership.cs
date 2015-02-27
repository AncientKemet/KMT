#if SERVER
using Code.Code.Libaries.Net;
using Server.Model.Entities;
using Server.Model.Entities.Human;

namespace Server.Model.Extensions.UnitExts
{
    public class UnitOwnership : EntityExtension
    {
        private int _ownerDatabaseId = -1;
        private ServerUnit _owner;

        public ServerUnit Owner
        {
            get
            {
                if (_owner == null)
                {
                    if (_ownerDatabaseId != -1)
                    {
                        _owner =
                            ServerSingleton.Instance.WorldServer.World.Players.Find(
                                player => player.Client.UserAccount.DatabaseID == _ownerDatabaseId);
                    }
                }
                return _owner;
            }
            set
            {
                _owner = value;
                if (value is Player)
                {
                    Player p = (Player) value;
                    _ownerDatabaseId = p.Client.UserAccount.DatabaseID;
                }
            }
        }

        public override void Progress()
        {
            
        }

        public override void Serialize(ByteStream bytestream)
        {
            bytestream.AddInt(_ownerDatabaseId);
        }

        public override void Deserialize(ByteStream bytestream)
        {
            _ownerDatabaseId = bytestream.GetInt();
        }
    }
}
#endif
