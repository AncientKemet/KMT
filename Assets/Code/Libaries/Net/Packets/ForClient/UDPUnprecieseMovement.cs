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

        public bool[] Mask { get; set; }

        public float Distance { get; set; }

        public float Angle { get; set; }


        public override int Port
        {
            get { return NetworkConfig.I.WorldUnprecieseMovmentPort; }
        }

        public override int Size
        {
            get { return 4; }
        }

        public override void Deserialize(ByteStream b)
        {
            int IdMask = b.GetUnsignedShort();
            UnitID = b.GetIdMask2BID(IdMask);
            Angle = b.GetAngle1B();
            Face = b.GetAngle1B();
            Distance = b.GetUnsignedByte() / DistanceTo256Ratio;
            //Mask = b.GetIdMask2BMASK(IdMask); // this works but is not neccecary
        }

        public override void Execute()
        {
            if (UnitManager.Instance.HasUnit(UnitID))
            {
                UnitManager.Instance[UnitID].OnUnprecieseMovement(this);
            }
        }

        public override void Serialize(ByteStream b)
        {
            
            b.AddIdMask2B(UnitID, Mask == null ? new bool[4] : Mask);
            b.AddAngle1B(Angle);
            b.AddAngle1B(Face);
            b.AddByte((int) (Distance * DistanceTo256Ratio));
        }
    }
}
