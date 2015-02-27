#if SERVER
using System;
using System.Collections.Generic;

namespace Server.Model.Entities.Minerals
{
    public abstract class MineralConfig
    {
        /// <summary>
        /// In content manager index.
        /// </summary>
        public abstract int MotherModel { get; }

        /// <summary>
        /// In content manager index.
        /// </summary>
        public abstract int ChildModel { get; }

        public abstract int BrickItemId { get; }
        public abstract int BrokenItemId { get; }

        /// <summary>
        /// Is applied additionaly as Health.
        /// </summary>
        public abstract float Resistance { get; }

        public static T GetConfig<T>() where T : MineralConfig, new()
        {
            Type type = typeof (T);
            if (_mineralConfigs.ContainsKey(type))
            {
                return _mineralConfigs[type] as T;
            }
            else
            {
                T t = new T();
                _mineralConfigs.Add(type, t);
                return t;
            }
        }

        private static Dictionary<System.Type, MineralConfig> _mineralConfigs = new Dictionary<Type, MineralConfig>();
    }
}
#endif
