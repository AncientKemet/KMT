﻿using System.Collections;
using Code.Code.Libaries.Net;
using Libaries.IO;
using Shared.Content;

namespace Libaries.Net.Packets.ForClient
{
    public class ProfileInterfaceUpdatePacket : BasePacket
    {
        public enum PacketTab
        {
            Main, Levels, Equipment, Dialogue, Inventory, Vendor, Trade, Access
        }

        public int UnitID { get; set; }
        public bool HasMainTab { get; set; }
        public bool HasLevelsTab { get; set; }
        public bool HasEquipmentTab { get; set; }
        public bool HasTradeTab { get; set; }
        public bool HasAccessTab { get; set; }
        public bool HasDialogueTab { get; set; }
        
        public bool HasVendorTradeTab { get; set; }
        public bool HasInventoryTab { get; set; }

        public PacketTab Tab { get; set; }
        
        public UnitAccess Access { get; set; }

        public JSONObject JsonObject = new JSONObject();

        protected override int GetOpCode()
        {
            return 53;
        }

        protected override void enSerialize(ByteStream b)
        {
            b.AddShort(UnitID);
            b.AddBitArray(new BitArray(new []{HasMainTab, HasLevelsTab, HasEquipmentTab,HasTradeTab,HasAccessTab,HasDialogueTab,HasVendorTradeTab,HasInventoryTab}));
            
            b.AddByte((int) Tab);

            if(HasAccessTab)
                Access.Serialize(b);

            b.AddString(JsonObject.ToString());
        }

        protected override void deSerialize(ByteStream b)
        {
            UnitID = b.GetShort();
            BitArray mask = b.GetBitArray();

            HasMainTab = mask[0];
            HasLevelsTab = mask[1];
            HasEquipmentTab = mask[2];
            HasTradeTab = mask[3];
            HasAccessTab = mask[4];
            HasDialogueTab = mask[5];
            HasVendorTradeTab = mask[6];
            HasInventoryTab = mask[7];

            Tab = (PacketTab) b.GetByte();

            if (HasAccessTab)
            {
                Access = new UnitAccess();
                Access.Deserialize(b);
            }

            JsonObject = new JSONObject(b.GetString());
        }
    }
}
