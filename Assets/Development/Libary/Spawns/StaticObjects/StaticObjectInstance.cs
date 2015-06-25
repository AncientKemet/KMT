using System.Collections.Generic;
using Client.Units;
using Code.Core.Client.Settings;
using Code.Core.Client.Units.Managed;
using Development.Libary.Spawns.StaticObjects;
using Server;
using Server.Model.Entities;
using Server.Model.Entities.StaticObjects;
using Server.Model.Extensions.UnitExts;
using Server.Servers;
using Shared.Content.Types;
using UnityEngine;
using UnityEngine.UI;
using Tree = Server.Model.Entities.StaticObjects.Tree;

namespace Development.Libary.Spawns
{
    public enum StaticObjectType
    {
        Other,
        Plant,
        Tree,
        Mineral
    }

    public class StaticObjectInstance : MonoBehaviour
    {
        public StaticObjectType Type;
        public List<UnitAttributePropertySerializable> AttributePropertySerializables;
        private ushort _id;
        public ushort Id
        {
            get { return _id; }
            set { _id = value; }
        }

        /// <summary>
        /// Is called by prefabinstance.
        /// </summary>
        /// <param name="unitId"></param>
        public void OnBake(ushort unitId)
        {
            Id = unitId;
#if SERVER
            switch (Type)
            {
                case StaticObjectType.Tree:
                    _serverUnit = gameObject.AddComponent<Tree>();
                    break;
                case StaticObjectType.Plant:
                    _serverUnit = gameObject.AddComponent<Plant>();
                    break;
                case StaticObjectType.Mineral:
                    _serverUnit = gameObject.AddComponent<Tree>();
                    break;
                default:
                    _serverUnit = new ServerUnit();
                    break;
            }
            _serverUnit.OnLastSetup += ApplySpawnExtensions;
            _serverUnit.Movement.Teleport(transform.position);
            ServerSingleton.Instance.GetComponent<WorldServer>().World.AddEntity(_serverUnit, Id + GlobalConstants.Instance.STATIC_UNIT_OFFSET);
#endif
#if CLIENT
            _playerUnit = gameObject.AddComponent<PlayerUnit>();
            _playerUnit.Id = (ushort) (Id + GlobalConstants.Instance.STATIC_UNIT_OFFSET);
            UnitManager.Instance[_playerUnit.Id] = _playerUnit;

            //Apply all other client extensions
            foreach (var ce in GetComponents<ClientStaticObjectExtension>())
            {
                ce.Apply(_playerUnit);
            }
#endif
        }

#if SERVER
        private ServerUnit _serverUnit;
#endif
#if CLIENT
        private PlayerUnit _playerUnit;
        
#endif
        
#if SERVER
        void ApplySpawnExtensions()
        {
            if (AttributePropertySerializables.Count > 0)
                _serverUnit.Attributes = _serverUnit.AddExt<UnitAttributes>();

            foreach (var a in AttributePropertySerializables)
            {
                _serverUnit.Attributes.Add(a.Property, a.Value);
            }

            var maxHp = AttributePropertySerializables.Find(a => a.Property == UnitAttributeProperty.Health);
            if (maxHp != null)
            {
                _serverUnit.Combat = _serverUnit.AddExt<UnitCombat>();
                _serverUnit.Combat.Revive(maxHp.Value);
            }

            //Apply all other server extensions
            foreach (var se in GetComponents<ServerStaticObjectExtension>())
            {
                se.Apply(_serverUnit);
            }

        }
#endif
    }
}
