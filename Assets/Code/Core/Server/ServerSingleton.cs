using System;
using System.Collections;
using System.Collections.Generic;
using Code.Libaries.Generic;
using Server.Servers;

namespace Server
{
    public class ServerSingleton : MonoSingleton<ServerSingleton>
    {


        public static List<Action> StuffToRunOnUnityThread;

        public bool Master, World, Data, Login;
        public string DataServerIPAdress = "127.0.0.1";
        public string DataCertificate = "missing certificate";
        public string DataRootPath = "not initialized";
#if SERVER
        public MasterServer MasterServer { get; private set; }
        public WorldServer WorldServer { get; private set; }
        public DataServer DataServer { get; private set; }
        public LoginServer LoginServer { get; private set; }

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
            if (Data)
            {
                DataServer = new DataServer();
                DataServer.StartServer();
            }
            if (Master)
            {
                MasterServer = new MasterServer();
                MasterServer.StartServer();
            }
            if (World)
            {
                WorldServer = new WorldServer(0);
                WorldServer.StartServer();
            }
            if (Login)
            {
                LoginServer = new LoginServer();
                LoginServer.StartServer();
            }
#endif
        }
#if SERVER
        private void OnDisable()
        {
            if (World)
                WorldServer.Stop();
            if (Login)
                LoginServer.Stop();
            if (Data)
                DataServer.Stop();
            if (Master)
                MasterServer.Stop();
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

            if(World && WorldServer != null)
                WorldServer.ServerUpdate();
            if(Login && LoginServer != null)
                LoginServer.ServerUpdate();
            if(Data && DataServer != null)
                DataServer.ServerUpdate();
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
