using Server.Model.Content.Spawns.NpcSpawns;
using Server.Model.Entities.Human;
using Server.Servers;
using UnityEngine;
#if SERVER

namespace Server.Model.Content.Spawns
{
    public class NPCSpawn : MonoBehaviour
    {

        public int ModelID = 0;

        [Range(-1f, 100f)]
        public float WalkRange = -1f;

        public float MinSleepTime = 1;
        public float MaxSleepTime = 5;
        public bool EnableWalking = true;

        public Vector3 StaticPosition { get; set; }
        private NPC n;

        private void Awake()
        {
            n = gameObject.AddComponent<NPC>();
            n.Spawn = this;
            
            n.OnLastSetup += ApplySpawnExtensions;
            
            ServerSingleton.Instance.GetComponent<WorldServer>().World.AddEntity(n);
        }

        void ApplySpawnExtensions()
        {
            n.Display.ModelID = ModelID;

            n.Movement.Teleport(transform.position);

            StaticPosition = transform.position;

            foreach (var ext in GetComponents<NpcSpawnExtension>())
            {
                ext.Apply(n);
            }
        }
    }
}
#endif
