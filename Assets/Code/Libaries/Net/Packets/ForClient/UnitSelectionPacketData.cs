using System.Collections;
using System.Collections.Generic;
using Code.Code.Libaries.Net;
using Shared.Content.Types;

namespace Libaries.Net.Packets.ForClient
{
    public class UnitSelectionPacketData : BasePacket
    {
        public string UnitName;

        public bool HasCombat { get; set; }

        public bool HasAttributes { get; set; }

        public Dictionary<UnitAttributeProperty, float> Attributes;

        protected override int GetOpCode()
        {
            return 23;
        }

        protected override void enSerialize(ByteStream bytestream)
        {
            bytestream.AddString(UnitName);

            bytestream.AddFlag(HasCombat,HasAttributes);

            if (HasAttributes)
            {
                bytestream.AddInt(Attributes.Count);
                foreach (KeyValuePair<UnitAttributeProperty, float> pair in Attributes)
                {
                    bytestream.AddByte((int)pair.Key);
                    bytestream.AddFloat4B((int)pair.Value);
                }
            }
        }

        protected override void deSerialize(ByteStream bytestream)
        {
            UnitName = bytestream.GetString();

            BitArray mask = bytestream.GetBitArray();

            HasCombat = mask[0];
            HasAttributes = mask[1];

            if (HasAttributes)
            {
                int count = bytestream.GetInt();
                Attributes = new Dictionary<UnitAttributeProperty, float>(count);
                for (int i = 0; i < count; i++)
                {
                    Attributes.Add((UnitAttributeProperty) bytestream.GetUnsignedByte(), bytestream.GetFloat4B());
                }
            }
        }
    }
}