using Client.Units;
using UnityEngine;
#if CLIENT
namespace Development.Libary.Spawns.StaticObjects
{
    public abstract class ClientStaticObjectExtension : MonoBehaviour
    {
        public abstract void Apply(PlayerUnit playerUnit);
    }
}
#endif
