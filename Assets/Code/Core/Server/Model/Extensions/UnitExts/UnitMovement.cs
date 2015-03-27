using System;
using Libaries.Net.Packets.ForClient;
#if SERVER
using Shared.Content.Types;

using Server.Model.Pathfinding;

using Code.Code.Libaries.Net;
using Server.Model.Entities.Vegetation;
using Pathfinding;
using Server.Model.Entities;
using Server.Model.Entities.Human;
using UnityEngine;

namespace Server.Model.Extensions.UnitExts
{

    public class UnitMovement : UnitUpdateExt
    {
        //how fast does force fadeout?
        private const float ForceFade = 0.80f;

        //how strong the force is?
        private const float ForceWeight = 0.0f;

        //important variables
        [SerializeField]
        private Vector3 _position = Vector3.one;
        [SerializeField]
        private float _rotation = 0;

        public float _baseSpeed = 2.5f;

        private Vector3 _force = Vector3.zero;

        //Pathfinding seeker
        private Seeker _seeker;
        private int _currentWaypoint = 0;

        //other variables
        private UnitCombat _combat;
        private bool _positionUpdate = false;
        private bool _rotationUpdate = false;
        private bool _parentUpdate = false;

        //destination variables
        private Vector3 destination;
        private Path _path;
        private bool _lookingForPath = false;
        private bool _dontWalk;
        public float _rotationSpeed = 10f;
        private System.Action OnArrive;

        public UnitMovement Parent
        {
            get { return _parent; }
            set
            {
                _parent = value;
                _parentUpdate = true;
                _wasUpdate = true;
            }
        }

        public int ParentPlaneID = 0;

        #region CORRECTION

        private Vector3 _lastPositionSent;
        private float _correctionWasSent = 0f;
        private const float _correctionRatio = 0.333f;
        #endregion

        #region UnprecieseMovement

        public UDPUnprecieseMovement UnprecieseMovementPacket;
        [SerializeField]
        private UnitMovement _parent;

        #endregion

        //property getters
        public Vector3 Position
        {
            get { return _position; }
            private set
            {
                _position = value;
                _positionUpdate = true;
                bool _correction = (_correctionWasSent + _correctionRatio) < Time.time || Teleported;
                if (_correction)
                    _wasUpdate = true;
                else
                {
                    float distance = Vector3.Distance(_lastPositionSent, _position);
                    Vector3 difference = _position - _lastPositionSent;
                    float angleInDegrees = Mathf.Atan2(difference.z, difference.x) * 180 / Mathf.PI;

                    UnprecieseMovementPacket = new UDPUnprecieseMovement();

                    UnprecieseMovementPacket.Angle = angleInDegrees;
                    UnprecieseMovementPacket.Face = Rotation;
                    UnprecieseMovementPacket.Distance = distance;
                    UnprecieseMovementPacket.UnitID = Unit.ID;

                    _lastPositionSent = _position;
                }
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
                _rotationUpdate = true;
            }
        }

        public bool CanMove { get; set; }

        public bool CanRotate { get; set; }

        public bool Running { get; set; }

        public float CurrentSpeed
        {
            get
            {
                return _baseSpeed * (1.0f + Unit.Attributes[UnitAttributeProperty.MovementSpeed])
                    *
                    (Running ? 3f : 1f);
            }
        }

        //Methods
        public override void Progress()
        {
            base.Progress();

            if (Parent != null)
                return;

            if (Running && Unit.Combat.Energy < 20)
            {
                Running = false;
            }

            if (_path != null)
            {
                if (_currentWaypoint >= _path.vectorPath.Count)
                {
                    _path = null;
                    if (OnArrive != null)
                        OnArrive();
                }
                else
                {
                    MoveAndRotate();
                }
            }

            if (_dontWalk)
            {
                _path = null;
            }

            if (_force.magnitude > 0.017f)
            {
                Position += Force;
            }
            _force *= ForceFade;

            /*
            RotateTo(Quaternion.LookRotation(new Vector3(destination.x, 0, destination.z) - new Vector3(_position.x, 0, _position.z)).eulerAngles.y);

            if (Vector2.Distance(new Vector2(destination.x, destination.z), new Vector2()) > 0.5f)
            {
                MoveTo(_position + Quaternion.Euler(new Vector3(0,_rotation,0)) * Vector3.forward * _baseSpeed);
            }*/
        }

        private void MoveAndRotate()
        {
            try
            {
                //Check if we are close enough to the next waypoint
                //If we are, proceed to follow the next waypoint
                for (int i = 0; i < 3; i++)
                {
                    Vector3 wayPoint = _path.vectorPath[_currentWaypoint];
                    if (Vector2.Distance(new Vector2(_position.x, _position.z), new Vector2(wayPoint.x, wayPoint.z)) <
                        Unit.Display.Size * 0.75f)
                    {
                        _currentWaypoint++;
                    }
                }

                Vector3 waypoint = _path.vectorPath[_currentWaypoint];
                Vector3 dir = waypoint - _position;

                float dirMagnitude = dir.magnitude;

                if (dirMagnitude > 0.3f)
                {
                    /*if (
                    RotateTo(
                        Quaternion.LookRotation(dir * _rotationSpeed * (1.0f + Unit.Attributes[UnitAttributeProperty.Mobility]) *
                                                Time.fixedDeltaTime +
                                                (Quaternion.Euler(new Vector3(0, _rotation, 0)) * Vector3.forward)).eulerAngles.y))
                {*/
                    RotateTo(Quaternion.LookRotation(dir).eulerAngles.y);
                    //eq holding space
                    if (!_dontWalk /*&&
                        Vector3.Distance(Position + Forward * 1.25f, waypoint) < Vector3.Distance(Position + Forward * -1, waypoint)*/)
                    {
                        MoveForward(CurrentSpeed * Time.fixedDeltaTime);
                        _position.y += (waypoint.y - _position.y) / 5f;
                    }

                }

                //Check if we are close enough to the next waypoint
                //If we are, proceed to follow the next waypoint
                for (int i = 0; i < 3; i++)
                {
                    Vector3 wayPoint = _path.vectorPath[_currentWaypoint];
                    if (Vector2.Distance(new Vector2(_position.x, _position.z), new Vector2(wayPoint.x, wayPoint.z)) <
                        Unit.Display.Size * 0.75f)
                    {
                        _currentWaypoint++;
                    }
                }
            }
            catch (ArgumentOutOfRangeException e)
            { }
        }

        private void MoveForward(float speed)
        {
            MoveTo(_position + Forward * speed);

            ServerUnit serverUnit = entity as ServerUnit;
            if (serverUnit != null)
            {
                serverUnit.Combat.ReduceEnergy(speed * 0.3f);
            }
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
            if (CanMove)
            {
                _force += (newPosition - _position) * ForceWeight;
                Position = newPosition;
            }
        }

        public void StopWalking()
        {
            _dontWalk = true;
        }

        public void ContinueWalking()
        {
            _dontWalk = false;
        }

        /// <summary>
        /// Smoothly walks to the destination.
        /// </summary>
        /// <param name="newPosition">Destination of walk.</param>
        public void WalkTo(Vector3 newPosition)
        {
            if (CanMove && !_lookingForPath)
            {
                if (Vector3.Distance(destination, newPosition) < 0.1f)
                    return;
                destination = newPosition;
                _seeker.StartPath(_position, destination, OnPathWasFound);
                _lookingForPath = true;
                OnArrive = null;
            }
        }

        public void WalkTo(Vector3 newPosition, System.Action<int, string> _onArrive, int unitId, string action)
        {
            WalkTo(newPosition, () => _onArrive(unitId, action));
        }

        public void WalkTo(Vector3 newPosition, System.Action _onArrive)
        {
            if (CanMove && !_lookingForPath)
            {
                /*if (Vector3.Distance(destination, newPosition) < 0.1f)
                {
                    this._onArrive = _onArrive;
                    return;
                }*/

                destination = newPosition;
                _seeker.StartPath(_position, destination, OnPathWasFound);
                _lookingForPath = true;

                OnArrive = _onArrive;
            }
        }

        public void WalkWay(Vector3 direction)
        {
            WalkTo(_position + direction * (Unit.Display.Size + 0.5f));
        }

        private bool RotateTo(float newRotation)
        {
            if (CanRotate)
            {
                Rotation = newRotation;
                return true;
            }
            return false;
        }

        public void Teleport(Vector3 location)
        {
            Position = location;
            Teleported = true;
            _wasUpdate = true;
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
                _currentWaypoint = 0;
                _path = path;
            }
            else
            {
                Debug.LogError("Error finding path: " + path.errorLog);
            }
        }

        protected override void OnExtensionWasAdded()
        {
            base.OnExtensionWasAdded();

            Unit = entity as ServerUnit;
            
            if (!Unit.IsStatic())
            {
                _seeker = entity.gameObject.AddComponent<Seeker>();
                entity.gameObject.AddComponent<FunnelModifier>();
                CanMove = true;
                CanRotate = true;
            }
            if (!(Unit is Plant))
            {
                CanMove = true;
                CanRotate = true;
            }
        }

        #region StateSerialization
        protected override void pSerializeState(Code.Code.Libaries.Net.ByteStream packet)
        {
            packet.AddFlag(true, true, true, true, true);
            packet.AddPosition6B(_position);

            packet.AddAngle1B(_rotation);
            
            packet.AddShort(Parent == null ? -1 : Parent.Unit.ID);
            packet.AddByte(ParentPlaneID);
        }

        protected override void pSerializeUpdate(Code.Code.Libaries.Net.ByteStream packet)
        {
            bool _correction = (_correctionWasSent + _correctionRatio) < Time.time || Teleported;
            packet.AddFlag(_positionUpdate, _rotationUpdate, Teleported, _correction, _parentUpdate);
            if (_correction)
            {
                if (_positionUpdate)
                {
                    packet.AddPosition6B(_position);
                    _correctionWasSent = Time.time;
                    _lastPositionSent = _position;
                }
                if (_rotationUpdate)
                {
                    packet.AddAngle1B(_rotation);
                }
                if (_parentUpdate)
                {
                    packet.AddShort(Parent == null ? -1 : Parent.Unit.ID);
                    packet.AddByte(ParentPlaneID);
                }
            }

            Teleported = false;
            _parentUpdate = false;
        }
        #endregion StateSerialization

        //Update flag
        public override byte UpdateFlag()
        {
            return 0x01;
        }

        /// <summary>
        /// Push the unit to direction. This function is safe so this unit wont get pushed away int non walkable terrain.
        /// </summary>
        /// <param name="vector2">direction and its strenght</param>
        public void PushFromCollision(Vector3 strenght, ServerUnit secondUnit)
        {
            if (Unit is Plant && secondUnit is Plant)
            {
                Teleport(_position + new Vector3(strenght.x, 0, strenght.z));
            }
            MoveTo(_position + new Vector3(strenght.x, 0, strenght.z));
            _path = null;
        }

        public void PushForward(float strenght)
        {
            Push(Forward, strenght);
        }

        public void Push(Vector3 vector3, float strenght)
        {
            if (CanMove)
                MoveTo(_position + vector3 * strenght);
        }

        public override void Serialize(ByteStream bytestream)
        {
            bytestream.AddPosition12B(_position);
            bytestream.AddFloat4B(_rotation);
        }

        public override void Deserialize(ByteStream bytestream)
        {
            _position = bytestream.GetPosition12B();
            _rotation = bytestream.GetFloat4B();
        }

        public void _UnSafeMoveTo(Vector3 position)
        {
            Position = position;
        }
    }
}
#endif
