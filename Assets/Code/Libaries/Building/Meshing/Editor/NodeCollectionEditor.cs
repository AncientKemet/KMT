using Game.Code.Scripts.Meshing;
using UnityEditor;
using UnityEngine;

namespace Assets.Game.Code.Scripts.Meshing.Editor
{
    [CustomEditor(typeof(NodeCollection))]
    public class NodeCollectionEditor : UnityEditor.Editor
    {
        private bool FoldOut = false;

        protected override void OnHeaderGUI()
        {
        }

        public override void OnInspectorGUI()
        {
            BeginHeader("NodeCollection");
            {
                Bool("DisplayBasicInspector", ref FoldOut);

                if (FoldOut)
                    base.OnInspectorGUI();

                EditorGUILayout.BeginHorizontal();
                {
                    DrawButton("Add", delegate
                    {
                        ((NodeCollection)target).AddNode();
                    });
                    
                    DrawButton("Sort", delegate
                    {
                        ((NodeCollection)target).Sort();
                    });
                }
                EditorGUILayout.EndHorizontal();

            }
            EndHeader();

            EditorGUILayout.BeginHorizontal();
            {
                DrawButton("Create Walls", delegate
                {
                    //NotYetDone
                });
            }
            EditorGUILayout.EndHorizontal();
        }

        void OnSceneGUI()
        {
            foreach (var n in ((NodeCollection)target).Nodes)
            {
                Handles.Label(n.transform.position, "" + n.Index);
            }
            
        }

        void DrawHeaderLabel(string name)
        {
            GUILayout.Label(name, EditorStyles.boldLabel);
        }

        void Label(string name)
        {
            GUILayout.Label(name, EditorStyles.miniLabel);
        }

        void Bool(string text, ref bool b)
        {
            b = EditorGUILayout.Toggle(text, b);
        }


        void BeginHeader(string name)
        {
            DrawHeaderLabel(name);
            GUILayout.Space(-1);
            EditorGUI.indentLevel++;
            GUILayout.BeginVertical("box");
        }

        private void DrawButton(string text, System.Action onAction)
        {
            if (GUILayout.Button(text))
            {
                if (onAction != null)
                    onAction();
            }
        }

        private void EndHeader()
        {
            GUILayout.EndVertical();
            EditorGUI.indentLevel--;
        }
    }
}
