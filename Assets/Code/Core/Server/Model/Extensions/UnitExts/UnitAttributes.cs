#if SERVER
using System;
using Libaries.Net.Packets.ForClient;
using Server.Model.Entities.Human;

using Shared.Content.Types;

using System.Collections.Generic;
using Code.Code.Libaries.Net;
using UnityEngine;
using Server.Model.Entities;

namespace Server.Model.Extensions.UnitExts
{
    public class UnitAttributes : EntityExtension
    {
        private List<BuffInstance> _buffs = new List<BuffInstance>();
        public Dictionary<UnitAttributeProperty, float> Attributes = new Dictionary<UnitAttributeProperty, float>();

        public float Strenght { get; set; }
        public float Dexterity { get; set; }
        public float Wisdom { get; set; }

        public float MaxHealth { get { return 100 + (Strenght * (1.0f + Get(UnitAttributeProperty.StrenghtBonus))) * 0.33f; } }
        public float MaxEnergy { get { return 100 + Dexterity * 0.33f; } }

        public ServerUnit Unit { get; private set; }

        public float BaseArmor { get; set; }

        private int _updateTick = 0;

        public override void Progress()
        {
            if (_buffs.Count > 0)
            {
                var expireds = _buffs.FindAll(instance => instance.Expired);
                foreach (var expired in expireds)
                {
                    RemoveBuff(expired);
                }
            }
        }

        public float Get(UnitAttributeProperty type)
        {
            if (Attributes.ContainsKey(type))
            {
                return Attributes[type];
            }
            Attributes.Add(type, 0);
            return 0;
        }
        public void Set(UnitAttributeProperty type, float val)
        {
            if (Attributes.ContainsKey(type))
                Attributes[type] = val;
            else
                Attributes.Add(type, val);
        }
        public float this[UnitAttributeProperty type] { get { return Get(type); } set { Set(type, value); } }

        public override void Serialize(ByteStream bytestream)
        { }

        public override void Deserialize(ByteStream bytestream)
        { }

        protected override void OnExtensionWasAdded()
        {
            base.OnExtensionWasAdded();
            Unit = entity as ServerUnit;

            OnBuffUpdate += (instance, b) =>
            {
                var pa = new BuffUpdatePacket();
                pa.UnitId = Unit.ID;
                pa.AddOrRemove = b;
                pa.BuffInstance = instance;

                if (Unit is Player)
                {
                    (Unit as Player).Client.ConnectionHandler.SendPacket(pa);
                }

                foreach (var listener in Unit.Focus.Listeners)
                {
                    if(listener == null || !(listener is Player) && listener == Unit)
                        continue;

                    Player p = listener as Player;

                    p.Client.ConnectionHandler.SendPacket(pa);
                }
            };
        }

        public void AddBuff(Buff buff, float duration)
        {
            if (buff.Stackable)
            {
                var b = _buffs.Find(instance => instance.Buff == buff);
                b.Stacks ++;
                b.Duration = duration;
                foreach (var a in buff.Attributes)
                {
                    this[a.Property] += a.Value;
                }
                if (OnBuffUpdate != null)
                    OnBuffUpdate(b, true);
            }
            else
            {
                var b = new BuffInstance(_buffs.Count, Time.time, buff, duration);
                _buffs.Add(b);
                
                foreach (var a in buff.Attributes)
                {
                    this[a.Property] += a.Value;
                }
                
                if (OnBuffUpdate != null)
                    OnBuffUpdate(b, true);
            }
        }

        public void AddBuff(Buff buff)
        {
            AddBuff(buff, 999999);
        }

        public void RemoveBuff(Buff buff)
        {
            var b = _buffs.Find(instance => instance.Buff == buff);
            _buffs.Remove(b);
            if (OnBuffUpdate != null)
                OnBuffUpdate(b, false);
            foreach (var a in buff.Attributes)
            {
                this[a.Property] -= a.Value;
            }
        }

        public void RemoveBuff(BuffInstance buff)
        {
            _buffs.Remove(buff);
            if (OnBuffUpdate != null)
                OnBuffUpdate(buff, false);
            foreach (var a in buff.Buff.Attributes)
            {
                this[a.Property] -= a.Value;
            }
        }

        /// <summary>
        /// The bool stands for added/removed, eq false=buff has been removed.
        /// </summary>
        public Action<BuffInstance, bool> OnBuffUpdate;

    }
}
#endif
