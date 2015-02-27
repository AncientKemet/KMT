using System;

#if UNITY_EDITOR
using UnityEditor;
#endif

using UnityEngine;

namespace Game.Code.Scripts.Meshing
{
    [ExecuteInEditMode]
    public class Node : MonoBehaviour, ISalesmanNode
    {
        [SerializeField]
        [Range(1,100)]
        private float Roundf = 20f;

        private int _index;

        public int Index
        {
            get { return _index; }
            set
            {
                _index = value;
                name = "Node" + _index;
            }
        }

        /*private void Update()
        {
            Vector3 pos = transform.localPosition;

            pos.x = (float)((int)(pos.x * Roundf)) / Roundf;
            pos.y = (float)((int)(pos.y * Roundf)) / Roundf;
            pos.z = (float)((int)(pos.z * Roundf)) / Roundf;

            transform.localPosition = pos;
        }*/

        void OnDrawGizmos()
        {
            Gizmos.color = new Color(0,0,0);
            Gizmos.DrawCube(transform.position, Vector3.one / Roundf);
        }

 #region SalesManProblemImplementation
        public Vector3 Position { get { return transform.position; }}
#endregion
    }
}
