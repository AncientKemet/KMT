
#if SERVER
using Server.Servers;
using System.Data;
using System;
using System.Collections.Generic;
using System.Threading;
using MySql.Data.MySqlClient;
using Server.Model.Extensions.PlayerExtensions;
using Server.Model.Extensions.PlayerExtensions.UIHelpers;
using Shared.NET;
using UnityEngine;

namespace Server
{
    /// <summary>
    /// A static class that runs on a separate thread, authenticating login attempts.
    /// </summary>
    public class LoginServer : AServer
    {

        /// <summary>
        /// An index of next client to be progressed.
        /// </summary>
        private int _currentIndex;

        private static MySqlConnection conSQL;
        private static List<Action> sqlActions = new List<Action>();

        Thread _threadSql = new Thread(new ThreadStart(() =>
        {
            try
            {
                conSQL = new MySqlConnection("Server=mysql51.websupport.sk;Port=3309;" +
                                                 "Database=yiqd2eyi;" +
                                                 "User ID=yiqd2eyi;" +
                                                 "Password=1Goghteek*;" +
                                                 "Pooling=true;" +
                                                 "Max Pool Size=100;" +
                                                 "Min Pool Size=0");
                conSQL.Open();
            }
            catch (Exception e)
            {
                Debug.LogException(e);
            }
            while (true)
            {
                lock (sqlActions)
                {
                    foreach (var action in sqlActions)
                    {
                        action();
                    }
                    sqlActions.Clear();
                }
                Thread.Sleep(500);
            }
        })); 

        public override void StartServer()
        {
            try
            {
                _threadSql.Start();
            }
            catch (Exception e)
            {
                Debug.LogException(e);
            }
            base.StartServer();
            socket = CreateServerSocket(NetworkConfig.I.LoginServerPort);
            Debug.Log("Login server running.");
        }

        public override void ServerUpdate(float f)
        {
            base.ServerUpdate(f);
            scm.Get.AcceptConnections(socket);
            if (Clients.Count > 0)
            {
                if (_currentIndex >= Clients.Count)
                    _currentIndex = 0;

                if (Clients[_currentIndex] != null)
                {
                    ServerClient client = Clients[_currentIndex];
                    Action actionToRunOnUnityThread = () => client.Progress(f);

                    lock (ServerSingleton.StuffToRunOnUnityThread)
                    {
                        ServerSingleton.StuffToRunOnUnityThread.Add(actionToRunOnUnityThread);
                    }
                }
                _currentIndex++;
            }
        }

        public override void AddClient(ServerClient client)
        {
            client.UserChat = new UserChat(client);
            client.UI = new ClientUI(client);
            client.UI.Login.Opened = true;
            base.AddClient(client);
        }

        public override void Stop()
        {
            socket.Close();
            conSQL.Close();
            Debug.Log("Stopping Login server.");
        }

        public void Authorize(ServerClient client, string username, string password, Action<bool, int> onResult)
        {
            Action a = () =>
            {
                int DBID = -1;
                MySqlCommand command = conSQL.CreateCommand();
                command.CommandText = "SELECT ID FROM wp_users WHERE user_login='" + username + "'AND user_pass='"+password+"'";
                command.ExecuteNonQuery();
                MySqlDataAdapter dataAdapter = new MySqlDataAdapter(command);
                DataSet dataSet = new DataSet();
                dataAdapter.Fill(dataSet);

                try
                {
                    DBID = int.Parse("" + dataSet.Tables[0].Rows[0]["ID"]);
                    onResult(true, DBID);
                    
                }
                catch (Exception e)
                {
                    onResult(false, DBID);
                }
            };

            lock (sqlActions)
            {
                sqlActions.Add(a);
            }
        }


        public ServerClient GetClient(string toUser)
        {
            return Clients.Find(client => client.UserAccount.Username == toUser);
        }
    }
}
#endif
