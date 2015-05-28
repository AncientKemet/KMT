using System.Collections;
using Code.Code.Libaries.Net;
using Code.Core.Client.Units.Managed;
using Shared.NET;

namespace Libaries.Net.Packets.ForClient
{
    public class UDPUnprecieseMovement : DatagramPacket
    {
        public float Face { get; set; }

        public const float DistanceTo256Ratio = 100f;

        public int UnitID { get; set; }

        public BitArray Mask { get; set; }

        public float Distance { get; set; }

        public float YAngle { get; set; }

        public float XAngle { get; set; }

        public override int Port
        {
            get { return NetworkConfig.I.WorldUnprecieseMovmentPort; }
        }

        public override int Size
        {
            get { return 6; }
        }
        
        public override void Deserialize(ByteStream b)
        {
            int IdMask = b.GetUnsignedShort();
            Mask = b.GetIdMask2BMASK(IdMask);
            UnitID = b.GetIdMask2BID(IdMask);
            YAngle = b.GetAngle1B();
            XAngle = b.GetAngle1B();
            Face = b.GetAngle1B();
            Distance = b.GetUnsignedByte() / DistanceTo256Ratio;
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
            b.AddIdMask2B(UnitID, Mask);
            b.AddAngle1B(YAngle);
            b.AddAngle1B(XAngle);
            b.AddAngle1B(Face);
            b.AddByte((int) (Distance * DistanceTo256Ratio));
        }
    }
}
