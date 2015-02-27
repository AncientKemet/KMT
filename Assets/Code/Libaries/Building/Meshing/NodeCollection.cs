using System.Collections.Generic;
using System.Linq;

#if UNITY_EDITOR
using UnityEditor;
#endif

using UnityEngine;

namespace Game.Code.Scripts.Meshing
{
    [ExecuteInEditMode]
    [RequireComponent(typeof(MeshFilter))]
    [RequireComponent(typeof(MeshRenderer))]
#if UNITY_EDITOR
    [DrawGizmo (GizmoType.SelectedOrChild)]
#endif
    public class NodeCollection : MonoBehaviour
    {
        [SerializeField]
        private bool _triangulate;
        [SerializeField]
        private List<Node> _nodes;

        private List<Node> _cachedNodes; 

        private bool _recreateMesh = false;

        public List<Node> Nodes
        {
            get { return _nodes; }
        }

        public Node AddNode()
        {
            Node n = new GameObject("newNode").AddComponent<Node>();

            n.transform.position = transform.position;
            n.transform.parent = transform;
            
            Nodes.Add(n);
            
            return n;
        }

        private void Update()
        {

#if UNITY_EDITOR
            _nodes = new List<Node>(GetComponentsInChildren<Node>());
#endif

            if(_cachedNodes == null)  _cachedNodes = new List<Node>();

            if (_cachedNodes.Count != Nodes.Count)
            {
                _recreateMesh = true;
            }
            else
            {
                for (int i = 0; i < _cachedNodes.Count; i++)
                {
                    if (_cachedNodes[i] == Nodes[i]) continue;
                    _recreateMesh = true;
                    break;
                }
            }
            
            if (_recreateMesh)
            {   
                Sort();
                _cachedNodes = new List<Node>(Nodes.Count);

                for (int i = 0; i < Nodes.Count; i++)
                {
                    _cachedNodes.Add(Nodes[i]);
                    Nodes[i].Index = i;
                }

                GetComponent<MeshFilter>().mesh = CreateMesh(Nodes);
            }
        }

        private Mesh CreateMesh(List<Node> nodes)
        {
            if (nodes.Count < 3)
                return null;

            Triangulator triangulator;
            Mesh mesh = new Mesh();

            List<Vector3> vertices = new List<Vector3>();
            List<Vector2> UV = new List<Vector2>();
            List<int> triangles = new List<int>();

            int currentNodeIndex = 0;

            //add first two verts
            UV.Add(nodes[currentNodeIndex].transform.localPosition);
            vertices.Add(nodes[currentNodeIndex++].transform.localPosition);
            UV.Add(nodes[currentNodeIndex].transform.localPosition);
            vertices.Add(nodes[currentNodeIndex++].transform.localPosition);

            while (currentNodeIndex+1 <= nodes.Count)
            {
                //creating vertices
                for (int i = 0; i < 1; i++)
                {
                    Vector3 pos = nodes[currentNodeIndex].transform.localPosition;
                    UV.Add(new Vector2(pos.x, pos.y));
                    vertices.Add(nodes[currentNodeIndex++].transform.localPosition);
                }

                //creating triangles
                if (_triangulate)
                {
                    triangulator = new Triangulator(vertices);
                    triangles = new List<int>(triangulator.Triangulate());
                }
                else
                {
                    triangles.Add(currentNodeIndex - 1);
                    triangles.Add(currentNodeIndex - 2);
                    triangles.Add(currentNodeIndex - 3);
                }
            }
            
            mesh.vertices = vertices.ToArray();
            
            mesh.SetTriangles(triangles.ToArray(), 0);
            mesh.uv = UV.ToArray();
            mesh.RecalculateBounds();
            mesh.RecalculateNormals();

            return mesh;
        }

        void OnDrawGizmos()
        {
            Gizmos.color = Color.yellow;
            Vector3 oldVector3 = _nodes.Last().Position;
            foreach (var node in Nodes)
            {
                Gizmos.DrawLine(oldVector3, node.transform.position);
                oldVector3 = node.transform.position;
            }
        }

        public void Sort()
        {
            _recreateMesh = true;
            _nodes = TravelingSalesmanProblem<Node>.Sort(Nodes);
        }
    }
}