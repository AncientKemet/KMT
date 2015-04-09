using Server.Model.Entities.Human;
using UnityEngine;

namespace Server.Model.Content.Spawns.NpcSpawns
{
    public class NpcSpawnExtension : MonoBehaviour
    {
        public NPC Npc { get; private set; }

        public virtual void Apply(NPC n)
        {
            Npc = n;
        }
    
    }
}
