using System.Collections.Generic;
using System.IO;
using System.Linq;
using Code.Core.Shared.Content;
using UnityEditor;
using UnityEngine;

namespace Assets.Editor
{
    /// <summary>
    /// Makes sure that all server cs files are marcoed  
    /// </summary>
    public class OldBloodEditor
    {


        [MenuItem("OldBlood/Fix GUIDs")]
        private static void FixGUIDs()
        {
            foreach (var v in GameObject.FindObjectsOfType<ContentItem>())
            {
                if(v.GUID ==null)
                { }
            }
        }

        private static string BeginFileMacro = "#if SERVER";
        private static string EndFileMacro = "#endif";

        [MenuItem("OldBlood/ServerFiles")]
        private static void Do()
        {
            List<string> Directories = new List<string>();
            List<string> Files = new List<string>();
            GetDirectories(Application.dataPath+"/Code/Core/Server", Directories);
        
            foreach (var dir in Directories)
            {
                GetCss(dir, Files);
            }

            foreach (var file in Files)
            {
                EnsureMacro(file);
            }
        }

        private static void EnsureMacro(string file)
        {
            string[] lines = File.ReadAllLines(file);

            string firstLine = lines.First();
            string lastLine = lines.Last();

            if (firstLine != BeginFileMacro)
            {
                for (int i = 0; i < lines.Length; i++)
                {
                    if (lines[i].Contains(BeginFileMacro))
                    {
                        lines[i] = null;
                    }
                }
                List<string> newLines = new List<string>(lines);
                
                newLines.Insert(0, BeginFileMacro);
                lines = newLines.ToArray();
                File.WriteAllLines(file, lines);
            }

            if (lastLine != EndFileMacro)
            {
                for (int i = 0; i < lines.Length; i++)
                {
                    if (lines[i].Contains(EndFileMacro))
                    {
                        lines[i] = null;
                    }
                }
                List<string> newLines = new List<string>(lines);
                newLines.Add(EndFileMacro);
                lines = newLines.ToArray();
                File.WriteAllLines(file, lines);
            }
        }

        private static void GetDirectories(string path, List<string> all)
        {
            string[] dirs = Directory.GetDirectories(path, "*");
            all.AddRange(dirs);

            foreach (var directory in dirs)
            {
                GetDirectories(directory, all);
            }
        }

        private static void GetCss(string path, List<string> all)
        {
            string[] files = Directory.GetFiles(path, "*.cs");
            all.AddRange(files);
        }
    }
}
