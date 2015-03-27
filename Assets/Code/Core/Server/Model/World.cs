using Server.Servers;
#if SERVER
using Server.Model.Content;

using Client.Enviroment;
using Server.Model.Pathfinding;

using Server.Model.ContentHandling;
using System.Collections.Generic;
using Code.Libaries.Generic.Trees;
using Code.Libaries.UnityExtensions;
using Server.Model.Entities;
using Server.Model.Entities.Human;
using UnityEngine;

namespace Server.Model
{
    public class World : ServerMonoBehaviour
    {
        public WorldServer WorldServer { get; set; }
        public KemetMap Map { get; private set; }
        public bool EnableVegetation = false;
        public long LAST_TICK = 0;
        public List<WorldEntity> _entities = new List<WorldEntity>();
        public List<ServerUnit> Units;
        public List<Player> Players;

        private List<int> _freeIds = new List<int>(8);

        private bool _initialized = false;

        public QuadTree Tree;

        public void AddEntity(WorldEntity entity)
        {
            EnsureInitialization();

            bool _foundNullIndex = false;
            int _nullIndex = -1;

            if (_freeIds.Count > 0)
            {
                _nullIndex = _freeIds[0];
                _freeIds.RemoveAt(0);
                _foundNullIndex = true;
            }
            
            if (!_foundNullIndex)
            {
                _entities.Add(entity);
                entity.ID = _entities.Count - 1;
            }
            else
            {
                _entities[_nullIndex] = entity;
                entity.ID = _nullIndex;
            }

            entity.CurrentWorld = this;

            if (entity is ServerUnit)
            {
                ServerUnit serverUnit = entity as ServerUnit;
                Units.Add(serverUnit);
                Tree.AddObject(serverUnit);
            }

            if (entity is Player)
            {
                Player player = entity as Player;
                Players.Add(player);
                player.OnEnteredWorld(this);
            }

            entity.transform.parent = transform;
        }

        public void RemoveEntity(WorldEntity entity)
        {
            _freeIds.Add(entity.ID);
            _entities[entity.ID] = null;
        }

        public void Progress()
        {
            LAST_TICK++;
            
            foreach (var player in Players)
            {
                foreach (var o in player.CurrentBranch.ActiveObjectsVisible)
                {
                    ServerUnit e = o as ServerUnit;
                    if (e != null)
                    {
                        e.Progress();
                    }
                }
            }
            if(LAST_TICK % 20 == 0)
                Tree.Update();
        }

        #region Constructor

        private void Start()
        {
            AstarPath.Instance.Scan();

            EnsureInitialization();
        }

        private void EnsureInitialization()
        {
            if (_initialized)
                return;

            _initialized = true;

            _entities = new List<WorldEntity>(1024);
            Units = new List<ServerUnit>();
            Players = new List<Player>();
            Tree = new QuadTree(5, Vector2.zero, Vector2.one*1024);

            Map = KemetMap.GetMap("1");
            Map.transform.parent = transform;
            if (EnableVegetation)
                WorldVegeationManager.Instance(this);
        }

        #endregion

        public int ID { get; set; }

        public ServerUnit this[int unitId]
        {
            get
            {
                if (unitId > _entities.Count-1 || unitId < 0)
                {
                    //throw  new Exception("Wrong unit index: "+unitId);
                    return null;
                }
                return _entities[unitId] as ServerUnit;
            }
        }

        public void Save()
        {
            Debug.Log("Saving world.");
            foreach (var player in Players)
            {
                
                if (player.Client.UserAccount != null)
                {
                    Debug.Log("Saving player : "+player.Client.UserAccount.Username);
                    player.Client.UserAccount.SaveAccount();
                    player.Client.UserAccount.SaveUnit(player);
                }
            }
        }
    }
}

#endif
