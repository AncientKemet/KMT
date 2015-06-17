using System.Collections.Generic;
using Client.Units;
using Code.Core.Client.Units.Managed;
using Server;
using Server.Model.Entities;
using Server.Model.Entities.Vegetation;
using Server.Servers;
using Shared.Content.Types;
using UnityEngine;
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

        public int Id { get; set; }

        /// <summary>
        /// Is called by prefabinstance by send message.
        /// </summary>
        /// <param name="unitId"></param>
        public void OnBake(int unitId)
        {
            Id = unitId;
        }

#if SERVER
        private ServerUnit _serverUnit;
#endif
#if CLIENT
        private PlayerUnit _playerUnit;
#endif

        void Awake()
        {
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
            ServerSingleton.Instance.GetComponent<WorldServer>().World.AddEntity(_serverUnit, Id);
#endif
#if CLIENT
            _playerUnit = gameObject.AddComponent<PlayerUnit>();
#endif
        }

#if SERVER
        void ApplySpawnExtensions()
        {
            _serverUnit.Movement.Teleport(transform.position);
            _serverUnit.Movement.Rotation = transform.eulerAngles.y;
            foreach (var a in AttributePropertySerializables)
            {
                _serverUnit.Attributes.Add(a.Property, a.Value);
            }
        }
#endif
    }
}
