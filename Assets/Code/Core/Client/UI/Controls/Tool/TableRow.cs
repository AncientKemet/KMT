using UnityEngine;

namespace Client.UI.Controls.Tool
{
    [ExecuteInEditMode]
    public class TableRow : MonoBehaviour
    {
        public tk2dSpriteCollectionData collection;
        public float width = 2f;
        public float cellHeight;

        public Vector3 offset;

        public bool Build = false;

        private int _amountOfCells;

        public TableCell this[int index]
        {
            get
            {
                return transform.GetChild(index).GetComponent<TableCell>();
            }
            set
            {
                if (value == null)
                {
                    if (transform.GetChild(index) != null)
                    {
                        Destroy(transform.GetChild(index).gameObject);
                    }
                    return;
                }
                value.transform.parent = transform;
                value.transform.SetSiblingIndex(index);
            }
        }

        public int cells { get; set; }

#if UNITY_EDITOR
        private void Update()
        {
            if (Build)
            {
                Build = false;
                foreach (var cell in GetComponentsInChildren<TableCell>())
                {
                    DestroyImmediate(cell.gameObject);
                }
                for (int i = 0; i < cells; i++)
                {
                    TableCell cell = new GameObject("cell").AddComponent<TableCell>();
                    cell.transform.parent = transform;
                    cell.transform.localPosition = new Vector3(0, -(i * cellHeight + offset.y));
                    cell.height = cellHeight;
                    cell.width = width;
                    cell.collection = collection;
                    cell.Build = true;
                }
            }
        }
#endif
    }
}