using Code.Core.Shared.Content.Types;
using Code.Libaries.Generic.Managers;
using Shared.Content.Types;
using UnityEditor;
using UnityEngine;
using System.Collections;

[CustomEditor(typeof(Item))]
public class ItemEditor : Editor 
{

    private Item Target { get { return (Item) target; } }

    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();
    }
}
