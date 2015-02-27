using System;
using UnityEditor;
using UnityEngine;

namespace Code.Libaries.UnityExtensions.Editor
{
    public static class BloodGUI
    {

        public static void Button(string text, Action action)
        {
            if(GUILayout.Button(text))
            {
                if (action != null)
                {
                    action();
                }
            }
        }

        public static Vector3 Vector(string text, Vector3 vector3)
        {
            return EditorGUILayout.Vector3Field(text, vector3);
        }
    }
}

