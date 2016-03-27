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

        public const float floatPrecision = 500f;

        public ushort UnitID { get; set; }
        
        public override int Port
        {
            get { return NetworkConfig.I.WorldUnprecieseMovmentPort; }
        }

        public override int Size
        {
            get { return 10; }
        }

        public Vector3 Difference { get; set; }

        public override void Deserialize(ByteStream b)
        {
            UnitID = b.GetUnsignedShort();
            Face = b.GetAngle2B();
            short x = b.GetShort(), y = b.GetShort(), z = b.GetShort();
            Difference = new Vector3((float)x / floatPrecision, (float)y / floatPrecision, (float)z / floatPrecision);
        }
        public override void Serialize(ByteStream b)
        {
            b.AddShort(UnitID);
            b.AddAngle2B(Face);
                b.AddShort((int)(Difference.x * floatPrecision));
                b.AddShort((int)(Difference.y * floatPrecision));
                b.AddShort((int)(Difference.z * floatPrecision));
        }

        public override void Execute()
        {
            if (UnitManager.Instance.HasUnit(UnitID))
            {
                UnitManager.Instance[UnitID].OnUnprecieseMovement(this);
            }
            else
            {
                Debug.LogError("Broken UDPUnprecieseMovement "+UnitID);
            }
        }

        
    }
}
