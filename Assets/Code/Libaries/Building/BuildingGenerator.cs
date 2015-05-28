using System.Collections.Generic;
using UnityEngine;

namespace Code.Libaries.Building
{
    [ExecuteInEditMode]
    [RequireComponent(typeof(MeshFilter))]
    public class BuildingGenerator : MonoBehaviour
    {
        private Mesh _mesh;

        public Mesh mesh
        {
            get { return _mesh; }
            set { _mesh = value; meshFilter.mesh = value; }
        }
        private MeshFilter _meshFilter;

        public MeshFilter meshFilter
        {
            get 
            {
                if(_meshFilter == null)
                {
                    _meshFilter = GetComponent<MeshFilter>();
                }
                return _meshFilter;
            }
        }

        [SerializeField]
        private bool _forceBuild = false;
        [SerializeField]
        private bool _clean = false;

        [SerializeField]
        private float WallHeight = 1f;

        [SerializeField] private MeshRenderer _middle, _edge;

        private List<BuildingWall> _walls = null;

        public List<BuildingNode> nodes = new List<BuildingNode>();

        public List<BuildingWall> walls
        {
            get
            {
                if (_walls == null)
                {
                    _walls = new List<BuildingWall>();
                    BuildingWall w = new BuildingWall();
                    foreach (var buildingNode in nodes)
                    {
                        if (w.node0 == null)
                            w.node0 = buildingNode;
                        else if (w.node1 == null)
                        {
                            w.node1 = buildingNode;
                            _walls.Add(w);
                           
                            w = new BuildingWall();
                            w.node0 = buildingNode;
                        }
                    }
                }
                return _walls;
            }
        }

#if UNITY_EDITOR
        public void Update()
#else
        public void FixedUpdate()
#endif
        {
            if (_forceBuild)
            {
                BuildBuilding();
            }

#if UNITY_EDITOR 
            nodes.Clear();
            nodes.AddRange(GetComponentsInChildren<BuildingNode>());
            if (_clean)
            {
                _clean = false;
                nodes.Clear();
                foreach (var r in GetComponentsInChildren<BuildingNode>())
                {
                    DestroyImmediate(r.gameObject);
                }
            }
#endif
        }

        public void ForceBuild()
        {
            _forceBuild = true;
        }

        private void BuildBuilding()
        {
            _forceBuild = false;
            _walls = null;

            //destroy old s
            foreach (var r in GetComponentsInChildren<MeshRenderer>())
            {
                DestroyImmediate(r.gameObject);
            }


            if (walls == null)
            {
                Debug.LogError("Error null walls.");
                return;
            }

            int index = 0;
            foreach (var node in nodes)
            {
                MeshRenderer eRenderer = ((GameObject) Instantiate(_edge.gameObject)).GetComponent<MeshRenderer>();
                eRenderer.transform.position = node.transform.position;
                eRenderer.transform.parent = transform;
                if (index < nodes.Count-1)
                {
                    MeshRenderer wRenderer = ((GameObject)Instantiate(_middle.gameObject)).GetComponent<MeshRenderer>();
                    wRenderer.transform.position = node.transform.position;
                    wRenderer.transform.LookAt(nodes[index + 1].transform.position);
                    wRenderer.transform.eulerAngles+= new Vector3(-90, 90, 0);
                    wRenderer.transform.localScale = new Vector3(Vector3.Distance(node.transform.position, nodes[index + 1].transform.position),1,1);
                    eRenderer.transform.LookAt(nodes[index + 1].transform.position, Vector3.forward);
                    eRenderer.transform.eulerAngles += new Vector3(-90, 90, 0);
                    wRenderer.transform.parent = transform;
                }
                index++;
            }
        }

        private void OnDrawGizmos()
        {
            foreach (var item in walls)
            {
                if(item != null)
                    if(item.node1 != null && item.node0 != null)
                    Gizmos.DrawLine(item.node0.transform.position, item.node1.transform.position);
            }
        }

    }

}
