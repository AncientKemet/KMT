
using System.Collections;
using Libaries.IO;
using Libaries.Net.Packets.ForClient;
using Server.Model.Entities.Human;
using Shared.StructClasses;
#if SERVER
using System;
using System.Collections.Generic;
using Shared.Content.Types;
using Server.Model.Entities;
using UnityEngine;


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

        public new CapsuleCollider collider;

        #region Current HP & EN

        private bool _currentHpUpdate, _currentEnUpdate;
        private float _currenHealth;
        private float _currentEnergy;

        public float CurrenHealth
        {
            get { return _currenHealth; }
            private set
            {
                _currenHealth = value;
                _currentHpUpdate = true;
                _wasUpdate = true;
            }
        }

        public float CurrentEnergy
        {
            get { return _currentEnergy; }
            private set
            {
                _currentEnergy = value;
                _currentEnUpdate = true;
                _wasUpdate = true;
            }
        }

        #endregion

        public bool Dead
        {
            get { return _dead; }
            set
            {
                if (_dead != value)
                {
                    if (value)
                    {
                        if (Unit.Anim != null)
                        {
                            Unit.Anim.SetDefaults();
                            Unit.Anim.StandAnimation = "Dead";
                            Unit.Anim.ActionAnimation = "Death";
                        }
                        if (Unit.Focus != null)
                        {
                            Unit.Focus.FocusedUnit = null;
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

        public event Action<HitInformation> OnHitOther;
        public event Action<HitInformation> OnHitMe;
        public event Action<Dictionary<ServerUnit, float>> OnDeath;

        public override void Progress(float time)
        {
            base.Progress(time);

            RegenTick++;
            if (!Dead)
                if (RegenTick >= RegenUpdateTick)
                {
                    RegenTick = 0;

                    CurrentEnergy += Unit.Attributes[UnitAttributeProperty.EnergyRegen] * time * RegenUpdateTick;
                    CurrentEnergy = Mathf.Clamp(CurrentEnergy, 0, 100);
                    CurrenHealth += Unit.Attributes[UnitAttributeProperty.HealthRegen] * time * RegenUpdateTick;
                    CurrenHealth = Mathf.Clamp(CurrenHealth, 0, 100);
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
            CurrentEnergy = 1;
            CurrenHealth = 1;

            collider = gameObject.AddComponent<CapsuleCollider>();
            collider.radius = 0.5f;
            collider.center = new Vector3(0, 0.5f, 0);
            collider.height = 2;

            gameObject.layer = 30;

            Player p = Unit as Player;
            if (p != null)
            {
                //this unit is player let's listen to damage actions and send damage packets.
                OnHitMe += (i) =>
                {
                    DamagePacket packet = new DamagePacket();

                    packet.UnitId = i.targetID;
                    packet.DamageType = i.DamageType;
                    packet.HitType = i.HitType;
                    packet.Strenght = i.Strenght;
                    packet.Damage = (int) i.Damage;

                    p.Client.ConnectionHandler.SendPacket(packet);
                };

                OnHitOther += (i) =>
                {
                    DamagePacket packet = new DamagePacket();

                    packet.UnitId = i.targetID;
                    packet.DamageType = i.DamageType;
                    packet.HitType = i.HitType;
                    packet.Strenght = i.Strenght;
                    packet.Damage = (int)i.Damage;

                    p.Client.ConnectionHandler.SendPacket(packet);
                };

                OnHitOther += (i) => p.Levels.AddExperience(Levels.Skills.Attack, (int) (i.Damage * Levels.DamageXpRatio));
            }
        }

        public override byte UpdateFlag()
        {
            return 0x04;
        }

        internal void ReduceEnergy(float amount)
        {
            _currentEnergy -= amount;
            _currentEnergy = Mathf.Clamp(CurrentEnergy, 0, Unit.Attributes[UnitAttributeProperty.Energy]);
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

        private void ReduceHealth(float amount)
        {
            CurrenHealth -= amount;

            if (CurrenHealth <= 0)
            {
                if (!Dead)
                {
                    CurrenHealth = Mathf.Clamp(CurrenHealth, 0, Unit.Attributes[UnitAttributeProperty.Health]);
                    Dead = true;
                    if (OnDeath != null)
                        OnDeath(_damageRecieved);
                    return;
                }
            }

            CurrenHealth = Mathf.Clamp(CurrenHealth, 0, Unit.Attributes[UnitAttributeProperty.Health]);
        }

        internal void Revive(float _health)
        {
            if (Dead)
            {
                Dead = false;
                CurrenHealth = _health;
                CurrenHealth = Mathf.Clamp(CurrenHealth, 0, Unit.Attributes[UnitAttributeProperty.Health]);
            }
        }

        #region Update packet serialization

        protected override void pSerializeState(Code.Code.Libaries.Net.ByteStream packet)
        {
            packet.AddFlag(new[] { true, true, true });

            packet.AddByte((int)CurrenHealth);

            packet.AddByte((int)CurrentEnergy);

            packet.AddByte(Unit.Attributes.Attributes.Count);

            foreach (KeyValuePair<UnitAttributeProperty, float> change in Unit.Attributes.Attributes)
            {
                packet.AddByte((int)change.Key);
                packet.AddShort((int)(change.Value * 100f));
            }
        }

        protected override void pSerializeUpdate(Code.Code.Libaries.Net.ByteStream packet)
        {
            packet.AddFlag(new[] { _currentHpUpdate, _currentEnUpdate, _attributeUpdate });

            if (_currentHpUpdate)
                packet.AddByte((int)CurrenHealth);

            if (_currentEnUpdate)
                packet.AddByte((int)CurrentEnergy);

            if (_attributeUpdate)
            {
                packet.AddByte(_attrbuteChanges.Count);

                foreach (KeyValuePair<UnitAttributeProperty, float> change in _attrbuteChanges)
                {
                    packet.AddByte((int)change.Key);
                    packet.AddShort((int)(change.Value * 100f));
                }

                //Clean the shit out!
                _attrbuteChanges = new Dictionary<UnitAttributeProperty, float>();
            }

            _currentHpUpdate = false;
            _currentEnUpdate = false;
            _attributeUpdate = false;
        }

        #endregion

        #region Hiteffects

        private void MeleePhysicalHitEffects(Spell.HitStrenght strenght, UnitCombat dealer, float damage)
        {
            float distance = Vector3.Distance(Unit.Movement.Position, dealer.Unit.Movement.Position);
            float distanceFromForward = Vector3.Distance(Unit.Movement.Position + Unit.Movement.Forward,
                                                         dealer.Unit.Movement.Position);

            Unit.Movement.Push((Unit.Movement.Position - dealer.Unit.Movement.Position), ((float)strenght) / 3f);

            if (Unit.Anim != null)
                Unit.Anim.ActionAnimation = "Hit" + (distance < distanceFromForward ? "Back" : "Front");
        }

        public void Hit(Spell.DamageType dmgType, Spell.HitType hitType, Spell.HitStrenght strenght, UnitCombat dealer,
            float damage)
        {
            if(Dead)
                return;
            
            if (dmgType == Spell.DamageType.Physical)
            {
                damage = (damage * (1.0f + dealer.Unit.Attributes[UnitAttributeProperty.PhysicalDamage])) *
                                       (1.0f - Unit.Attributes[UnitAttributeProperty.Armor]);
                MeleePhysicalHitEffects(strenght, dealer, damage);
                ReduceHealth(dealer, damage);
            }
            else if (dmgType == Spell.DamageType.Magical)
            {
                damage = (damage * (1.0f + dealer.Unit.Attributes[UnitAttributeProperty.MagicalDamage])) *
                                       (1.0f - Unit.Attributes[UnitAttributeProperty.MagicResist]);
                ReduceHealth(dealer, damage);
            }
            else if (dmgType == Spell.DamageType.True)
            {
                ReduceHealth(dealer, damage);
            }

            var information = new HitInformation(dealer.Unit.ID, Unit.ID, dmgType, hitType, strenght, damage);

            if (OnHitMe != null)
                OnHitMe(information);
            if(dealer.OnHitOther != null)
                dealer.OnHitOther(information);
        }

        #endregion

        #region Attribute update handling

        private Dictionary<UnitAttributeProperty, float> _attrbuteChanges =
            new Dictionary<UnitAttributeProperty, float>();

        private bool _attributeUpdate;

        public void AttributeHasChanged(UnitAttributeProperty property, float value)
        {
            _attributeUpdate = true;
            _wasUpdate = true;
            if (_attrbuteChanges.ContainsKey(property))
                _attrbuteChanges[property] = value;
            else
                _attrbuteChanges.Add(property, value);
        }

        #endregion

        public class HitInformation
        {
            public int dealerID, targetID;
            public Spell.DamageType DamageType;
            public Spell.HitType HitType;
            public Spell.HitStrenght Strenght;
            public float Damage;

            public HitInformation(int id, int i, Spell.DamageType dmgType, Spell.HitType hitType, Spell.HitStrenght strenght, float damage)
            {
                this.dealerID = id;
                this.targetID = i;
                this.DamageType = dmgType;
                this.HitType = hitType;
                this.Strenght = strenght;
                this.Damage = damage;
            }
        }
    }
}

#endif