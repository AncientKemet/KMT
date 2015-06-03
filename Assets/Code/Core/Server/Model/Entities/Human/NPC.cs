using System;
using Server.Model.Content.Spawns;
using Random = UnityEngine.Random;
#if SERVER
using UnityEngine;

namespace Server.Model.Entities.Human
{
    public class NPC : Human
    {

        public NPCSpawn Spawn { get; set; }

        private float _sleepTime = 1f;
        
        public override void Progress(float time)
        {
            base.Progress(time);

            if(Spawn.EnableWalking)
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
