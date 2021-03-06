using System;
using System.Collections;
using Code.Core.Client.Settings;
using Server.Servers;
#if SERVER
using Server.Model.Content;

using Client.Enviroment;
using Server.Model.ContentHandling;
using System.Collections.Generic;
using Code.Libaries.Generic.Trees;
using Server.Model.Entities;
using Server.Model.Entities.Human;
using UnityEngine;

namespace Server.Model
{
    public class World : ServerMonoBehaviour
    {
        public WorldServer WorldServer { get; set; }
        public KemetMap Map { get; private set; }
        public long LAST_TICK = 0;
        public List<WorldEntity> _entities = new List<WorldEntity>();
        public List<ServerUnit> Units;
        public List<Player> Players;

        private List<int> _freeIds = new List<int>(8);

        private bool _initialized = false;

        public QuadTree Tree;

        public void AddEntity(WorldEntity entity, int _forcedId = -1)
        {
            EnsureInitialization();

            bool _foundNullIndex = false;
            int _nullIndex = -1;

            if (_forcedId != -1)
            {
                _foundNullIndex = true;
                _nullIndex = _forcedId;
            }
            else
                if (_freeIds.Count > 0)
                {
                    _nullIndex = _freeIds[0];
                    _freeIds.RemoveAt(0);
                    _foundNullIndex = true;
                }

            if (!_foundNullIndex && _forcedId == -1)
            {
                _entities.Add(entity);
                entity.ID = (ushort) (_entities.Count - 1);
            }
            else
            {
                if(_entities[_nullIndex] != null)
                    throw new Exception("Fuck at this Id there already is an unit! " +_nullIndex);
                _entities[_nullIndex] = entity;
                entity.ID = (ushort) _nullIndex;
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
        }

        public void RemoveEntity(WorldEntity entity)
        {
            try
            {
                _freeIds.Add(entity.ID);
                _entities[entity.ID] = null;
            }
            catch (ArgumentOutOfRangeException e)
            {
                Debug.LogError("ArgumentOutOfRangeException [" + entity.ID + "] " + entity.name);
            }
        }

        public void Progress(float time)
        {
            LAST_TICK++;

            var progressId = (byte) LAST_TICK;
            foreach (var player in Players)
            {
                foreach (var o in player.CurrentBranch.ObjectsVisible)
                {
                    ServerUnit e = o as ServerUnit;
                    if (e != null)
                    {
                        e.ProgressOnce(time, progressId);
                    }
                }
            }
            if (LAST_TICK % 5 == 0)
            {
                LAST_TICK = 0;
                Tree.Update();
            }
        }

        #region Constructor

        private void Start()
        {
            ServerSpawnManager.Instance(this);
            EnsureInitialization();
        }

        private void EnsureInitialization()
        {
            if (_initialized)
                return;

            _initialized = true;

            _entities = new List<WorldEntity>(new WorldEntity[GlobalConstants.Instance.MAX_UNIT_AMOUNT]);
            Units = new List<ServerUnit>();
            Players = new List<Player>();
            

            Map = KemetMap.GetMap("1");
            Map.transform.parent = transform;
            MapQuadTree mapQuadTree = Map.GetComponent<MapQuadTree>();
            Tree = new QuadTree(mapQuadTree._divisions, mapQuadTree._position, mapQuadTree._size);
        }

        #endregion

        public int ID { get; set; }

        public ServerUnit this[int unitId]
        {
            get
            {
                if (unitId > _entities.Count - 1 || unitId < 0)
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
                    Debug.Log("Saving player : " + player.Client.UserAccount.Username);
                    player.Client.UserAccount.SaveAccount();
                    player.Client.UserAccount.SaveUnit(player);
                }
            }
        }

        public void ScanGraph()
        {
            StartCoroutine(_ScanGraph());
        }

        private IEnumerator _ScanGraph()
        {
            Map.GetComponent<MapQuadTree>().enabled = false;
            foreach (var i in Map.GetComponentsInChildren<MapQuadTree>(true))
            {
                i.gameObject.SetActive(true);
                i.enabled = false;
            }
            yield return new WaitForEndOfFrame();
            Tree.Scan();
            yield return new WaitForEndOfFrame();
            foreach (var i in Map.GetComponentsInChildren<MapQuadTree>(true))
            {
                i.enabled = true;
            }
        }
    }
}

#endif
