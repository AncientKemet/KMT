﻿using System;
using System.Collections;
using Code.Code.Libaries.Net;

namespace Libaries.Net.Packets.ForClient
{
    public class ProfileInterfaceUpdatePacket : BasePacket
    {

        public int UnitID { get; set; }
        public bool HasMainTab { get; set; }
        public bool HasLevelsTab { get; set; }
        public bool HasEquipmentTab { get; set; }
        public bool HasTradeTab { get; set; }
        public bool HasQuestTab { get; set; }
        public bool HasPvPTab { get; set; }

        protected override int GetOpCode()
        {
            return 53;
        }

        protected override void enSerialize(ByteStream b)
        {
            b.AddShort(UnitID);
            b.AddBitArray(new BitArray(new []{HasMainTab, HasLevelsTab, HasEquipmentTab,HasTradeTab,HasQuestTab,HasPvPTab}));
        }

        protected override void deSerialize(ByteStream b)
        {
            UnitID = b.GetShort();
            BitArray mask = b.GetBitArray();
            HasMainTab = mask[0];
            HasLevelsTab = mask[1];
            HasEquipmentTab = mask[2];
            HasTradeTab = mask[3];
            HasQuestTab = mask[4];
            HasPvPTab = mask[5];
        }
    }
}