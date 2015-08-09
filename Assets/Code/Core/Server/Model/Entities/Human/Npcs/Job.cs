using System;
using System.Collections.Generic;
using System.Reflection;
using Server.Model.Entities.Human.Npcs.Jobs;
using UnityEngine;

namespace Server.Model.Entities.Human.Npcs
{
    public abstract class aJob
    {
        protected NPC n;

        protected aJob(NPC me)
        {
            this.n = me;
        }

        public virtual bool Finished { get; protected set; }

        public abstract bool Continue();
    }

    public static class JobManager
    {
        private static Dictionary<string, Type> Jobs;

        private static bool _jobsWereLoaded = false;

        public static aJob JobForName(NPC npc, string name)
        {
            List<object> param = new List<object>();
            param.Add(npc);
            string originalName = name;
            if (name.Contains("("))
            {
                string paramString = name.Split('(')[1].Split(')')[0]; // result "1,0.01f,\"Hello\""
                string[] parameterStrings = paramString.Split(','); // result "1", "0.01f", "\"Hello\""
                foreach (var s in parameterStrings)
                {
                    int i;
                    if (s.Contains("."))
                    {
                        float f;
                        if (float.TryParse(s, out f))
                        {
                            param.Add(f);
                        }
                        else
                        {
                            param.Add(s);
                        }
                    }
                    else if(int.TryParse(s, out i))
                    {
                        param.Add(i);
                    }
                    else
                    {
                        param.Add(s);
                    }
                }
                name = name.Remove(name.IndexOf('('));
            }

            if (!_jobsWereLoaded)
            {
                LoadJobs();
            }
            aJob aJob;
            try
            {
                aJob = (aJob)Activator.CreateInstance(Jobs[name], param.ToArray());
            }
            catch (KeyNotFoundException e)
            {
                Debug.LogError("Unknown job name: " + originalName);
                return null;
            }
            catch (MissingMethodException e)
            {
                string _parameterS = "";
                foreach (var o in param)
                {
                    _parameterS += "," + o.GetType().Name;
                }
                Debug.LogError("Unknown job name: " + originalName + " - " + _parameterS);
                return null;
            }
            return aJob;
        }

        private static void LoadJobs()
        {
            _jobsWereLoaded = true;
            Jobs = new Dictionary<string, Type>();
            Type aJobType = typeof(aJob);
            foreach (var type in Assembly.GetAssembly(typeof(WalkJob)).GetTypes())
            {
                if (aJobType.IsAssignableFrom(type) && type != aJobType && !type.IsAbstract)
                    Jobs[type.Name] = type;
            }
        }
    }
}
