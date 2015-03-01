#if SERVER
using System;
using System.Collections.Generic;
using Code.Code.Libaries.Net;
using Code.Libaries.Generic.Trees;
using Code.Libaries.Net.Packets.ForClient;
using Server.Model.Entities.Human;
using Server.Model.Extensions;
using Server.Model.Extensions.UnitExts;
using UnityEngine;

namespace Server.Model.Entities
{
    public class ServerUnit : WorldEntity, IQuadTreeObject
    {
        public UnitMovement Movement;

        public UnitCombat Combat;

        public UnitDisplay Display;

        public UnitAnim Anim;

        public UnitAttributes Attributes;

        public UnitFocus Focus;

        public UnitActions Actions;

        private List<UnitUpdateExt> _updateExtensions;

        protected virtual void OnEnterWorld(World world)
        { }

        public override World CurrentWorld
        {
            get { return base.CurrentWorld; }
            set
            {
                if (base.CurrentWorld != null)
                {
                    if (CurrentBranch != null)
                    {
                        CurrentBranch.RemoveObject(this);
                    }
                }
                _quadTreePos = new Vector2(Movement.Position.x, Movement.Position.z);
                base.CurrentWorld = value;
                OnEnterWorld(value);
            }
        }

        public virtual void Awake()
        {
            _updateExtensions = new List<UnitUpdateExt>();

            AddExt(Movement = new UnitMovement());
            if(Display == null)
            AddExt(Display = new UnitDisplay());
            
            AddExt(Actions = new UnitActions());
            AddExt(Focus = new UnitFocus());

            //in the end find all updatable extensions
            foreach (EntityExtension extension in Extensions)
            {
                if (extension is UnitUpdateExt)
                {
                    _updateExtensions.Add(extension as UnitUpdateExt);
                }
            }

            _updateExtensions.Sort(
                (ext, updateExt) => ext.UpdateFlag().CompareTo(updateExt.UpdateFlag())
                );
        }

        public override void Progress()
        {
            if(CurrentWorld == null)
                return;
            
            ReCreateUpdatePacket();

            if (_updatePacket != null)
            {
                SendPacketToPlayersAround(_updatePacket);
            }

            base.Progress();

            _statePacket = null;
        }

        private void SendPacketToPlayersAround(BasePacket packet)
        {
            foreach (IQuadTreeObject objectAround in CurrentBranch.ActiveObjectsVisible)
            {
                Player playerAround = objectAround as Player;
                if (playerAround != null)
                {
                    playerAround.Client.ConnectionHandler.SendPacket(packet);
                    if (Movement.UnprecieseMovementPacket != null)
                    {
                        playerAround.PlayerUdp.Send(Movement.UnprecieseMovementPacket);
                    }
                }
            }
            Movement.UnprecieseMovementPacket = null;
        }
        #region SERIALIZATION

        public virtual void Serialize(ByteStream bytestream)
        {
            foreach (var extension in Extensions)
            {
                extension.Serialize(bytestream);
            }
        }

        public virtual void Deserialize(ByteStream bytestream)
        {
            foreach (var extension in Extensions)
            {
                extension.Deserialize(bytestream);
            }
        }

        #endregion

        #region QUAD TREE IMPL
        private QuadTree _currentBranch;
        private Vector2 _quadTreePos = Vector2.zero;

        /// <summary>
        /// Gets or sets a quad tree current branch for this, unit. Don't use Set, as it's already implemented.
        /// </summary>
        public QuadTree CurrentBranch
        {
            get { return _currentBranch; }
            set { _currentBranch = value; }
        }

        /// <summary>
        /// Gets the x, z position of this unit. 
        /// </summary>
        /// <returns></returns>
        public Vector2 GetPosition()
        {
            return _quadTreePos + new Vector2(0.01f, 0.01f);
        }

        public Vector2 PositionChange()
        {
            Vector2 r = new Vector2(Movement.Position.x, Movement.Position.z) - _quadTreePos;
            _quadTreePos = new Vector2(Movement.Position.x, Movement.Position.z);
            return r;
        }

        public virtual void ObjectBecameVisible(IQuadTreeObject o)
        {
            Player player = o as Player;
            if (player != null && player != this)
            {
                player.Client.ConnectionHandler.SendPacket(StatePacket);
            }
        }

        public virtual bool IsStatic()
        {
            return true;
        }

        #endregion QUAD TREE IMPL

        #region STATE_PACKET

        private UnitUpdatePacket _statePacket;

        /// <summary>
        /// State packet is an classical update packe with the exception of that, it serializes all data.
        /// </summary>
        public UnitUpdatePacket StatePacket
        {
            get
            {
                if (_statePacket == null)
                {
                    _statePacket = new UnitUpdatePacket();
                    //Crate mask
                    int mask = 0;
                    foreach (UnitUpdateExt updateExtension in _updateExtensions)
                    {
                        mask = mask | updateExtension.UpdateFlag();
                    }

                    _statePacket = new UnitUpdatePacket();
                    _statePacket.UnitID = ID;

                    //add mask
                    _statePacket.SubPacketData.AddByte(mask);

                    //serialize the rest of the packet
                    foreach (UnitUpdateExt updateExtension in _updateExtensions)
                    {
                        updateExtension.SerializeState(_statePacket.SubPacketData);
                    }

                }
                return _statePacket;
            }
        }
        #endregion

        #region UPDATE_PACKET
        private UnitUpdatePacket _updatePacket;

        private void ReCreateUpdatePacket()
        {

            _updatePacket = null;
            //Crate mask
            int mask = 0;
            foreach (UnitUpdateExt updateExtension in _updateExtensions)
            {
                if (updateExtension.WasUpdate())
                {
                    mask = mask | updateExtension.UpdateFlag();
                }
            }

            if (mask == 0)
                return;

            _updatePacket = new UnitUpdatePacket();
            _updatePacket.UnitID = ID;

            //add mask
            _updatePacket.SubPacketData.AddByte(mask);

            //serialize the rest of the packet
            foreach (UnitUpdateExt updateExtension in _updateExtensions)
            {
                if (updateExtension.WasUpdate())
                {
                    updateExtension.SerializeUpdate(_updatePacket.SubPacketData);
                    updateExtension.ResetUpdate();
                }
            }

#if DEBUG_NETWORK
            string log = "";
            log += "\n" + "WorldServer created packet size " + _updatePacket;
            Debug.Log(log);
#endif
        }

        #endregion

    }
}

#endif