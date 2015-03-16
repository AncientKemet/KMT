using Shared.Content;
using UnityEngine;

namespace Client.UI.Interfaces.Profile
{
    public class AccessTab : ProfileTab
    {

        public Sprite ViewInventory, AddToInventory, TakeFromInventory, PickUp, Use, Manage;

        private UnitAccess _access;

        public UnitAccess Access
        {
            get { return _access; }
            set { _access = value; }
        }
    }
}
