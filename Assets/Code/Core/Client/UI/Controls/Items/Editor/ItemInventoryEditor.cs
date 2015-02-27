using System.Collections.Generic;
using Code.Libaries.Generic.Managers;
using Code.Libaries.UnityExtensions.Editor;
using UnityEditor;
using UnityEngine;

namespace Code.Core.Client.UI.Controls.Items.Editor
{
    [CustomEditor(typeof(ItemInventory))]
    public class ItemInventoryEditor : UnityEditor.Editor
    {

        private ItemInventory t
        {
            get
            {
                return target as ItemInventory;
            }
        }

        public override void OnInspectorGUI()
        {
            if(t == null)
                return;

            BloodGUI.Button("Rebuild", delegate { t.ForceRebuild(); });

            base.OnInspectorGUI();
        }
    }
}
