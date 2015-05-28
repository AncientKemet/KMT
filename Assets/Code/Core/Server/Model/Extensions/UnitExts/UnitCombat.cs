using Libaries.IO;
#if SERVER
using System;
using System.Collections.Generic;
using Shared.Content.Types;
using Server.Model.Entities;
using UnityEngine;

namespace Server.Model.Extensions.UnitExts
{
    public class UnitCombat : UnitUpdateExt
    {

        public ServerUnit Unit { get; private set; }

        private UnitAttributes UnitAttributes
        {
            get
            {
                if (_unitAttributes == null && Unit != null)
                {
                    _unitAttributes = Unit.GetExt<UnitAttributes>();
                }
                return _unitAttributes;
            }
        }

        public event Action<float> OnHit;
        public event Action<Spell.DamageType, Spell.HitType, Spell.HitStrenght> OnHitT;
        public event Action<Dictionary<ServerUnit, float>> OnDeath;

        public float Health { get; private set; }
        public float Energy { get; private set; }
        public float EnergyRatio { get { return Energy / 100f; } }

        public bool Dead
        {
            get { return _dead; }
            private set
            {
                if (_dead != value)
                {
                    if (value)
                    {
                        if (Unit.Anim != null)
                        {
                            Unit.Anim.SetDefaults();
                            Unit.Anim.StandAnimation = "Death";
                        }
                    }
                    else
                    {
                        Unit.Anim.SetDefaults();
                    }
                }
                _dead = value;
            }
        }

        private int RegenTick = 0;

        private UnitAttributes _unitAttributes;
        private readonly Dictionary<ServerUnit, float> _damageRecieved = new Dictionary<ServerUnit, float>();
        private bool _dead;

        const int RegenUpdateTick = 20;

        public override void Progress(float time)
        {
            base.Progress(time);

            RegenTick++;
            if (!Dead)
                if (RegenTick >= RegenUpdateTick)
                {
                    RegenTick = 0;

                    Energy += UnitAttributes[UnitAttributeProperty.EnergyRegen] * time * RegenUpdateTick;
                    Energy = Mathf.Clamp(Energy, 0, 100);
                    Health += UnitAttributes[UnitAttributeProperty.HealthRegen] * time * RegenUpdateTick;
                    Health = Mathf.Clamp(Health, 0, 100);
                    _wasUpdate = true;
                }
        }

        public override void Serialize(JSONObject j)
        {
            JSONObject combat = new JSONObject();

            combat.AddField("hp", "" + Health);
            combat.AddField("en", "" + Energy);

            j.AddField("combat", combat);
        }

        public override void Deserialize(JSONObject j)
        {
            JSONObject combat = j.GetField("combat");

            if (combat.HasField("hp"))
                Health = Mathf.Max(float.Parse(combat.GetField("hp").str), 1f);
            if (combat.HasField("en"))
                Energy = Mathf.Max(float.Parse(combat.GetField("en").str), 1f);
        }

        protected override void OnExtensionWasAdded()
        {
            base.OnExtensionWasAdded();

            Unit = entity as ServerUnit;
            Energy = 100;
            Health = 100;

            _wasUpdate = true;
        }

        public override byte UpdateFlag()
        {
            return 0x04;
        }

        protected override void pSerializeState(Code.Code.Libaries.Net.ByteStream packet)
        {
            packet.AddByte((int)Health);
            packet.AddByte((int)UnitAttributes.MaxHealth);
            packet.AddByte((int)Energy);
            packet.AddByte((int)UnitAttributes.MaxEnergy);
        }

        protected override void pSerializeUpdate(Code.Code.Libaries.Net.ByteStream packet)
        {
            packet.AddByte((int)Health);
            packet.AddByte((int)UnitAttributes.MaxHealth);
            packet.AddByte((int)Energy);
            packet.AddByte((int)UnitAttributes.MaxEnergy);
        }

        internal void ReduceEnergy(float amount)
        {
            Energy -= amount;
            Energy = Mathf.Clamp(Energy, 0, 100);
            _wasUpdate = true;
        }

        internal void ReduceHealth(UnitCombat dealer, float amount)
        {
            if (Health > 0)
            {
                if (_damageRecieved.ContainsKey(dealer.Unit))
                {
                    _damageRecieved[dealer.Unit] += amount;
                }
                else
                {
                    _damageRecieved.Add(dealer.Unit, amount);
                }
            }
            ReduceHealth(amount);
        }

        internal void ReduceHealth(float amount)
        {
            if (amount != null)
            {
                if (OnHit != null)
                {
                    OnHit(Health - amount);
                }
            }
            Health -= amount;

            if (Health <= 0)
            {
                if (!Dead)
                {
                    Health = Mathf.Clamp(Health, 0, 100);
                    _wasUpdate = true;
                    Dead = true;
                    if(OnDeath != null)
                        OnDeath(_damageRecieved);
                    return;
                }
            }

            Health = Mathf.Clamp(Health, 0, 100);
            _wasUpdate = true;
        }

        internal void Revive(float _health)
        {
            if (Dead)
            {
                Dead = false;
                Health = _health;
                Health = Mathf.Clamp(Health, 0, 100);
                _wasUpdate = true;
            }
        }

        private void MeleePhysicalHitEffects(Spell.HitStrenght strenght, UnitCombat dealer, float damage)
        {
            float distance = Vector3.Distance(Unit.Movement.Position, dealer.Unit.Movement.Position);
            float distanceFromForward = Vector3.Distance(Unit.Movement.Position + Unit.Movement.Forward, dealer.Unit.Movement.Position);

            Unit.Movement.Push((Unit.Movement.Position - dealer.Unit.Movement.Position), (float)strenght);

            if (Unit.Anim != null)
                Unit.Anim.ActionAnimation = "Hit" + (distance < distanceFromForward ? "Back" : "Front");
        }

        public void Hit(Spell.DamageType dmgType, Spell.HitType hitType, Spell.HitStrenght strenght, UnitCombat dealer, float damage)
        {
            if (dmgType == Spell.DamageType.Physical)
            {
                
                    MeleePhysicalHitEffects(strenght, dealer, damage);

                float modifiedDamage = (damage * (1.0f + dealer.UnitAttributes[UnitAttributeProperty.PhysicalDamage])) *
                                        (1.0f - Unit.Attributes[UnitAttributeProperty.Armor]);
                ReduceHealth(dealer, modifiedDamage);
            }
            else if (dmgType == Spell.DamageType.Magical)
            {
                ReduceHealth(dealer, (damage * (1.0f + dealer.UnitAttributes[UnitAttributeProperty.MagicalDamage])) *
                                        (1.0f - Unit.Attributes[UnitAttributeProperty.MagicResist]));
            }
            else if (dmgType == Spell.DamageType.True)
            {
                ReduceHealth(dealer, damage);
            }

            if (OnHitT != null)
                OnHitT(dmgType, hitType, strenght);
        }
    }
}
#endif
