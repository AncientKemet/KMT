using Server.Model.Content.Spawns;
#if SERVER
using Server.Servers;
using Server.Model.Extensions.UnitExts;
using UnityEngine;

namespace Server.Model.Entities.Human
{
    public class NPC : Human
    {
        public override void Awake()
        {
            base.Awake();
        }

        public NPCSpawn Spawn { get; set; }

        private float _sleepTime = 1f;

        public override void Progress()
        {
            base.Progress();

            if(Spawn.WalkRange > 0)
            if (!Movement.IsWalkingSomeWhere)
            {
                _sleepTime -= Time.fixedDeltaTime;
                if (_sleepTime < 0)
                {
                    Movement.WalkTo(
                        Spawn.StaticPosition +
                        new Vector3(
                            Random.Range(-Spawn.WalkRange / 2f, Spawn.WalkRange / 2f),
                            0,
                            Random.Range(-Spawn.WalkRange / 2f, Spawn.WalkRange / 2f)),
                            () => { _sleepTime = Random.Range(Spawn.MinSleepTime, Spawn.MaxSleepTime); }
                            );
                }
            }

        }
    }
}

#endif
