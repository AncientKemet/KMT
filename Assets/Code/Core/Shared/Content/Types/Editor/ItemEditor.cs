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

        if(Target.gameObject.layer == 9)
        if (GUILayout.Button("Set Icon Rotation"))
        {
            Debug.Log(ContentManager.I.Items.Count+" / "+Target.InContentManagerIndex);
            ContentManager.I.Items[Target.InContentManagerIndex].Position = Target.transform.localPosition;
            ContentManager.I.Items[Target.InContentManagerIndex].Rotation = Target.transform.localEulerAngles;
            ContentManager.I.Items[Target.InContentManagerIndex].Scale = Target.transform.localScale;
        }
    }
}
