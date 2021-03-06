using System.Collections.Generic;
using Shared.Content;
#if SERVER
using Server.Model.Entities;
using Server.Model.Entities.Human;

namespace Server.Model.Extensions.UnitExts
{
    public class UnitAccessOwnership : EntityExtension
    {
        private int _ownerDatabaseId = -1;

        private ServerUnit _owner;

        private List<UnitAccess> _unitAccesses = new List<UnitAccess>();

        public ServerUnit Owner
        {
            get
            {
                if (_owner == null)
                {
                    if (_ownerDatabaseId != -1)
                    {
                        _owner =
                            ServerSingleton.Instance.GetComponent<World>().Players.Find(
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
                    Player p = (Player)value;
                    _ownerDatabaseId = p.Client.UserAccount.DatabaseID;
                }
            }
        }

        /// <summary>
        /// Default public access.
        /// </summary>
        private UnitAccess _publicAccess = new UnitAccess()
        {
            Add_To_Inventory = true,
            Manage_Accesses = false,
            DatabaseID = -1,
            Pick_Up = false,
            Take_From_Inventory = false,
            Use = true,
            View_Inventory = true
        };

        public UnitAccess GetAccessFor(ServerUnit unit)
        {
            Player p = unit as Player;

            if (entity == unit)
                return UnitAccess.Full;

            return GetAccess(p.Client.UserAccount.DatabaseID);
        }

        private UnitAccess GetAccess(int databaseId)
        {
            var a = _unitAccesses.Find(access => access.DatabaseID == databaseId);

            //If no defined access was found we'll return the public access
            if (a == null)
                return _publicAccess;

            return a;
        }

        public override void Progress(float time)
        {
        }

        public void ClearAccesses()
        {
            _unitAccesses.Clear();
        }

        public void SetAccess(Player player, UnitAccess unitAccess)
        {
            unitAccess.DatabaseID = player.Client.UserAccount.DatabaseID;
            var alreadyIn = _unitAccesses.Find(access => access.DatabaseID == player.Client.UserAccount.DatabaseID);
            if(alreadyIn != null)
                _unitAccesses.Remove(alreadyIn);
            _unitAccesses.Add(unitAccess);
        }
    }
}
#endif
