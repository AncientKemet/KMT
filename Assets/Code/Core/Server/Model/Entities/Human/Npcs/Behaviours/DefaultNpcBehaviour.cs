
using Server.Model.Entities.Human.Npcs.Jobs;
using UnityEngine;

namespace Server.Model.Entities.Human.Npcs.Behaviours
{
    public class DefaultNpcBehaviour : aNpcBehaviour
    {

        public DefaultNpcBehaviour(NPC me) : base(me)
        {
        }

        protected override aJob DecideNextJob()
        {
            
            int id = Random.Range(0, JobList.Count);
            string jobname = JobList[id];
            return JobManager.JobForName(n, jobname);
        }
    }
}
