using Server.Model.Entities.Human;
using Server.Servers;
using UnityEngine;
#if SERVER

namespace Server.Model.Content.Spawns
{
    public class NPCSpawn : MonoBehaviour
    {

        public int ModelID = 0;

        [Range(-1f,100f)]
        public float WalkRange = -1f;

        public float MinSleepTime = 1;
        public float MaxSleepTime = 5;

        public Vector3 StaticPosition { get; set; }

        void Awake ()
        {
            NPC n = gameObject.AddComponent<NPC>();

            n.Display.ModelID = ModelID;

            if (n.CurrentWorld == null)
                ServerSingleton.Instance.GetComponent<WorldServer>().World.AddEntity(n);

            n.Movement.Teleport(transform.position);

            StaticPosition = transform.position;

            n.Spawn = this;

            foreach (var ext in GetComponents<NpcSpawnExtension>())
            {
                ext.Apply(n);
            }
        }
    }
}
#endif
