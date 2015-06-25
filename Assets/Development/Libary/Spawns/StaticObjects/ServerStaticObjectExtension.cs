using Server.Model.Entities;
using UnityEngine;

#if SERVER
namespace Development.Libary.Spawns.StaticObjects
{
    public abstract class ServerStaticObjectExtension : MonoBehaviour
    {

        public abstract void Apply(ServerUnit serverUnit);
    }
}
#endif
