#if SERVER
using System.Collections.Generic;
using Server.Model.Entities;
using UnityEngine;

namespace Code.Libaries.Generic.Trees
{
    /// <summary>
    /// Quad tree.
    /// </summary>
    public class QuadTree
    {
        public static bool AllowVisibilityFromTop = true;

        private int _divisions = 0;
        private bool _isDivided = false;
        private bool _wasInitialized = false;
        private QuadTree[] children = null;
        private QuadTree parent = null;
        //Objects that can move
        private LinkedList<IQuadTreeObject> _activeObjects;
        //Objects that can't move
        private LinkedList<IQuadTreeObject> _staticObjects;
        private Vector2 _position, _size;
        private Vector2 longBoundary;
        private Vector2 shortBoundary;
        private QuadTree _root = null;
        private QuadTree _north = null;
        private List<IQuadTreeObject> _objectsVisible = null;
        private List<IQuadTreeObject> _staticObjectsVisible = null;
        private List<IQuadTreeObject> _activeObjectsVisible = null;

        /// <summary>
        /// Returns a list of all visible objects, this includes active and static objects.
        /// </summary>
        public List<IQuadTreeObject> ObjectsVisible
        {
            get
            {
                if (_objectsVisible == null)
                {
                    if (!_isDivided)
                    {
                        _objectsVisible = new List<IQuadTreeObject>(AmountOfObjects);
                        if (AmountOfObjects > 0)
                        {
                            ObjectsVisible.AddRange(_activeObjects);
                            ObjectsVisible.AddRange(_staticObjects);
                        }
                        if (North != null)
                        {
                            if (North.AmountOfObjects > 0)
                            {
                                ObjectsVisible.AddRange(North._activeObjects);
                                ObjectsVisible.AddRange(North._staticObjects);
                            }
                        }
                        if (NorthEast != null)
                        {
                            if (NorthEast.AmountOfObjects > 0)
                            {
                                ObjectsVisible.AddRange(NorthEast._activeObjects);
                                ObjectsVisible.AddRange(NorthEast._staticObjects);
                            }
                        }
                        if (East != null)
                        {
                            if (East.AmountOfObjects > 0)
                            {
                                ObjectsVisible.AddRange(East._activeObjects);
                                ObjectsVisible.AddRange(East._staticObjects);
                            }
                        }
                        if (SouthEast != null)
                        {
                            if (SouthEast.AmountOfObjects > 0)
                            {
                                ObjectsVisible.AddRange(SouthEast._activeObjects);
                                ObjectsVisible.AddRange(SouthEast._staticObjects);
                            }
                        }
                        if (South != null)
                        {
                            if (South.AmountOfObjects > 0)
                            {
                                ObjectsVisible.AddRange(South._activeObjects);
                                ObjectsVisible.AddRange(South._staticObjects);
                            }
                        }
                        if (SouthWest != null)
                        {
                            if (SouthWest.AmountOfObjects > 0)
                            {
                                ObjectsVisible.AddRange(SouthWest._activeObjects);
                                ObjectsVisible.AddRange(SouthWest._staticObjects);
                            }
                        }
                        if (West != null)
                        {
                            if (West.AmountOfObjects > 0)
                            {
                                ObjectsVisible.AddRange(West._activeObjects);
                                ObjectsVisible.AddRange(West._staticObjects);
                            }
                        }
                        if (NorthWest != null)
                        {
                            if (NorthWest.AmountOfObjects > 0)
                            {
                                ObjectsVisible.AddRange(NorthWest._activeObjects);
                                ObjectsVisible.AddRange(NorthWest._staticObjects);
                            }
                        }
                    }
                    if (_isDivided && AllowVisibilityFromTop)
                    {
                        _objectsVisible = new List<IQuadTreeObject>(AmountOfObjects);
                        foreach (var child in children)
                        {
                            _objectsVisible.AddRange(child.ObjectsVisible);
                        }
                    }
                }
                return _objectsVisible;
            }
        }

        /// <summary>
        /// Returns a list of all visible static objects.
        /// </summary>
        public List<IQuadTreeObject> StaticObjectsVisible
        {
            get
            {
                if (_staticObjectsVisible == null)
                {
                    if (!_isDivided)
                    {
                        _staticObjectsVisible = new List<IQuadTreeObject>(32);
                        if (AmountOfObjects > 0)
                            _staticObjectsVisible.AddRange(_staticObjects);
                        if (North != null)
                        {
                            if (North.AmountOfObjects > 0)
                                _staticObjectsVisible.AddRange(North._staticObjects);
                        }
                        if (NorthEast != null)
                        {
                            if (NorthEast.AmountOfObjects > 0)
                                _staticObjectsVisible.AddRange(NorthEast._staticObjects);
                        }
                        if (East != null)
                        {
                            if (East.AmountOfObjects > 0)
                                _staticObjectsVisible.AddRange(East._staticObjects);
                        }
                        if (SouthEast != null)
                        {
                            if (SouthEast.AmountOfObjects > 0)
                                _staticObjectsVisible.AddRange(SouthEast._staticObjects);
                        }
                        if (South != null)
                        {
                            if (South.AmountOfObjects > 0)
                                _staticObjectsVisible.AddRange(South._staticObjects);
                        }
                        if (SouthWest != null)
                        {
                            if (SouthWest.AmountOfObjects > 0)
                                _staticObjectsVisible.AddRange(SouthWest._staticObjects);
                        }
                        if (West != null)
                        {
                            if (West.AmountOfObjects > 0)
                                _staticObjectsVisible.AddRange(West._staticObjects);
                        }
                        if (NorthWest != null)
                        {
                            if (NorthWest.AmountOfObjects > 0)
                                _staticObjectsVisible.AddRange(NorthWest._staticObjects);
                        }
                    }
                    if (_isDivided && AllowVisibilityFromTop)
                    {
                        _staticObjectsVisible = new List<IQuadTreeObject>(AmountOfObjects);
                        foreach (var child in children)
                        {
                            _staticObjectsVisible.AddRange(child.StaticObjectsVisible);
                        }
                    }
                }
                return _staticObjectsVisible;
            }
        }

        public List<IQuadTreeObject> ActiveObjectsVisible
        {
            get
            {
                if (_activeObjectsVisible == null)
                {
                    if (!_isDivided)
                    {
                        _activeObjectsVisible = new List<IQuadTreeObject>(8);
                        if (AmountOfObjects > 0)
                            _activeObjectsVisible.AddRange(_activeObjects);
                        if (North != null)
                        {
                            if (North.AmountOfObjects > 0)
                                _activeObjectsVisible.AddRange(North._activeObjects);
                        }
                        if (NorthEast != null)
                        {
                            if (NorthEast.AmountOfObjects > 0)
                                _activeObjectsVisible.AddRange(NorthEast._activeObjects);
                        }
                        if (East != null)
                        {
                            if (East.AmountOfObjects > 0)
                                _activeObjectsVisible.AddRange(East._activeObjects);
                        }
                        if (SouthEast != null)
                        {
                            if (SouthEast.AmountOfObjects > 0)
                                _activeObjectsVisible.AddRange(SouthEast._activeObjects);
                        }
                        if (South != null)
                        {
                            if (South.AmountOfObjects > 0)
                                _activeObjectsVisible.AddRange(South._activeObjects);
                        }
                        if (SouthWest != null)
                        {
                            if (SouthWest.AmountOfObjects > 0)
                                _activeObjectsVisible.AddRange(SouthWest._activeObjects);
                        }
                        if (West != null)
                        {
                            if (West.AmountOfObjects > 0)
                                _activeObjectsVisible.AddRange(West._activeObjects);
                        }
                        if (NorthWest != null)
                        {
                            if (NorthWest.AmountOfObjects > 0)
                                _activeObjectsVisible.AddRange(NorthWest._activeObjects);
                        }
                    }
                    if (_isDivided && AllowVisibilityFromTop)
                    {
                        _activeObjectsVisible = new List<IQuadTreeObject>(AmountOfObjects);
                        foreach (var child in children)
                        {
                            _activeObjectsVisible.AddRange(child.ActiveObjectsVisible);
                        }
                    }
                }
                return _activeObjectsVisible;
            }
        }

        public QuadTree North
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

        private QuadTree _northEast = null;

        public QuadTree NorthEast
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

        private QuadTree _east = null;

        public QuadTree East
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

        private QuadTree _southEast = null;

        public QuadTree SouthEast
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

        private QuadTree _south = null;

        public QuadTree South
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

        private QuadTree _southWest = null;

        public QuadTree SouthWest
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

        private QuadTree _west = null;

        public QuadTree West
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

        private QuadTree _northWest = null;
        private int _amountOfObjects = 0;

        public QuadTree NorthWest
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

        public QuadTree Root
        {
            get
            {
                if (_root == null)
                {
                    if (parent == null)
                    {
                        _root = this;
                        return  _root;
                    }

                    QuadTree par = parent;
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

        void GetBranchNeightbour(ref QuadTree _ref, Vector2 offset)
        {
            //As root shouldn't have any neightbours
            if (Root != this)
            {
                _ref = Root.GetTreeFor(_position + offset, _divisions);
            }
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

        public Vector3 Center
        {
            get { return _position + _size / 2f; }
        }

        public Vector3 Size
        {
            get { return _size; }
        }

        /// <summary>
        /// Is called whenever the amount should be changed, it changes amount of all parents aswell.
        /// </summary>
        /// <param name="change"></param>
        private void AmountOfObjectsChange(int change)
        {
            QuadTree p = parent;
            _amountOfObjects += change;
            while (p != null)
            {
                p._amountOfObjects += change;
                p = p.parent;
            }
        }

        public QuadTree(int divisions, Vector2 position, Vector2 size)
        {
            _position = position;
            _size = size;
            _divisions = divisions;

            shortBoundary = _position;
            longBoundary = _position + _size;
            if (divisions > 0)
            {
                _isDivided = true;
                int _divisionsLeft = divisions - 1;
                Vector2 _childSize = size / 2f;
                children = new QuadTree[]
                {
                    new QuadTree(_divisionsLeft, position + new Vector2(0, 0), _childSize),
                    new QuadTree(_divisionsLeft, position + new Vector2(_childSize.x, 0), _childSize),
                    new QuadTree(_divisionsLeft, position + new Vector2(0, _childSize.y), _childSize),
                    new QuadTree(_divisionsLeft, position + new Vector2(_childSize.x, _childSize.y), _childSize) 
                };
                foreach (var child in children)
                {
                    child.parent = this;
                }
            }

            if (!_isDivided)
            {
                _activeObjects = new LinkedList<IQuadTreeObject>();
                _staticObjects = new LinkedList<IQuadTreeObject>();
            }
        }

        public void Update()
        {
            if (!_isDivided)
            {
                if (_activeObjects.Count == 0)
                    return;
                List<KeyValuePair<QuadTree, IQuadTreeObject>> objectsToRemove = new List<KeyValuePair<QuadTree, IQuadTreeObject>>();
                foreach (var item in _activeObjects)
                {
                    if (item != null)
                    {
                        Vector2 change = item.PositionChange();
                        if (change != Vector2.zero)
                        {
                            Vector2 pos = item.GetPosition();
                            if (!ContainsPoint(pos))
                            {
                                bool succeed = false;
                                if (change.x >= 0)
                                {
                                    if (change.y >= 0)
                                    {
                                        if (East != null && East.ContainsPoint(pos))
                                        {
                                            objectsToRemove.Add(new KeyValuePair<QuadTree, IQuadTreeObject>(East, item));
                                            succeed = true;
                                        }
                                        else if (NorthEast != null && NorthEast.ContainsPoint(pos))
                                        {
                                            objectsToRemove.Add(new KeyValuePair<QuadTree, IQuadTreeObject>(NorthEast, item));
                                            succeed = true;
                                        }
                                        else if (North != null && North.ContainsPoint(pos))
                                        {
                                            objectsToRemove.Add(new KeyValuePair<QuadTree, IQuadTreeObject>(North, item));
                                            succeed = true;
                                        }
                                    }
                                    else
                                    {
                                        if (East != null && East.ContainsPoint(pos))
                                        {
                                            objectsToRemove.Add(new KeyValuePair<QuadTree, IQuadTreeObject>(East, item));
                                            succeed = true;
                                        }
                                        else if (SouthEast != null && SouthEast.ContainsPoint(pos))
                                        {
                                            objectsToRemove.Add(new KeyValuePair<QuadTree, IQuadTreeObject>(SouthEast, item));
                                            succeed = true;
                                        }
                                        else if (South != null && South.ContainsPoint(pos))
                                        {
                                            objectsToRemove.Add(new KeyValuePair<QuadTree, IQuadTreeObject>(South, item));
                                            succeed = true;
                                        }
                                    }
                                }
                                else
                                {
                                    if (change.y >= 0)
                                    {
                                        if (West != null && West.ContainsPoint(pos))
                                        {
                                            objectsToRemove.Add(new KeyValuePair<QuadTree, IQuadTreeObject>(West, item));
                                            succeed = true;
                                        }
                                        else if (NorthWest != null && NorthWest.ContainsPoint(pos))
                                        {
                                            objectsToRemove.Add(new KeyValuePair<QuadTree, IQuadTreeObject>(NorthWest, item));
                                            succeed = true;
                                        }
                                        else if (North != null && North.ContainsPoint(pos))
                                        {
                                            objectsToRemove.Add(new KeyValuePair<QuadTree, IQuadTreeObject>(North, item));
                                            succeed = true;
                                        }
                                    }
                                    else
                                    {
                                        if (West != null && West.ContainsPoint(pos))
                                        {
                                            objectsToRemove.Add(new KeyValuePair<QuadTree, IQuadTreeObject>(West, item));
                                            succeed = true;
                                        }
                                        else if (SouthWest != null && SouthWest.ContainsPoint(pos))
                                        {
                                            objectsToRemove.Add(new KeyValuePair<QuadTree, IQuadTreeObject>(SouthWest, item));
                                            succeed = true;
                                        }
                                        else if (South != null && South.ContainsPoint(pos))
                                        {
                                            objectsToRemove.Add(new KeyValuePair<QuadTree, IQuadTreeObject>(South, item));
                                            succeed = true;
                                        }
                                    }
                                }
                                if (!succeed)
                                {
                                    objectsToRemove.Add(new KeyValuePair<QuadTree, IQuadTreeObject>(Root, item));
                                }
                            }
                        }
                    }
                }
                if (objectsToRemove.Count > 0)
                {
                    foreach (var item in objectsToRemove)
                    {
                        if (item.Value.IsStatic())
                            _staticObjects.Remove(item.Value);
                        else
                            _activeObjects.Remove(item.Value);
                        item.Key.AddObject(item.Value);
                        AmountOfObjectsChange(-1);
                    }
                }
            }
            else
            {
                foreach (var child in children)
                {
                    if (child.AmountOfObjects > 0)
                        child.Update();
                }
            }
            
            _objectsVisible = null;
            _staticObjectsVisible = null;
            _activeObjectsVisible = null;
        }

        public void AddObject(IQuadTreeObject o)
        {
            if (!_isDivided)
            {
                if (!_wasInitialized)
                {
                    Initalize();
                }
                if (o.IsStatic())
                    _staticObjects.AddFirst(o);
                else
                    _activeObjects.AddFirst(o);

                AmountOfObjectsChange(1);

                QuadTree oldTree = o.CurrentBranch;

                bool _commingFromNeighbour = false;

                if (oldTree == SouthWest)
                {
                    if(NorthWest != null)
                    NorthWest.SendObjectBecameVisibleToLocals(o);
                    if (North != null)
                    North.SendObjectBecameVisibleToLocals(o);
                    if (NorthEast != null)
                    NorthEast.SendObjectBecameVisibleToLocals(o);
                    if (East != null)
                    East.SendObjectBecameVisibleToLocals(o);
                    if (SouthEast != null)
                    SouthEast.SendObjectBecameVisibleToLocals(o);
                    _commingFromNeighbour = true;
                }
                if (oldTree == SouthEast)
                {
                    if (NorthWest != null) NorthWest.SendObjectBecameVisibleToLocals(o);
                    if (North != null) North.SendObjectBecameVisibleToLocals(o);
                    if (NorthEast != null) NorthEast.SendObjectBecameVisibleToLocals(o);
                    if (West != null) West.SendObjectBecameVisibleToLocals(o);
                    if (SouthWest != null) SouthWest.SendObjectBecameVisibleToLocals(o);
                    _commingFromNeighbour = true;
                }
                if (oldTree == NorthEast)
                {
                    if (NorthWest != null) NorthWest.SendObjectBecameVisibleToLocals(o);
                    if (West != null) West.SendObjectBecameVisibleToLocals(o);
                    if (SouthEast != null) SouthEast.SendObjectBecameVisibleToLocals(o);
                    if (South != null) South.SendObjectBecameVisibleToLocals(o);
                    if (SouthWest != null) SouthWest.SendObjectBecameVisibleToLocals(o);
                    _commingFromNeighbour = true;
                }
                if (oldTree == NorthWest)
                {
                    if (SouthWest != null) SouthWest.SendObjectBecameVisibleToLocals(o);
                    if (South != null) South.SendObjectBecameVisibleToLocals(o);
                    if (NorthEast != null) NorthEast.SendObjectBecameVisibleToLocals(o);
                    if (East != null) East.SendObjectBecameVisibleToLocals(o);
                    if (SouthEast != null) SouthEast.SendObjectBecameVisibleToLocals(o);
                    _commingFromNeighbour = true;
                }

                if (oldTree == West)
                {
                    if (NorthEast != null) NorthEast.SendObjectBecameVisibleToLocals(o);
                    if (East != null) East.SendObjectBecameVisibleToLocals(o);
                    if (SouthEast != null) SouthEast.SendObjectBecameVisibleToLocals(o);
                    _commingFromNeighbour = true;
                }

                if (oldTree == East)
                {
                    if (NorthWest != null) NorthWest.SendObjectBecameVisibleToLocals(o);
                    if (West != null) West.SendObjectBecameVisibleToLocals(o);
                    if (SouthWest != null) SouthWest.SendObjectBecameVisibleToLocals(o);
                    _commingFromNeighbour = true;
                }

                if (oldTree == South)
                {
                    if (NorthEast != null) NorthEast.SendObjectBecameVisibleToLocals(o);
                    if (North != null) North.SendObjectBecameVisibleToLocals(o);
                    if (NorthWest != null) NorthWest.SendObjectBecameVisibleToLocals(o);
                    _commingFromNeighbour = true;
                }

                if (oldTree == North)
                {
                    if (SouthWest != null) SouthWest.SendObjectBecameVisibleToLocals(o);
                    if (South != null) South.SendObjectBecameVisibleToLocals(o);
                    if (SouthEast != null) SouthEast.SendObjectBecameVisibleToLocals(o);
                    _commingFromNeighbour = true;
                }

                if (oldTree == null || _commingFromNeighbour == false)
                {
                    for (int i = 0; i < ObjectsVisible.Count; i++)
                    {
                        var treeObject = ObjectsVisible[i];
                        treeObject.ObjectBecameVisible(o);
                    }
                }
                o.CurrentBranch = this;
            }
            else
            {
                QuadTree t = GetTreeFor(o.GetPosition());
                if (t != null)
                    t.AddObject(o);
                else
                {
                    Debug.LogError("Failed to find sub tree for DirecionVector: "+ o.GetPosition());
                }
            }
        }

        /// <summary>
        /// Is ran whenever this branch has some kind of idea, like its actually used.
        /// </summary>
        private void Initalize()
        {
            _wasInitialized = true;
            /*AstarPath astarPath = AstarPath.Instance;
            //on erro rescan the astar
            Bounds bounds = new Bounds();

            bounds.center = new Vector3(Center.x, 0, Center.y);
            bounds.size = new Vector3(Size.x, 50, Size.y);

            astarPath.UpdateGraphs(bounds);*/
        }

        private void SendObjectBecameVisibleToLocals(IQuadTreeObject o)
        {
            if (!_wasInitialized)
            {
                Initalize();
            }
            foreach (var treeObject in _activeObjects)
            {
                ServerUnit unit = treeObject as ServerUnit;
                if (unit != null)
                {
                    unit.ObjectBecameVisible(o);
                }
            }
            foreach (var treeObject in _staticObjects)
            {
                ServerUnit unit = treeObject as ServerUnit;
                if (unit != null)
                {
                    unit.ObjectBecameVisible(o);
                }
            }
        }
        private QuadTree GetTreeFor(Vector2 pos)
        {
            return GetTreeFor(pos, 0);
        }

        private QuadTree GetTreeFor(Vector2 pos, int division)
        {
            if (ContainsPoint(pos))
            {
                if (_divisions == division)
                {
                    return this;
                }
                else
                {
                    QuadTree tree;
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
            if (shortBoundary.x <= point.x && shortBoundary.y <= point.y && longBoundary.x > point.x && longBoundary.y > point.y)
                return true;
            return false;
        }

        public void DrawGizmos()
        {
            if (AmountOfObjects == 0)
                return;

            if (!_isDivided)
            {
                Gizmos.color = Color.white;
                Gizmos.DrawWireCube(new Vector3(_position.x, 20, _position.y) + new Vector3(_size.x, 0, _size.y) / 2, new Vector3(_size.x, 0, _size.y));
                Gizmos.color = Color.white;
                foreach (var o in _activeObjects)
                {
                    Vector2 pos = o.GetPosition();
                    Gizmos.DrawCube(new Vector3(pos.x, 20, pos.y), Vector3.one);
                }
                Gizmos.color = Color.gray;
                foreach (var o in _staticObjects)
                {
                    Vector2 pos = o.GetPosition();
                    Gizmos.DrawCube(new Vector3(pos.x, 20, pos.y), Vector3.one);
                }
            }
            else
            {
                Gizmos.color = Color.red;
                Gizmos.DrawWireCube(new Vector3(_position.x, 20, _position.y) + new Vector3(_size.x, 0, _size.y) / 2, new Vector3(_size.x, 0, _size.y));
                foreach (var item in children)
                {
                    item.DrawGizmos();
                }
            }
        }

        public void RemoveObject(IQuadTreeObject o)
        {
            if (o.IsStatic())
                _staticObjects.Remove(o);
            else
                _activeObjects.Remove(o);
            AmountOfObjectsChange(-1);
        }
    }
}

#endif
