using Assets.Editor;
using Code.Core.Shared.Content.Types;
using Code.Libaries.Generic.Managers;
using Shared.Content.Types;
using UnityEditor;
using UnityEngine;

namespace Assets.Code.Core.Shared.Content.Types.Editor
{
    [CustomEditor(typeof(NpcPrefab))]
    public class NPCPrefabEditor : UnityEditor.Editor
    {

        private NpcPrefab t { get { return target as NpcPrefab; } }

        public override void OnInspectorGUI()
        {
            t.NPC_ID = EditorGUILayout.IntField("NPC Identifier", t.NPC_ID);
        }
    }
}
