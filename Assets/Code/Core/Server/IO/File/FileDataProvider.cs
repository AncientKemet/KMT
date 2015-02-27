#if SERVER
using System;
using System.IO;
using System.Threading;
using UnityEngine;

namespace Server.IO.File
{
    public class FileDataProvider : IDataProvider
    {

        private string _rootDirectory;

        public FileDataProvider()
        {
            ServerSingleton.Instance.DataRootPath = System.Environment.GetEnvironmentVariable("USERPROFILE")+"\\Ancient Kemet\\Server\\";
            _rootDirectory = ServerSingleton.Instance.DataRootPath;

            //Create the root Directory if it doesnt exist 
            if(!Directory.Exists(_rootDirectory))
                Directory.CreateDirectory(_rootDirectory);

            SetData("user/Test/Password", "test", b => {});
        }

        public void GetData(string dataPath, Action<bool, string> onFinish)
        {
            ServerSingleton.StuffToRunOnUnityThread.Add(new Action(() =>
            {
                try
                {
                    string text = System.IO.File.ReadAllText(_rootDirectory + dataPath.Replace("/", "\\")+".txt");
                    onFinish(true, text);
                }
                catch (DirectoryNotFoundException e)
                {
                    onFinish(false, "");
                }
                catch (FileNotFoundException e)
                {
                    onFinish(false, "");
                }
            }));
            
        }

        public void SetData(string dataPath, string data, Action<bool> onFinish)
        {
            ServerSingleton.StuffToRunOnUnityThread.Add(new Action(() =>
            {
                try
                {
                    string path = _rootDirectory + dataPath.Replace("/", "\\") + ".txt";
                    string[] splitdirectorypath = path.Split("\\"[0]);
                    string directoryPath = "";
                    for (int i = 0; i < splitdirectorypath.Length -1; i++)
                    {
                        directoryPath += splitdirectorypath[i]+"\\";
                    }

                    //Create the Directory if it doesnt exist 
                    if (!Directory.Exists(directoryPath))
                        Directory.CreateDirectory(directoryPath);

                    System.IO.File.WriteAllText(path, data);

                    onFinish(true);
                }
                catch (DirectoryNotFoundException e)
                {
                    Debug.LogException(e);
                    onFinish(false);
                }
                catch (FileNotFoundException e)
                {
                    Debug.LogException(e);
                    onFinish(false);
                }
            }));
        }

        public void AppendData(string dataPath, string data, Action<bool> onFinish)
        {
            GetData(dataPath, (b, s) => SetData(dataPath, s + data, onFinish));
        }

        public void ReplaceData(string dataPath, string oldValue, string newValue, Action<bool> onFinish)
        {

            GetData(dataPath, (b, s) =>
            {
                string ns = s.Replace(oldValue, newValue);
                SetData(dataPath, ns, onFinish);
            });
        }
    }
}
#endif
