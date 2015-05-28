using System;
using Code.Code.Libaries.Net;
using Code.Core.Client.UI;
using UnityEngine;

namespace Libaries.Net.Packets.ForServer
{
    public class ItemDragPacket : BasePacket
    {

        public int BeingDragID;
        public InterfaceType BeingDragInterfaceID;
        public int DropOnID;
        public InterfaceType DropOnInterfaceID;

        protected override int GetOpCode()
        {
            return 31;
        }

        protected override void enSerialize(ByteStream bytestream)
        {
            try
            {
                bytestream.AddShort(BeingDragID);
                bytestream.AddShort((int) BeingDragInterfaceID);
                bytestream.AddShort(DropOnID);
                bytestream.AddShort((int) DropOnInterfaceID);
            }
            catch(Exception e)
            { Debug.LogException(e);}
        }

        protected override void deSerialize(ByteStream bytestream)
        {
            try
            {
                BeingDragID = bytestream.GetShort();
                BeingDragInterfaceID = (InterfaceType) bytestream.GetShort();
                DropOnID = bytestream.GetShort();
                DropOnInterfaceID  = (InterfaceType) bytestream.GetShort();
            }
            catch (Exception e)
            { Debug.LogException(e); }
        }
    }
}
