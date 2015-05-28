
#if SERVER
using Libaries.Net.Packets.ForClient;

using Shared.Content.Types;
using System.Collections.Generic;
using Server.Model.Entities;
using Server.Model.Entities.Human;
using UnityEngine;

namespace Server.Model.Extensions.UnitExts
{
    public class UnitSpells : EntityExtension
    {
        public ServerUnit Unit { get; private set; }

        /// <summary>
        /// List of spells. The first four will be placed into players action bars.
        /// </summary>
        private readonly List<Spell> _spells = new List<Spell>(8);

        private int _currentCastingSpellId = -1;
        private float _currentSpellTime;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="time">Time that has passed since last call in seconds. (0.033f usually)</param>
        public override void Progress(float time)
        {
            if (CurrentCastingSpell != null)
            {
                if (CurrentCastingSpell.HasEnergyCost)
                {
                    Unit.Combat.ReduceEnergy(time * CurrentCastingSpell.ChargeEnergyCost * Unit.Combat.EnergyRatio);
                }
                _currentSpellTime += time * Unit.Combat.EnergyRatio;
                CurrentCastingSpell.StrenghtChanged(Unit, CurrentCastStrenght);

                //If the unit is player we'll send them direct Spell strenght Update packet.
                if (Unit is Player)
                {
                    Player p = Unit as Player;

                    SpellUpdatePacket packet = new SpellUpdatePacket();
                    packet.Index = _currentCastingSpellId;
                    packet.IsCasting = true;
                    packet.Strenght = CurrentCastStrenght;

                    p.Client.ConnectionHandler.SendPacket(packet);
                }

            }
        }
        
        public float CurrentCastStrenght
        {
            get { return (Mathf.Min(_currentSpellTime, CurrentCastingSpell.MaxDuration) - CurrentCastingSpell.ActivableDuration) / (CurrentCastingSpell.MaxDuration - CurrentCastingSpell.ActivableDuration); }
        }

        public Spell CurrentCastingSpell
        {
            get
            {
                if (_currentCastingSpellId == -1) { return null; }

                return _spells[_currentCastingSpellId];
            }
        }

        protected override void OnExtensionWasAdded()
        {
            base.OnExtensionWasAdded();
            Unit = entity as ServerUnit;
            for (int i = 0; i < _spells.Capacity; i++)
            {
                _spells.Add(null);
            }
        }

        public void UnEquipSpell(int i)
        {
            _spells[i] = null;

            Player p = Unit as Player;

            //Only if the id is < 4 then the spell should be visible to Player.
            if (p != null && i < 4)
            {
                SpellUpdatePacket packet = new SpellUpdatePacket();

                packet.Index = i;
                packet.IsEnabled = false;
                packet.SetSpell = true;
                packet.Spell = null;

                p.Client.ConnectionHandler.SendPacket(packet);
            }
        }

        public void EquipSpell(Spell spell, int i)
        {
            _spells[i] = spell;

            Player p = Unit as Player;

            //Only if the id is < 4 then the spell should be visible to Player.
            if (p != null && i < 4)
            {
                SpellUpdatePacket packet = new SpellUpdatePacket();

                packet.Index = i;
                packet.IsEnabled = true;
                packet.SetSpell = true;
                packet.Spell = spell;

                p.Client.ConnectionHandler.SendPacket(packet);
            }
        }

        public void StartSpell(int id)
        {
            if (CurrentCastingSpell == null)
                if (_spells[id] != null)
                {
                    _currentCastingSpellId = id;
                    _spells[id].StartCasting(Unit);
                }
        }

        public void CancelSpell(int id)
        {
            if (CurrentCastingSpell == null)
                if (_spells[id] != null)
                {
                    _currentCastingSpellId = id;
                    _spells[id].CancelCasting(Unit);
                }
        }

        public void CancelCurrentSpell()
        {
            CancelSpell(_currentCastingSpellId);
        }

        public void FinishSpell(int id)
        {
            if (_spells[id] != null)
                if (_spells[id] == CurrentCastingSpell)
                {
                    if (CurrentCastingSpell.ActivableDuration < _currentSpellTime)
                        CurrentCastingSpell.FinishCasting(Unit, CurrentCastStrenght);
                    else
                        CancelSpell(id);
                    _currentSpellTime = 0;
                    _currentCastingSpellId = -1;
                }
        }
        
        public bool HasSpell(Spell spell, int index)
        {
            return _spells[index] == spell;
        }

        public void StartOrStopSpell(int index)
        {
            if (CurrentCastingSpell == _spells[index])
                FinishSpell(index);
            else
                StartSpell(index);
        }

        public void UnEquipSpells(Spell[] spells)
        {
            foreach (var spell in spells)
            {
                UnEquipSpell(_spells.IndexOf(spell));
            }
        }

        public void EquipSpells(Spell[] spells)
        {
            for (int i = 0; i < spells.Length; i++)
            {
                if (spells[i] != null)
                    EquipSpell(spells[i], i);
            }
        }
    }
}
#endif
