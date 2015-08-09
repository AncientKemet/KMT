using System;
using System.Collections.Generic;
using Server.Model.Entities.Human.Npcs;

namespace Server.Model.Entities.Human.Npcs
{
    public abstract class aNpcBehaviour
    {
        protected NPC n;

        private string _jobString;
        public List<string> JobList = new List<string>(); 
        public string JobString
        {
            get { return _jobString; }
            set
            {
                _jobString = value;
                var splitJobs = JobString.Split('\n');
                foreach (var s in splitJobs)
                {
                    if (!string.IsNullOrEmpty(s))
                    {
                        if (char.IsNumber(s[0]))
                        {
                            string _realJobName = s.Substring(s.IndexOf('*')+1);
                            for (int i = 0; i < Int32.Parse(s.Substring(0, s.IndexOf('*'))); i++)
                            {
                                JobList.Add(_realJobName);
                            }
                        }
                        else
                        {
                            JobList.Add(s);
                        }
                    }
                }
            }
        }

        public aNpcBehaviour(NPC me)
        {
            n = me;
        }

        protected aJob CurrentJob;

        /// <summary>
        /// This is called whenever the npc is doing nothing and has to decide what to do.
        /// </summary>
        /// <returns></returns>
        protected abstract aJob DecideNextJob();

        public void Behave()
        {
            if (CurrentJob != null)
            {
                if (CurrentJob.Finished)
                    CurrentJob = null;
                else if (!CurrentJob.Continue())
                    CurrentJob = null;
            }
            else
                CurrentJob = DecideNextJob();
        }
    }
}
