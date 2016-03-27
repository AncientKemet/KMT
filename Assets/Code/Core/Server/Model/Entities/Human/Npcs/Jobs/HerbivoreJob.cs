using UnityEngine;

namespace Server.Model.Entities.Human.Npcs.Jobs
{
    public class HerbivoreJob : aJob
    {
        public HerbivoreJob(NPC me) : base(me)
        {
        }

        private enum State
        {
            Patrolling,
            Eating,
            Defending,
            Resting
        }
        
        private bool _startedWalking = false;
        private bool _finished = false;
        private bool _interrupted = false;

        private Vector3 currentWay = Vector3.forward;

        private State Current = State.Resting;

        public override bool Finished
        {
            get { return _finished; }
        }

        public override bool Continue()
        {
            if (!_interrupted)
            {
                switch (Current)
                {
                    case State.Patrolling:

                        if (!_startedWalking)
                        {
                            currentWay /= 1.5f;
                            currentWay = new Vector3(Random.Range(-3f, 3f), 0, Random.Range(-3f, 3f));
                            _startedWalking = true;
                            n.Movement.WalkTo(
                                n.StaticPosition + currentWay,
                                () => _finished = true, () => _interrupted = true);
                        }
                        break;
                }
                return true;
            }
            return false;
        }
    }
}
