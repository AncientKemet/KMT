using Server;
using UnityEditor;
using UnityEngine;
using System.Collections;

[InitializeOnLoad]
class DoInEditor
{
    static DoInEditor ()
    {
        EditorApplication.update += Update;
    }

    static void Update ()
    {
        if (!Application.isPlaying)
        {
            if (!ServerSingleton.IsNull)
            {

            }
        }
    }
}
