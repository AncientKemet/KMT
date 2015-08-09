using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Code.Libaries.Generic.Trees;
using Libaries.IO;
using Libaries.Net.Packets.ForClient;
using Server.Model.Entities.Human;
using Server.Model.Entities.StaticObjects;
#if SERVER
using Shared.Content.Types;
using Pathfinding;
using Server.Model.Entities;
using UnityEngine;

namespace Server.Model.Extensions.UnitExts
{

    public class UnitMovement : UnitUpdateExt
    {
        //how fast does force fadeout?
        private const float ForceFade = 0.99f;

        //how strong the force is?
        private const float ForceWeight = 0.0f;

        //important variables
        [SerializeField]
        private Vector3 _position = Vector3.one;
        private float _rotation;

        private Vector3 _force = Vector3.zero;

        //Pathfinding seeker
        private Seeker _seeker;
        private int _currentWaypoint;

        //other variables
        private UnitCombat _combat;
        private bool _parentUpdate;
        private Action _pushAction;
        private float _pushLenght = -1;

        //destination variables
        private Vector3 destination;
        private Path _path;
        private bool _lookingForPath;
        private Action OnArrive;
        private Action OnInterrupt;
        private LinkedList<Action> NextMovements = new LinkedList<Action>();

        public UnitMovement Parent
        {
            get { return _parent; }
            set
            {
                if (value == null && _parent != null) Position = _parent.Position;
                else if (value != null) Position = value.Position;
                _parent = value;
                _parentUpdate = true;
                _wasUpdate = true;
            }
        }

        [SerializeField]
        private UnitMovement _parent;
        public int ParentPlaneID = 0;

        #region CORRECTION
        private Vector3 _lastPositionSent;
        private float _correctionWasSent;
        private const float _correctionRatio = 1.333f;
        #endregion

        #region UnprecieseMovement
        private bool _smallUpdate = false;
        private bool _isFlying;
        #endregion

        //property getters
        public Vector3 Position
        {
            get { return Parent == null ? _position : Parent._position; }
            private set
            {
                _position = value;
                _smallUpdate = true;
            }
        }

        private void CheckForSmallUpdate()
        {
            if (_smallUpdate)
            {
                if (_correctionWasSent + _correctionRatio < Time.time || Teleported)
                    _wasUpdate = true;
                else
                {
                    Vector3 difference = _position - _lastPositionSent;

                    var UnprecieseMovementPacket = new UDPUnprecieseMovement();
                    UnprecieseMovementPacket.Face = _rotation;
                    UnprecieseMovementPacket.Difference = difference;
                    UnprecieseMovementPacket.UnitID = Unit.ID;
                    _lastPositionSent = _position;

                    foreach (IQuadTreeObject objectAround in Unit.CurrentBranch.ActiveObjectsVisible)
                    {
                        Player playerAround = objectAround as Player;
                        if (playerAround != null)
                            playerAround.PlayerUdp.Send(UnprecieseMovementPacket);
                    }
                }
                _smallUpdate = false;
            }
        }

        public ServerUnit Unit { get; private set; }

        public Vector3 Force
        {
            get { return _force; }
        }

        public float Rotation
        {
            get { return _rotation; }
            set
            {
                _rotation = value;
                _smallUpdate = true;
            }
        }

        public bool Running { get; set; }

        public float CurrentSpeed
        {
            get
            {
                return 1.75f * (1.0f + Mathf.Clamp01(Unit.Attributes[UnitAttributeProperty.MovementSpeed])) * (Running ? 3f : 1.5f);
            }
        }

        //Methods
        public override void Progress(float time)
        {
            base.Progress(time);

            if (Parent != null)
                return;

            if (Running && Unit.Combat.CurrentEnergy < 20)
                Running = false;

            lock (NextMovements)
            {
                if (NextMovements.Count > 0)
                {
                    NextMovements.First()();
                    NextMovements.RemoveFirst();
                }
                else
                {
                    if (_path != null)
                    {
                        if (_currentWaypoint >= _path.vectorPath.Count)
                        {
                            _path = null;
                            if (OnArrive != null)
                                OnArrive();
                        }
                        else
                            MoveAndRotate(time);
                    }
                }
            }
            CheckForSmallUpdate();
        }

        private void MoveAndRotate(float time)
        {
            Vector3 waypoint = _path.vectorPath[_currentWaypoint];
            Vector3 dir = waypoint - _position;
            _rotation = Quaternion.LookRotation(dir).eulerAngles.y;

            float step = CurrentSpeed * time;

            if (dir.magnitude < step)
                _currentWaypoint++;

            MoveTo(_position + dir.normalized * step);
        }

        public Vector3 Forward
        {
            get { return Quaternion.Euler(0, _rotation, 0) * Vector3.forward; }
        }

        /// <summary>
        /// Instantly moves to new DirecionVector, adds force.
        /// </summary>
        /// <param name="newPosition"></param>
        private void MoveTo(Vector3 newPosition)
        {
            _force += (newPosition - _position) * ForceWeight;
            Position = newPosition;
        }

        public void WalkTo(Vector3 newPosition, System.Action<int, string> _onArrive, int unitId, string action)
        {
            WalkTo(newPosition, () => _onArrive(unitId, action), null);
        }

        public void WalkTo(Vector3 newPosition, Action _onArrive, Action _onInterupt = null)
        {
            if (!_lookingForPath)
            {
                if (Vector3.Distance(destination, newPosition) < 0.1f)
                {
                    if (_onArrive != null)
                        _onArrive();
                    return;
                }

                destination = newPosition;
                OnArrive = _onArrive;
                OnInterrupt = _onInterupt;
                _seeker.StartPath(_position, destination, OnPathWasFound);
                _lookingForPath = true;
            }
        }

        public void WalkWay(Vector3 direction)
        {
            WalkTo(_position + direction, null);
        }

        public void RotateWay(Vector3 direcionVector)
        {
            if (direcionVector != Vector3.zero)
                Rotation = Quaternion.LookRotation(direcionVector).eulerAngles.y;
        }

        public void Teleport(Vector3 location)
        {
            _position = location;
            Teleported = true;
            _wasUpdate = true;
            _path = null;
            if (OnInterrupt != null)
                OnInterrupt();
        }

        public bool Teleported { get; private set; }

        public bool IsWalkingSomeWhere
        {
            get { return _lookingForPath || _path != null; }
        }

        private void OnPathWasFound(Path path)
        {
            _lookingForPath = false;
            if (!path.error)
            {
                _currentWaypoint = 1;
                _path = path;
            }
        }

        private void OnPathWasFoundPush(Path path)
        {
            lock(NextMovements)
            NextMovements.AddLast(() =>
            {
                if (!path.error)
                    if (path.GetTotalLength() < _pushLenght * 2f)
                        MoveTo(path.vectorPath.Last());
                if (_pushAction != null)
                    _pushAction();
                _pushAction = null;
                _pushLenght = -1;
            });
        }

        protected override void OnExtensionWasAdded()
        {
            base.OnExtensionWasAdded();

            Unit = entity as ServerUnit;

            if (!Unit.IsStatic())
            {
                _seeker = entity.gameObject.AddComponent<Seeker>();
                entity.gameObject.AddComponent<FunnelModifier>();
            }
        }

        #region StateSerialization
        protected override void pSerializeState(Code.Code.Libaries.Net.ByteStream packet)
        {
            packet.AddFlag(true, true, _isFlying);
            packet.AddPosition12B(_position);

            packet.AddAngle2B(_rotation);

            packet.AddShort(Parent == null ? -1 : Parent.Unit.ID);
            packet.AddByte(ParentPlaneID);
            _lastPositionSent = _position;
        }

        protected override void pSerializeUpdate(Code.Code.Libaries.Net.ByteStream packet)
        {
            packet.AddFlag(Teleported, _parentUpdate, _isFlying);
            packet.AddPosition12B(_position);

            packet.AddAngle2B(_rotation);
            if (_parentUpdate)
            {
                packet.AddShort(Parent == null ? -1 : Parent.Unit.ID);
                packet.AddByte(ParentPlaneID);
            }

            Teleported = false;
            _parentUpdate = false;
            _correctionWasSent = Time.time;
            _lastPositionSent = _position;
        }
        //Update flag
        public override byte UpdateFlag()
        {
            return 0x01;
        }
        #endregion StateSerialization

        public void PushFromCollision(Vector3 strenght, ServerUnit secondUnit)
        {
            Push(strenght, strenght.magnitude);
        }

        public void PushForward(float strenght, Action action = null)
        {
            Push(Forward, strenght, action);
        }

        public void Push(Vector3 vector3, float strenght, Action action = null)
        {
            _pushAction = action;
            _pushLenght = strenght;
            if (_pushLenght > 0.7f)
                DiscardPath();
            _seeker.StartPath(_position, _position + vector3 * strenght, OnPathWasFoundPush);
        }

        public override void Serialize(JSONObject j)
        {
            JSONObject movement = new JSONObject();

            movement.AddField("x", "" + Position.x);
            movement.AddField("y", "" + Position.y);
            movement.AddField("z", "" + Position.z);

            movement.AddField("rot", "" + Rotation);

            j.AddField("movement", movement);
        }

        public override void Deserialize(JSONObject j)
        {
            JSONObject movement = j.GetField("movement");
            Vector3 pos = new Vector3(float.Parse(movement.GetField("x").str), float.Parse(movement.GetField("y").str),
                                      float.Parse(movement.GetField("z").str));
            if(pos.x < 0 || pos.z < 0)
                pos = new Vector3(512,12,512);
            Teleport(pos);
            Rotation = float.Parse(movement.GetField("rot").str);
        }

        public void _UnSafeMoveTo(Vector3 position)
        {
            Position = position;
        }

        public void DiscardPath()
        {
            _path = null;
            if (OnInterrupt != null)
                OnInterrupt();
        }
    }
}
#endif
