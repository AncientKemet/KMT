using UnityEngine;
using UnityEngine.SocialPlatforms;

namespace Server.Model.Entities.Human.Npcs.Jobs
{
    public class WalkJob : aJob {

        public WalkJob(NPC me)
            : base(me)
        {
        }

        public WalkJob(NPC me, float range)
            : base(me)
        {
            _range = range;
        }

        private float _range = 10f;
        private bool _startedWalking = false;
        private bool _finished = false;
        private bool _interrupted = false;

        public override bool Finished
        {
            get { return _finished; }
        }

        public override bool Continue()
        {
            if (!_interrupted)
            {
                if (!_startedWalking)
                {
                    _startedWalking = true;
                    n.Movement.WalkTo(
                        n.StaticPosition + new Vector3(Random.Range(-_range, _range), 0, Random.Range(-_range, _range)),
                        () => _finished = true, () => _interrupted = true);
                }
                return true;
            }
            return false;
        }
    }
}
