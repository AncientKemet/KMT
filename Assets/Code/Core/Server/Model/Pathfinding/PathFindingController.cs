#if SERVER
using Code.Libaries.Generic;
using Pathfinding;
using UnityEngine;

namespace Server.Model.Pathfinding
{
    [RequireComponent(typeof(AstarPath))]
    [ExecuteInEditMode]
    public class PathFindingController : MonoSingleton<PathFindingController>
    {

        [SerializeField] private AstarPath _astarPath;

        public int Width = 8;
        public int Height = 8;

        public float nodeSize = 1f;
        public int GridSize = 64;

        public bool UpdateRequired = false;

        void Start()
        {
            //UpdateRequired = true;
        }

        void Update()
        {
            if (_astarPath == null)
            {
                _astarPath = GetComponent<AstarPath>();
            }

            if (UpdateRequired)
            {
                UpdateRequired = false;
                ForceBuild();
            }
        }

        private void ForceBuild()
        {
            _astarPath.graphs = new NavGraph[Width * Height];

            int x = 0;
            int z = 0;

            for (int i = 0; i < _astarPath.graphs.Length; i++)
            {
                var graph = new GridGraph();

                graph.Width = GridSize;
                graph.Depth = GridSize;

                graph.maxClimb = 200;
                graph.maxSlope = 30f;

                graph.nodeSize = nodeSize;
                graph.center = new Vector3(x * GridSize + GridSize / 2f * nodeSize - x, 0, z * GridSize + GridSize / 2f * nodeSize);

                graph.UpdateSizeFromWidthDepth();

                graph.active = _astarPath;

                _astarPath.graphs[i] = graph;

                x++;
                if (x == Width)
                {
                    x = 0;
                    z++;
                }
            }
            
            _astarPath.Scan();
        }
    }
}
#endif
