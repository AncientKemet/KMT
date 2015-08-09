using System;
using System.Collections.Generic;
using Client.Units;
using Code.Libaries.Generic.Trees;
using Server.Model.Entities;
using UnityEditor;
using UnityEngine;

namespace Client.Enviroment
{
    /// <summary>
    /// Quad tree.
    /// </summary>
    [ExecuteInEditMode]
    public class MapQuadTree : MonoBehaviour
    {
        [SerializeField]
        private bool _build = false;
        [SerializeField]
        [Range(1, 8)] public int _divisions = 0;
        private bool _wasInitialized = false;
        [SerializeField]
        private MapQuadTree[] children = null;
        [SerializeField]
        private MapQuadTree parent = null;
        [SerializeField]
        private LinkedList<PrefabInstance> _objects;
        [SerializeField] public Vector2 _position;
        [SerializeField] public Vector2 _size;
        private MapQuadTree _root = null;
        private MapQuadTree _north = null;
        private List<PrefabInstance> _objectsVisible = null;

        /// <summary>
        /// Returns a list of all visible objects, this includes active and static objects.
        /// </summary>
        public List<PrefabInstance> ObjectsVisible
        {
            get
            {
                if (_objectsVisible == null)
                {
                    if (!IsDivided)
                    {
                        _objectsVisible = new List<PrefabInstance>(AmountOfObjects);
                        if (AmountOfObjects > 0)
                        {
                            ObjectsVisible.AddRange(_objects);
                        }
                        if (North != null)
                        {
                            if (North.AmountOfObjects > 0)
                            {
                                ObjectsVisible.AddRange(North._objects);
                            }
                        }
                        if (NorthEast != null)
                        {
                            if (NorthEast.AmountOfObjects > 0)
                            {
                                ObjectsVisible.AddRange(NorthEast._objects);
                            }
                        }
                        if (East != null)
                        {
                            if (East.AmountOfObjects > 0)
                            {
                                ObjectsVisible.AddRange(East._objects);
                            }
                        }
                        if (SouthEast != null)
                        {
                            if (SouthEast.AmountOfObjects > 0)
                            {
                                ObjectsVisible.AddRange(SouthEast._objects);
                            }
                        }
                        if (South != null)
                        {
                            if (South.AmountOfObjects > 0)
                            {
                                ObjectsVisible.AddRange(South._objects);
                            }
                        }
                        if (SouthWest != null)
                        {
                            if (SouthWest.AmountOfObjects > 0)
                            {
                                ObjectsVisible.AddRange(SouthWest._objects);
                            }
                        }
                        if (West != null)
                        {
                            if (West.AmountOfObjects > 0)
                            {
                                ObjectsVisible.AddRange(West._objects);
                            }
                        }
                        if (NorthWest != null)
                        {
                            if (NorthWest.AmountOfObjects > 0)
                            {
                                ObjectsVisible.AddRange(NorthWest._objects);
                            }
                        }
                    }
                    if (IsDivided)
                    {
                        _objectsVisible = new List<PrefabInstance>(AmountOfObjects);
                        foreach (var child in children)
                        {
                            _objectsVisible.AddRange(child.ObjectsVisible);
                        }
                    }
                }
                return _objectsVisible;
            }
        }

        public MapQuadTree North
        {
            get
            {
                if (_north == null)
                {
                    GetBranchNeightbour(ref _north, new Vector2(_size.x * 0.5f, _size.y * 1.5f));
                }
                return _north;
            }
        }

        private MapQuadTree _northEast = null;

        public MapQuadTree NorthEast
        {
            get
            {
                if (_northEast == null)
                {
                    GetBranchNeightbour(ref _northEast, new Vector2(_size.x * 1.5f, _size.y * 1.5f));
                }
                return _northEast;
            }
        }

        private MapQuadTree _east = null;

        public MapQuadTree East
        {
            get
            {
                if (_east == null)
                {
                    GetBranchNeightbour(ref _east, new Vector2(_size.x * 1.5f, _size.y * 0.5f));
                }
                return _east;
            }
        }

        private MapQuadTree _southEast = null;

        public MapQuadTree SouthEast
        {
            get
            {
                if (_southEast == null)
                {
                    GetBranchNeightbour(ref _southEast, new Vector2(_size.x * 1.5f, -_size.y * 0.5f));
                }
                return _southEast;
            }
        }

        private MapQuadTree _south = null;

        public MapQuadTree South
        {
            get
            {
                if (_south == null)
                {
                    GetBranchNeightbour(ref _south, new Vector2(_size.x * 0.5f, -_size.y * 0.5f));
                }
                return _south;
            }
        }

        private MapQuadTree _southWest = null;

        public MapQuadTree SouthWest
        {
            get
            {
                if (_southWest == null)
                {
                    GetBranchNeightbour(ref _southWest, new Vector2(-_size.x * 0.5f, -_size.y * 0.5f));
                }
                return _southWest;
            }
        }

        private MapQuadTree _west = null;

        public MapQuadTree West
        {
            get
            {
                if (_west == null)
                {
                    GetBranchNeightbour(ref _west, new Vector2(-_size.x * 0.5f, _size.y * 0.5f));
                }
                return _west;
            }
        }

        private MapQuadTree _northWest = null;
        private int _amountOfObjects = 0;

        public MapQuadTree NorthWest
        {
            get
            {
                if (_northWest == null)
                {
                    GetBranchNeightbour(ref _northWest, new Vector2(-_size.x * 0.5f, _size.y * 1.5f));
                }
                return _northWest;
            }
        }

        public MapQuadTree Root
        {
            get
            {
                if (_root == null)
                {
                    if (parent == null)
                    {
                        _root = this;
                        return _root;
                    }

                    MapQuadTree par = parent;
                    for (int i = 0; i < 10; i++)
                    {
                        if (par.parent == null)
                        {
                            _root = par;
                            break;
                        }
                        else
                            par = par.parent;
                    }
                }
                return _root;
            }
        }

        public bool IsDivided { get { return _divisions > 0; } }

        void GetBranchNeightbour(ref MapQuadTree _ref, Vector2 offset)
        {
            //As root shouldn't have any neightbours
            if (Root != this)
            {
                _ref = Root.GetTreeFor(_position + offset, _divisions);
            }
        }

        void OnDrawGizmos()
        {
            Gizmos.color = IsDivided ? Color.grey : Color.red;
            Gizmos.DrawWireCube(transform.position + new Vector3(_size.x, 0, _size.y) / 2f, new Vector3(_size.x, 0, _size.y));
        }

        /// <summary>
        /// Returns amount of all objects under this quad tree.
        /// </summary>
        public int AmountOfObjects
        {
            get
            {
                return _amountOfObjects;
            }
        }


        /// <summary>
        /// Is called whenever the amount should be changed, it changes amount of all parents aswell.
        /// </summary>
        /// <param name="change"></param>
        private void AmountOfObjectsChange(int change)
        {
            MapQuadTree p = parent;
            _amountOfObjects += change;
            while (p != null)
            {
                p._amountOfObjects += change;
                p = p.parent;
            }
        }

        private void Update()
        {
            //In edit mode we only look for prefabinstances
            if (!Application.isPlaying)
            {
                for (int i = 0; i < transform.childCount; i++)
                {
                    var child = transform.GetChild(i);
                    MonoBehaviour ins = child.GetComponent<PrefabInstance>();
                    if(ins == null)
                        ins = child.GetComponent<RelationShip>();
                    if (child.gameObject != Selection.activeGameObject)
                    {
                        if (ins != null)
                        {
                            Vector2 pos = new Vector2(ins.transform.position.x, ins.transform.position.z);
                            if (IsDivided)
                            {
                                ins.transform.parent = Root.GetTreeFor(pos).transform;
                            }
                            else
                            {
                                if (!ContainsPoint(pos))
                                    ins.transform.parent = Root.GetTreeFor(pos).transform;
                            }
                        }
                        else if (child.GetComponent<MapQuadTree>() == null)
                            child.transform.parent = null;
                    }
                }
            }
            else //In play mode we only look for playerunits
            {
                if (!IsDivided || Root == this)
                for (int i = 0; i < transform.childCount; i++)
                {
                    var child = transform.GetChild(i);
                    var unit = child.GetComponent<PlayerUnit>();

                    if (unit != null)
                    {
                        Vector2 pos = new Vector2(unit.transform.position.x, unit.transform.position.z);
                        if (Root == this)
                        {
                            unit.transform.parent = Root.GetTreeFor(pos).transform;
                        }
                        else if(!IsDivided)
                        {
                            if (!ContainsPoint(pos))
                            {
                                var branch = Root.GetTreeFor(pos);
                                if (branch != null)
                                    unit.transform.parent = branch.transform;
                            }
                        }
                    }
                }

            }
        }
#if UNITY_EDITOR
        private void OnRenderObject()
        {
            if (!Application.isPlaying)
            {
                if (parent == null)
                {
                    if (_build)
                    {
                        _build = false;
                        Build();
                    }
                }

                if (IsDivided)
                    if (SceneView.lastActiveSceneView != null)
                    {
                        List<Vector2> positions = new List<Vector2>();

                        Vector2 camera = new Vector2(SceneView.lastActiveSceneView.camera.transform.position.x,
                                                     SceneView.lastActiveSceneView.camera.transform.position.z);
                        positions.Add(camera);

                        for (int x = -1; x < 2; x++)
                            for (int y = -1; y < 2; y++)
                            {
                                if(y == 0 && x == 0)
                                    continue;
                                float stepx = Root._size.x / 32;
                                float stepz = Root._size.y / 32;
                                var vec = new Vector2(x * stepx, y * stepz);
                                positions.Add(camera + vec);
                            }

                        foreach (var child in children)
                        {
                            child.gameObject.SetActive(child.ContainsPoint(positions));
                        }
                    }
            }
            else
            {
                if (IsDivided)
                    if (Camera.main != null)
                    {
                        List<Vector2> positions = new List<Vector2>();

                        Vector2 camera = new Vector2(Camera.main.transform.position.x,
                                                     Camera.main.transform.position.z);
                        positions.Add(camera);

                        for (int x = -1; x < 2; x++)
                            for (int y = -1; y < 2; y++)
                            {
                                float stepx = Root._size.x / 32;
                                float stepz = Root._size.y / 32;
                                var vec = new Vector2(x * stepx, y * stepz);
                                positions.Add(camera + vec);
                            }

                        foreach (var child in children)
                        {
                            child.gameObject.SetActive(child.ContainsPoint(positions));
                        }
                    }
            }
        }
#endif

        private void Build()
        {
            if (Application.isPlaying)
            {
                throw new Exception("Error trying to rebuild while playing.");
            }

            if (transform.parent != null)
                if (parent == null || parent != transform.parent.GetComponent<MapQuadTree>())
                    parent = transform.parent.GetComponent<MapQuadTree>();

            if (parent != null)
            {
                _size = parent._size / 2f;
                _divisions = parent._divisions - 1;
            }

            if (IsDivided)
            {
                Vector2 _childSize = _size / 2f;
                foreach (var child in GetComponentsInChildren<PrefabInstance>())
                {
                    if (!Application.isPlaying)
                    {
                        child.transform.parent = Root.transform;
                    }
                }
                if (children != null)
                    foreach (var child in children)
                    {
                        if (child != null)
                            DestroyImmediate(child.gameObject);
                    }
                children = new MapQuadTree[]
                {
                    CreateChild(_position + new Vector2(0, 0),_childSize),
                    CreateChild(_position + new Vector2(_childSize.x, 0),_childSize),
                    CreateChild(_position + new Vector2(0, _childSize.y),_childSize),
                    CreateChild(_position + new Vector2(_childSize.x, _childSize.y),_childSize) 
                };
                foreach (var child in children)
                {
                    child.parent = this;
                }
            }
        }

        private MapQuadTree CreateChild(Vector2 p0, Vector2 childSize)
        {
            GameObject go = new GameObject(p0.ToString());
            var branch = go.AddComponent<MapQuadTree>();

            branch.transform.parent = transform;
            branch.transform.position = new Vector3(p0.x, 0, p0.y);

            branch._size = childSize;
            branch._position = p0;
            branch.Build();
            return branch;
        }

        public MapQuadTree GetTreeFor(Vector2 pos)
        {
            return GetTreeFor(pos, 0);
        }

        public MapQuadTree GetTreeFor(Vector2 pos, int division)
        {
            if (ContainsPoint(pos))
            {
                if (_divisions == division)
                {
                    return this;
                }
                else
                {
                    MapQuadTree tree;
                    foreach (var child in children)
                    {
                        tree = child.GetTreeFor(pos, division);
                        if (tree != null)
                            return tree;
                    }
                    return null;
                }
            }
            else
                return null;
        }

        private bool ContainsPoint(Vector2 point)
        {
            Vector3 longBoundary = _position + _size;
            if (_position.x <= point.x && _position.y <= point.y && longBoundary.x > point.x && longBoundary.y > point.y)
                return true;
            return false;
        }


        private bool ContainsPoint(List<Vector2> positions)
        {
            for (int i = 0; i < positions.Count; i++)
            {
                if (ContainsPoint(positions[i]))
                    return true;
            }
            return false;
        }


    }
}

