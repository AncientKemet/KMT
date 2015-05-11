using System;
using System.Collections.Generic;
using System.Data;
using System.Threading;
using MySql.Data.MySqlClient;
using UnityEngine;

namespace Server.IO.File
{
    public class MySQLDataProvider : IDataProvider
    {
        public MySQLDataProvider()
        {
            try
            {
                _threadSql.Start();
            }
            catch (Exception e)
            {
                Debug.LogException(e);
            }
        }
        private Dictionary<string,string> _cachedValues = new Dictionary<string, string>();

        private static MySqlConnection conSQL;
        private static List<Action> sqlActions = new List<Action>();

        #region thread
        private Thread _threadSql = new Thread(new ThreadStart(() =>
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
                    ExecuteAllSQL();
                }
                Thread.Sleep(500);
                conSQL.Ping();
            }
        }));

        public static void ExecuteAllSQL()
        {
            for (int i = 0; i < sqlActions.Count; i++)
            {
                if(sqlActions[i] != null)
                    sqlActions[i]();
            }
            sqlActions.Clear();
        }

        #endregion

        public void GetData(string dataPath, Action<bool, string> onFinish)
        {
            if (_cachedValues.ContainsKey(dataPath))
            {
                onFinish(true, _cachedValues[dataPath]);
            }
            else
            {
                Action a = () =>
                {
                    if (_cachedValues.ContainsKey(dataPath))
                    {
                        onFinish(true, _cachedValues[dataPath]);
                        return;
                    }

                    
                    MySqlCommand command = conSQL.CreateCommand();
                    command.CommandText = "SELECT v FROM DataServer WHERE i=@i";
                    //command.CommandTimeout = 1;
                    command.Parameters.Add(new MySqlParameter("@i", dataPath));
                    command.ExecuteScalar();
                    
                    MySqlDataAdapter dataAdapter = new MySqlDataAdapter(command);
                    DataSet dataSet = new DataSet();
                    dataAdapter.Fill(dataSet);
                    try
                    {
                        _cachedValues.Add(dataPath, dataSet.Tables[0].Rows[0]["v"].ToString());
                        onFinish(true, dataSet.Tables[0].Rows[0]["v"].ToString());
                    }
                    catch (IndexOutOfRangeException e)
                    {
                        onFinish(false, "");
                    }
                    catch (Exception e)
                    {
                        Debug.LogException(e);
                        onFinish(false, "");
                    }
                };
                lock (sqlActions)
                    sqlActions.Add(a);
            }
        }

        public void SetData(string dataPath, string data, Action<bool> onFinish)
        {
            Action a = () =>
            {
                MySqlCommand command = conSQL.CreateCommand();
                command.CommandText = "INSERT INTO DataServer (i, v) VALUES(@dataPath, @data) ON DUPLICATE KEY UPDATE i=VALUES(i), v=VALUES(v)";
                command.Parameters.Add(new MySqlParameter("@dataPath", dataPath));
                command.Parameters.Add(new MySqlParameter("@data", data));
                command.ExecuteNonQuery();
                MySqlDataAdapter dataAdapter = new MySqlDataAdapter(command);
                DataSet dataSet = new DataSet();
                dataAdapter.Fill(dataSet);

                try
                {
                    _cachedValues[dataPath] = data;
                }
                catch (KeyNotFoundException e)
                {
                    _cachedValues.Add(dataPath, data);
                }

                onFinish(true);
            };
            lock (sqlActions)
            sqlActions.Add(a);
        }

        public void AppendData(string dataPath, string data, Action<bool> onFinish)
        {
            GetData(dataPath, (b, s) =>
            {
                if (b)
                    SetData(dataPath, s + data, b1 => onFinish(true));
                else
                    SetData(dataPath, data, b1 => onFinish(true));
            });
        }

        public void ReplaceData(string dataPath, string oldValue, string newValue, Action<bool> onFinish)
        {
            GetData(dataPath, (b, s) =>
            {
                if (b)
                {
                    string ns = s.Replace(oldValue, newValue);
                    SetData(dataPath, ns, onFinish);
                }
                else
                {
                    onFinish(false);
                }
            });
        }
    }
}
