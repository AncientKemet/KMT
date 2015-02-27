using System.Collections.Generic;
using UnityEngine;

namespace Client.UI.Controls.Tool
{
    [ExecuteInEditMode]
    public class Table : MonoBehaviour
    {
        public tk2dSpriteCollectionData collection;
        public Vector3 offset;
        public int rows = 1;
        public int cells = 1;
        public float rowWidth = 1f;
        public float cellHeight = 0.2f;
        public bool Build = false;

        public TableRow this[int index]
        {
            get { return transform.GetChild(index).GetComponent<TableRow>(); }
        }

        private void Update()
        {

#if UNITY_EDITOR
            if (Build)
            {
                Build = false;
                
                foreach (var row in GetComponentsInChildren<TableRow>())
                {
                    DestroyImmediate(row.gameObject);
                }

                for (int i = 0; i < rows; i++)
                {
                    TableRow row = new GameObject("row" + i).AddComponent<TableRow>();
                    row.transform.parent = transform;
                    row.transform.localPosition = new Vector3(i * (rowWidth + offset.x), 0);
                    row.width = rowWidth;
                    row.cells = cells;
                    row.cellHeight = cellHeight;
                    row.collection = collection;
                    row.Build = true;
                }
                
            }
#endif
        }

        
    }
}
