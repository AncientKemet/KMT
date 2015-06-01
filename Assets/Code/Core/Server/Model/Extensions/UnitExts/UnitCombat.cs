#region

using System.Collections;
using Libaries.IO;
#if SERVER
using System;
using System.Collections.Generic;
using Shared.Content.Types;
using Server.Model.Entities;
using UnityEngine;

#endregion

namespace Server.Model.Extensions.UnitExts
{
    /// <summary>
    /// This is kinda strange update 
    /// </summary>
    public class UnitCombat : UnitUpdateExt
    {
        private const int RegenUpdateTick = 20;
        private readonly Dictionary<ServerUnit, float> _damageRecieved = new Dictionary<ServerUnit, float>();
        private int RegenTick;
        private bool _dead;
        private UnitAttributes _unitAttributes;
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

        public float CurrenHealth { get; private set; }

        public float CurrentEnergy { get; private set; }

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

        public event Action<float> OnHit;
        public event Action<Spell.DamageType, Spell.HitType, Spell.HitStrenght> OnHitT;
        public event Action<Dictionary<ServerUnit, float>> OnDeath;

        public override void Progress(float time)
        {
            base.Progress(time);

            RegenTick++;
            if (!Dead)
                if (RegenTick >= RegenUpdateTick)
                {
                    RegenTick = 0;

                    CurrentEnergy += UnitAttributes[UnitAttributeProperty.EnergyRegen]*time*RegenUpdateTick;
                    CurrentEnergy = Mathf.Clamp(CurrentEnergy, 0, 100);
                    CurrenHealth += UnitAttributes[UnitAttributeProperty.HealthRegen]*time*RegenUpdateTick;
                    CurrenHealth = Mathf.Clamp(CurrenHealth, 0, 100);
                    _wasUpdate = true;
                }
        }

        public override void Serialize(JSONObject j)
        {
            JSONObject combat = new JSONObject();

            combat.AddField("hp", "" + CurrenHealth);
            combat.AddField("en", "" + CurrentEnergy);

            j.AddField("combat", combat);
        }

        public override void Deserialize(JSONObject j)
        {
            JSONObject combat = j.GetField("combat");

            if (combat.HasField("hp"))
                CurrenHealth = Mathf.Max(float.Parse(combat.GetField("hp").str), 1f);
            if (combat.HasField("en"))
                CurrentEnergy = Mathf.Max(float.Parse(combat.GetField("en").str), 1f);
        }

        protected override void OnExtensionWasAdded()
        {
            base.OnExtensionWasAdded();

            Unit = entity as ServerUnit;
            CurrentEnergy = 100;
            CurrenHealth = 100;

            _wasUpdate = true;
        }

        public override byte UpdateFlag()
        {
            return 0x04;
        }

        internal void ReduceEnergy(float amount)
        {
            CurrentEnergy -= amount;
            CurrentEnergy = Mathf.Clamp(CurrentEnergy, 0, 100);
            _wasUpdate = true;
        }

        internal void ReduceHealth(UnitCombat dealer, float amount)
        {
            if (CurrenHealth > 0)
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
                    OnHit(CurrenHealth - amount);
                }
            }
            CurrenHealth -= amount;

            if (CurrenHealth <= 0)
            {
                if (!Dead)
                {
                    CurrenHealth = Mathf.Clamp(CurrenHealth, 0, 100);
                    _wasUpdate = true;
                    Dead = true;
                    if (OnDeath != null)
                        OnDeath(_damageRecieved);
                    return;
                }
            }

            CurrenHealth = Mathf.Clamp(CurrenHealth, 0, 100);
            _wasUpdate = true;
        }

        internal void Revive(float _health)
        {
            if (Dead)
            {
                Dead = false;
                CurrenHealth = _health;
                CurrenHealth = Mathf.Clamp(CurrenHealth, 0, 100);
                _wasUpdate = true;
            }
        }

        #region Update packet serialization

        protected override void pSerializeState(Code.Code.Libaries.Net.ByteStream packet)
        {
            packet.AddBitArray(new BitArray(new[] {}));
            packet.AddByte((int) CurrenHealth);
            packet.AddByte((int) UnitAttributes.MaxHealth);
            packet.AddByte((int) CurrentEnergy);
            packet.AddByte((int) UnitAttributes.MaxEnergy);
        }

        protected override void pSerializeUpdate(Code.Code.Libaries.Net.ByteStream packet)
        {
            packet.AddByte((int) CurrenHealth);
            packet.AddByte((int) UnitAttributes.MaxHealth);
            packet.AddByte((int) CurrentEnergy);
            packet.AddByte((int) UnitAttributes.MaxEnergy);
        }

        #endregion

        #region Hiteffects

        private void MeleePhysicalHitEffects(Spell.HitStrenght strenght, UnitCombat dealer, float damage)
        {
            float distance = Vector3.Distance(Unit.Movement.Position, dealer.Unit.Movement.Position);
            float distanceFromForward = Vector3.Distance(Unit.Movement.Position + Unit.Movement.Forward,
                                                         dealer.Unit.Movement.Position);

            Unit.Movement.Push((Unit.Movement.Position - dealer.Unit.Movement.Position), (float) strenght);

            if (Unit.Anim != null)
                Unit.Anim.ActionAnimation = "Hit" + (distance < distanceFromForward ? "Back" : "Front");
        }

        public void Hit(Spell.DamageType dmgType, Spell.HitType hitType, Spell.HitStrenght strenght, UnitCombat dealer,
            float damage)
        {
            if (dmgType == Spell.DamageType.Physical)
            {
                MeleePhysicalHitEffects(strenght, dealer, damage);

                float modifiedDamage = (damage*(1.0f + dealer.UnitAttributes[UnitAttributeProperty.PhysicalDamage]))*
                                       (1.0f - Unit.Attributes[UnitAttributeProperty.Armor]);
                ReduceHealth(dealer, modifiedDamage);
            }
            else if (dmgType == Spell.DamageType.Magical)
            {
                ReduceHealth(dealer, (damage*(1.0f + dealer.UnitAttributes[UnitAttributeProperty.MagicalDamage]))*
                                     (1.0f - Unit.Attributes[UnitAttributeProperty.MagicResist]));
            }
            else if (dmgType == Spell.DamageType.True)
            {
                ReduceHealth(dealer, damage);
            }

            if (OnHitT != null)
                OnHitT(dmgType, hitType, strenght);
        }

        #endregion

        #region Attribute update handling

        private Dictionary<UnitAttributeProperty, float> _attrbuteChanges =
            new Dictionary<UnitAttributeProperty, float>();

        private bool _attributeUpdate;

        public void AttributeHasChanged(UnitAttributeProperty property, float value)
        {
            _attributeUpdate = true;
            _attrbuteChanges.Add(property, value);
        }

        #endregion
    }
}

#endif