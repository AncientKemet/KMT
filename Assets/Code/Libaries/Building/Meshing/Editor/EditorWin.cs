
using System.Collections.Generic;
using UnityEditor;

namespace Assets.Game.Code.Scripts.Meshing.Editor
{
    public class EditorWin : EditorWindow 
    {
        public List<GUIObject> Objects = new List<GUIObject>();

        public virtual void OnGUI()
        {
            foreach (var o in Objects)
            {
                o.OnGUI();
            }
        }
    }

    public abstract class GUIObject
    {

        public List<GUIObject> Objects = new List<GUIObject>();

        public virtual void OnGUI()
        {
            BeginGUI();
            foreach (var o in Objects)
            {
                o.OnGUI();
            }
            EndGUI();
        }

        protected abstract void BeginGUI();
        protected abstract void EndGUI();
    }

    public class GUIPanel : GUIObject
    {
        public enum TYPE
        {
            NONE,
            BOX
        }

        public TYPE Type { get; set; }

        protected override void BeginGUI()
        {
            if (Type == TYPE.BOX)
                EditorGUILayout.BeginHorizontal("box");
            else
                EditorGUILayout.BeginHorizontal();
        }

        protected override void EndGUI()
        {
            EditorGUILayout.EndVertical();
        }
    }
}
