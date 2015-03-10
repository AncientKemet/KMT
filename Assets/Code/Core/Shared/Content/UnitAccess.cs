using System;
using System.Collections;
using Code.Code.Libaries.Net;

namespace Shared.Content
{
    public class UnitAccess
    {
        public bool View_Inventory, Add_To_Inventory, Take_From_Inventory, Use, Pick_Up, Manage_Accesses;
        public int DatabaseID;

        public void Serialize(ByteStream b)
        {
            b.AddFlag(View_Inventory,Add_To_Inventory,Take_From_Inventory,Use,Pick_Up,Manage_Accesses);
            b.AddInt(DatabaseID);
        }

        public void Deserialize(ByteStream b)
        {
            BitArray m = b.GetBitArray();

            View_Inventory = m[0];
            Add_To_Inventory = m[1];
            Take_From_Inventory = m[2];
            Use = m[3];
            Pick_Up = m[4];
            Manage_Accesses = m[5];

            DatabaseID = b.GetInt();
        }
    }
}
