#if SERVER
using System;
using Code.Code.Libaries.Net;
using Server.Model.Entities;
using UnityEngine;

namespace Server.Model.Extensions
{
    public class CollisionExt : EntityExtension
    {
        private ServerUnit unit;

        public Action<ServerUnit> OnCollision;

	    public override void Progress()
        {
            if(Time.deltaTime > 0.01)
                return;
            Vector2 pos1 = new Vector2(unit.Movement.Position.x, unit.Movement.Position.z);
	        foreach (var o in unit.IsStatic() ? unit.CurrentBranch.StaticObjectsVisible : unit.CurrentBranch.ObjectsVisible)
	        {
	            ServerUnit u = o as ServerUnit;
                if(u != unit )
	            if (u != null)
	            {

                    CollisionExt ext = u.CollisionExt;
	                if (ext != null)
	                {
                        
                        Vector2 pos2 = new Vector2(u.Movement.Position.x, u.Movement.Position.z);

                        if (Vector2.Distance(pos1, pos2) < u.Display.Size / 2f + unit.Display.Size / 2f + (unit.IsStatic() ? 1.5f : 0))
	                    {
	                        Vector3 vec1 = (pos1 - pos2);
	                        vec1.x += + UnityEngine.Random.Range(0, 0.1f);
	                        vec1.z = vec1.y + UnityEngine.Random.Range(0, 0.1f);
	                        vec1.y = 0;
	                        unit.Movement.PushFromCollision(Vector3.ClampMagnitude(vec1*1000, 0.1f), u);
	                        u.Movement.PushFromCollision(Vector3.ClampMagnitude(vec1*-1000, 0.1f), unit);
	                        if (OnCollision != null)
	                            OnCollision(u);
	                    }
	                }
	            }
	        }
        }

        public override void Serialize(ByteStream bytestream)
        {}

        public override void Deserialize(ByteStream bytestream)
        {}

        protected override void OnExtensionWasAdded()
        {
            base.OnExtensionWasAdded();
            unit = entity as ServerUnit;
        }
    }
}
#endif
