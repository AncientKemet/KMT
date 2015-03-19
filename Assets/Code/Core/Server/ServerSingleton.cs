using System;
using System.Collections;
using System.Collections.Generic;
using Code.Libaries.Generic;
using Server.Servers;
using UnityEngine;

namespace Server
{
    public class ServerSingleton : MonoSingleton<ServerSingleton>
    {


        public static List<Action> StuffToRunOnUnityThread;

        public string DataServerIPAdress = "127.0.0.1";
        public string DataCertificate = "missing certificate";
        public string DataRootPath = "not initialized";

        public Transform GOPool;
#if SERVER

        protected override void OnAwake()
        {
            StuffToRunOnUnityThread = new List<Action>();
        }
#endif
        private void OnEnable()
        {
#if !UNITY_EDITOR
            Destroy(gameObject);
#endif
#if SERVER
#endif
        }
#if SERVER
        private void OnDisable()
        {
        }
	
        void FixedUpdate () {
            //Run stuff that needs to be ran
            lock (StuffToRunOnUnityThread)
            {
                for (int i = 0; i < StuffToRunOnUnityThread.Count; i++)
                {
                    Action action = null;
                    try
                    {
                        action = StuffToRunOnUnityThread[i];
                    }
                    catch (Exception e) { }
                    
                    if(action != null)
                        action();
                }
                    

                StuffToRunOnUnityThread.Clear();
            }

        }

        private void OnDrawGizmos()
        {
            /*if(Application.isPlaying)
                if(WorldServer != null)
                    WorldServer.swm.Get.Kemet.Tree.DrawGizmos();*/
        }

        public void RunCoroutine(IEnumerator function)
        {
            StartCoroutine(function);
        }
#endif
    }
}
