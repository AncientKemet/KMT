using Code.Core.Shared.Content.Types.ItemExtensions;
using Shared.Content.Types.ItemExtensions;
#if SERVER
using System;
using Libaries.Net.Packets.ForClient;
using Server.Model.Entities.Human;

using Shared.Content.Types;

using System.Collections.Generic;
using UnityEngine;
using Server.Model.Entities;

namespace Server.Model.Extensions.UnitExts
{
    public class UnitAttributes : EntityExtension
    {
        private List<BuffInstance> _buffs = new List<BuffInstance>();
        public Dictionary<UnitAttributeProperty, float> Attributes = new Dictionary<UnitAttributeProperty, float>();
        
        public ServerUnit Unit { get; private set; }

        private int _updateTick = 0;

        public override void Progress(float time)
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

            if(Unit.Combat != null)
                Unit.Combat.AttributeHasChanged(type, val);
        }

        public void Add(UnitAttributeProperty type, float val)
        {
            Set(type, Get(type) + val);
        }

        public void Remove(UnitAttributeProperty type, float val)
        {
            Set(type, Get(type) - val);
        }

        public float this[UnitAttributeProperty type] { get { return Get(type); }
            private set { Set(type, value); } }


        protected override void OnExtensionWasAdded()
        {
            base.OnExtensionWasAdded();
            Unit = entity as ServerUnit;

            Add(UnitAttributeProperty.Health, 10);
            Add(UnitAttributeProperty.Energy, 100);
            Add(UnitAttributeProperty.HealthRegen, 0);
            Add(UnitAttributeProperty.EnergyRegen, 0);
            Add(UnitAttributeProperty.CriticalArea, 0.33f);
            Add(UnitAttributeProperty.CriticalDamage, 1.33f);
            Add(UnitAttributeProperty.Armor, 0);
            Add(UnitAttributeProperty.MagicResist, 0);
            Add(UnitAttributeProperty.MovementSpeed, 0);
            Add(UnitAttributeProperty.WeaponReach, 1f);
            Add(UnitAttributeProperty.PhysicalDamage, 0f);
            Add(UnitAttributeProperty.MagicalDamage, 0f);
            Add(UnitAttributeProperty.ChargeSpeed, 0);
            
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
                    if(p != null)
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

        public void AddStats(EquipmentItem item)
        {
            foreach (var attribute in item.Attributes)
            {
                Add(attribute.Property, attribute.Value);
            }
            if (item is Weapon)
            {
                Weapon w = item as Weapon;
                foreach (var attribute in w.Secondary)
                {
                    Add(attribute.Property, attribute.Value);
                }
            }
        }

        public void RemoveStats(EquipmentItem item)
        {
            foreach (var attribute in item.Attributes)
            {
                Remove(attribute.Property, attribute.Value);
            }
            if (item is Weapon)
            {
                Weapon w = item as Weapon;
                foreach (var attribute in w.Secondary)
                {
                    Remove(attribute.Property, attribute.Value);
                }
            }
        }
    }
}
#endif
