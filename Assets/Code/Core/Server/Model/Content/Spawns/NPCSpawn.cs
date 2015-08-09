using Development.Libary.Spawns;
using Server.Model.Content.Spawns.NpcSpawns;
using Server.Model.Entities.Human;
using Server.Model.Entities.Human.Npcs.Behaviours;
using Server.Servers;
using Shared.Content.Types;
using UnityEngine;
#if SERVER

namespace Server.Model.Content.Spawns
{
    public class NPCSpawn : StaticObjectInstance
    {

        public int ModelID = 0;
        public Fraction Fraction = Fraction.Friend;

        [Multiline(5)]
        public string Jobs = "WalkJob";

        private NPC n;
        
        protected override void ApplySpawnExtensions()
        {
            base.ApplySpawnExtensions();
            n = _serverUnit as NPC;
            n.Display.ModelID = ModelID;
            n.Combat.Fraction = Fraction;
            n.StaticPosition = transform.position;
            n.Behaviour = new DefaultNpcBehaviour(n);
            n.Behaviour.JobString = Jobs;

            foreach (var ext in GetComponents<NpcSpawnExtension>())
            {
                ext.Apply(n);
            }
        }
    }
}
#endif
