using System;
using System.Collections;
using Libaries.IO;
using Libaries.Net.Packets.ForClient;
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

        public float _baseSpeed = 2.5f;

        private Vector3 _force = Vector3.zero;

        //Pathfinding seeker
        private Seeker _seeker;
        private int _currentWaypoint;

        //other variables
        private UnitCombat _combat;
        private bool _parentUpdate;

        //destination variables
        private Vector3 destination;
        private Path _path;
        private bool _lookingForPath;
        private Action OnArrive;

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

        public bool IsFlying
        {
            get { return _isFlying; }
            private set
            {
                _isFlying = value;
                _wasUpdate = true;
            }
        }

        public int ParentPlaneID = 0;

        #region CORRECTION

        private Vector3 _lastPositionSent;
        private float _correctionWasSent;
        private const float _correctionRatio = 0.333f;
        #endregion

        #region UnprecieseMovement

        public UDPUnprecieseMovement UnprecieseMovementPacket;
        [SerializeField]
        private UnitMovement _parent;

        private bool _isFlying;

        #endregion

        //property getters
        public Vector3 Position
        {
            get { return Parent == null ? _position : Parent._position; }
            private set
            {
                _position = value;

                transform.position = value;

                RecreateUpdatePrecesion();
            }
        }

        private void RecreateUpdatePrecesion()
        {
            bool _correction = (_correctionWasSent + _correctionRatio) < Time.time || Teleported;
            if (_correction)
                _wasUpdate = true;
            else
            {
                Vector3 difference = _position - _lastPositionSent;
                
                    UnprecieseMovementPacket = new UDPUnprecieseMovement();
                    UnprecieseMovementPacket.Mask = new BitArray(new[] { _isFlying });
                    UnprecieseMovementPacket.Face = _rotation;
                    UnprecieseMovementPacket.Difference = difference;
                    UnprecieseMovementPacket.UnitID = Unit.ID;
                _lastPositionSent = _position;
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
                RecreateUpdatePrecesion();
            }
        }
        
        public bool Running { get; set; }

        public float WalkSpeed { get; set; }
        public float CurrentSpeed
        {
            get
            {
                return 1.75f * (1.0f + Unit.Attributes[UnitAttributeProperty.MovementSpeed])
                    *  Mathf.Min(WalkSpeed, Running ? 3f:1.5f);
            }
        }

        //Methods
        public override void Progress(float time)
        {
            base.Progress(time);

            if (Parent != null)
                return;

            _force *= ForceFade;

            if (IsFlying)
            {
                //Stop walking on floor
                _path = null;

                //Raycast next hit
                RaycastHit hit;
                Physics.Raycast(_position, Force, out hit, _force.magnitude, 1 << 8);
                if (hit.collider != null)
                {
                    Position = hit.point + new Vector3(0,0.3f,0);
                    _force = Vector3.zero;
                    IsFlying = false;
                }
                else
                {
                    Position += _force;
                    _force += Vector3.down / 5f;
                }
                return;
            }

            if (Running && Unit.Combat.CurrentEnergy < 20)
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
                    MoveAndRotate(time);
                }
            }
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
                _force += (newPosition - _position) * ForceWeight;
                Position = newPosition;
        }
        

        public void WalkTo(Vector3 newPosition, System.Action<int, string> _onArrive, int unitId, string action)
        {
            WalkTo(newPosition, () => _onArrive(unitId, action));
        }

        public void WalkTo(Vector3 newPosition, System.Action _onArrive)
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
                _seeker.StartPath(_position, destination, OnPathWasFound);
                _lookingForPath = true;

                
            }
        }

        public void WalkWay(Vector3 direction)
        {
            WalkSpeed = direction.magnitude;
            WalkTo(_position + direction,null);
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
                Teleport(_position + new Vector3(strenght.x, 0, strenght.z));
            MoveTo(_position + new Vector3(strenght.x, 0, strenght.z));
            _path = null;
        }

        public void PushForward(float strenght)
        {
            Push(Forward, strenght);
        }

        public void Push(Vector3 vector3, float strenght)
        {
            MoveTo(_position + vector3.normalized*strenght);
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

            Teleport(new Vector3(float.Parse(movement.GetField("x").str), float.Parse(movement.GetField("y").str), float.Parse(movement.GetField("z").str)));
            Rotation = float.Parse(movement.GetField("rot").str);
        }

        public void _UnSafeMoveTo(Vector3 position)
        {
            Position = position;
        }

        public void Fly(Vector3 wayAndStrenght)
        {
            _force += wayAndStrenght;
            IsFlying = true;
        }

        public void Jump()
        {
            if (!_isFlying)
            {
                Fly(Vector3.up * 0.5f + Forward * 0.3f);
            }
        }

        public void DiscardPath()
        {
            _path = null;
        }
    }
}
#endif
