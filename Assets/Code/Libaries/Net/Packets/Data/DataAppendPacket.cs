﻿using Code.Code.Libaries.Net;

namespace Libaries.Net.Packets.Data
{
    public class DataAppendPacket : DataPacket
    {

        public string Data { get; set; }

        protected override int GetOpCode()
        {
            return 253;
        }

        protected override void enSerialize(ByteStream bytestream)
        {
            base.enSerialize(bytestream);
            bytestream.AddString(Data);
        }

        protected override void deSerialize(ByteStream bytestream)
        {
            base.deSerialize(bytestream);
            Data = bytestream.GetString();
        }
    }
}