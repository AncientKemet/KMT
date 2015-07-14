using Development.Libary.Spawns;
using Server.Model.Content.Spawns.NpcSpawns;
using Server.Model.Entities.Human;
using Server.Servers;
using Shared.Content.Types;
using UnityEngine;
#if SERVER

namespace Server.Model.Content.Spawns
{
    public class NPCSpawn : StaticObjectInstance
    {

        public int ModelID = 0;

        public Vector3 StaticPosition { get; set; }
        private NPC n;

        protected override void ApplySpawnExtensions()
        {
            base.ApplySpawnExtensions();
            n = _serverUnit as NPC;
            n.Display.ModelID = ModelID;

            StaticPosition = transform.position;

            foreach (var ext in GetComponents<NpcSpawnExtension>())
            {
                ext.Apply(n);
            }
        }
    }
}
#endif
