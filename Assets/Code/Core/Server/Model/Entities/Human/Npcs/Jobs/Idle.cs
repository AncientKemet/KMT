using UnityEngine;

namespace Server.Model.Entities.Human.Npcs.Jobs
{
    public class Idle : aJob
    {
        private float _maxTime, _minTime, _decidedTime = -1, _startTime = -1;

        public Idle(NPC me, float maxTime)
            : base(me)
        {
            _maxTime = maxTime;
        }

        public Idle(NPC me, float minTime, float maxTime)
            : base(me)
        {
            _maxTime = maxTime;
            _minTime = minTime;

            if (maxTime < 0)
            {
                maxTime = 1;
            }
        }

        public override bool Continue()
        {
            if (_decidedTime < 0)
            {
                _decidedTime = Mathf.Clamp(Random.Range(_minTime, _maxTime),1,999);
                _startTime = Time.time;
            }
            if (_startTime + _decidedTime > Time.time)
                Finished = true;
            return true;
        }
    }
}
