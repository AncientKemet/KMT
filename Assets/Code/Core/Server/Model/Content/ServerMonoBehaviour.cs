#if SERVER
using System;
using UnityEngine;

namespace Server.Model.Content
{
    public class ServerMonoBehaviour : MonoBehaviour
    {
        public static T CreateInstance<T>() where T : ServerMonoBehaviour
        {
            return CreateInstance<T>(null);
        }

        public static WorldEntity CreateInstance(Type type) 
        {
            WorldEntity t = (WorldEntity)new GameObject(type.Name).AddComponent(type);
            
            return t;
        }

        public static T CreateInstance<T>(World world) where T : ServerMonoBehaviour
        {
            T t = new GameObject(typeof(T).Name).AddComponent<T>();

            if (world == null)
            {
                t.transform.parent = ServerSingleton.Instance.transform;
            }
            else
            {
                t.transform.parent = world.transform;
            }

            return t;
        }
    }
}

#endif
