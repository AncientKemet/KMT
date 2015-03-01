#if SERVER
using Server.Model.Content;

using Server.Model.Extensions;

using Server.Model.Entities;

using System;
using System.Collections.Generic;
using Code.Libaries.UnityExtensions;
using UnityEngine;

namespace Server.Model
{
    public abstract class WorldEntity : ServerMonoBehaviour
    {
        private World _currentWorld = null;

        private long _last_world_tick = 0;
        private CollisionExt _collisionExt = null;

        protected virtual ServerUnitPrioritization GetPrioritization()
        {
            return ServerUnitPrioritization.Low;
        }

        private int _serializedWorldId = -1;

        public int ID
        {
            get { return _id; }
            set { _id = value; }
        }

        private Dictionary<Type, EntityExtension> extensions = new Dictionary<Type, EntityExtension>();

        [SerializeField]
        private int _id;

        public Dictionary<Type, EntityExtension>.ValueCollection Extensions
        {
            get { return extensions.Values; }
        }

        public virtual World CurrentWorld
        {
            get { return _currentWorld; }
            set
            {
                _currentWorld = value;
                _serializedWorldId = value.ID;
                _last_world_tick = -100;
            }
        }

        public void AddExt(EntityExtension extension)
        {
            extension.entity = this;
            extensions[extension.GetType()] = extension;
            if (extension is CollisionExt)
            {
                _collisionExt = extension as CollisionExt;
            }
        }

        public T GetExt<T>() where T : EntityExtension
        {
            if (extensions.ContainsKey(typeof (T)))
            {
                return extensions[typeof (T)] as T;
            }
            return null;
        }

        public CollisionExt CollisionExt { get { return _collisionExt;} }

        public virtual void Progress()
        {
            if (_currentWorld.LAST_TICK < _last_world_tick + (int) GetPrioritization())
            {
                return;
            }

            _last_world_tick = _currentWorld.LAST_TICK;
            foreach (var extension in extensions.Values)
            {
                extension.Progress();
            }
        }

        private void OnDestroy()
        {
            if (CurrentWorld != null)
                CurrentWorld.RemoveEntity(this);
        }

    }
}

#endif