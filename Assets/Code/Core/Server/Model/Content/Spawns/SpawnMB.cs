#if SERVER
using UnityEngine;


namespace Server.Model.Content.Spawns
{
    public class SpawnMB : MonoBehaviour
    {

        public UnityEditor.MonoScript Script;
        public float RespawnTime = 30;

        [HideInInspector]
        public WorldEntity SpawnedEntity;

        private void Start()
        {
            /*if (Application.isPlaying)
            {
                if (ServerSingleton.Instance.WorldServer != null)
                {
                    ServerSpawnManager.Instance(ServerSingleton.Instance.WorldServer.World).AddItemSpawn(this);
                }
            }*/
        }

        public void Spawn()
        {
            /*if (Application.isPlaying)
            {
                if (ServerSingleton.Instance.WorldServer != null)
                {
                    Debug.Log("Spawning: "+Script);
                    //ServerSingleton.Instance.WorldServer.World.
                    OnSpawn(SpawnedEntity);
                }
            }*/
        }

        protected virtual void OnSpawn(WorldEntity spawnedEntity) { }
    }
}

#endif
