using System;
using Server.Model.Content.Spawns;
using Server.Model.Entities.Human.Npcs;
using Random = UnityEngine.Random;
#if SERVER
using UnityEngine;

namespace Server.Model.Entities.Human
{
    public class NPC : Human
    {
        public aNpcBehaviour Behaviour;

        private float _behaviourTime = 0;
        public Vector3 StaticPosition { get; set; }

        public override void Progress(float time)
        {
            base.Progress(time);
            if(Combat == null || !Combat.Dead)
            _behaviourTime += time;
            if (_behaviourTime > 1f)
            {
                _behaviourTime -= 1f;
                if (Behaviour != null)
                {
                    Behaviour.Behave();
                }
            }
        }

    }
}

#endif
