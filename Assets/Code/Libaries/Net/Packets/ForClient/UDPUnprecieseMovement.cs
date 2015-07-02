using System.Collections;
using Code.Code.Libaries.Net;
using Code.Core.Client.Units.Managed;
using Shared.NET;
using UnityEngine;

namespace Libaries.Net.Packets.ForClient
{
    public class UDPUnprecieseMovement : DatagramPacket
    {
        public float Face { get; set; }

        public const float DistanceTo256Ratio = 100f;

        public int UnitID { get; set; }

        public BitArray Mask { get; set; }


        public override int Port
        {
            get { return NetworkConfig.I.WorldUnprecieseMovmentPort; }
        }

        public override int Size
        {
            get { return 7; }
        }

        public Vector3 Difference { get; set; }

        public override void Deserialize(ByteStream b)
        {
            int IdMask = b.GetShort();
            Mask = b.GetIdMask2BMASK(IdMask);
            UnitID = b.GetIdMask2BID(IdMask);
            Face = b.GetAngle2B();

            sbyte x = (sbyte) b.GetByte(), y = (sbyte) b.GetByte(), z = (sbyte) b.GetByte();

            Difference = new Vector3((float)x / DistanceTo256Ratio, (float)y / DistanceTo256Ratio, (float)z / DistanceTo256Ratio);
        }
        public override void Serialize(ByteStream b)
        {
            b.AddIdMask2B(UnitID, Mask);
            b.AddAngle2B(Face);
                b.AddByte((int)(Difference.x * DistanceTo256Ratio));
                b.AddByte((int)(Difference.y * DistanceTo256Ratio));
                b.AddByte((int)(Difference.z * DistanceTo256Ratio));
        }

        public override void Execute()
        {
            if (UnitManager.Instance.HasUnit(UnitID))
            {
                UnitManager.Instance[UnitID].OnUnprecieseMovement(this);
            }
        }

        
    }
}
