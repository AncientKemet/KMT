#if SERVER
using Libaries.Net.Packets.ForClient;

using Shared.Content.Types;
using Code.Code.Libaries.Net;
using Server.Model.Entities.Animals;
using System.Collections.Generic;
using Server.Model.Entities;
using Server.Model.Entities.Human;
using Server.Model.Entities.Items;
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

        public override void Progress(float time)
        {
            if (CurrentCastingSpell != null)
            {
                if (CurrentCastingSpell.HasEnergyCost)
                {
                    Unit.Combat.ReduceEnergy(Time.fixedDeltaTime * CurrentCastingSpell.ChargeEnergyCost * Unit.Combat.EnergyRatio);
                }
                _currentSpellTime += Time.fixedDeltaTime * Unit.Combat.EnergyRatio;
                CurrentCastingSpell.StrenghtChanged(Unit, CurrentCastStrenght);
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
            if (CurrentCastingSpell != null)
            {
                CurrentCastingSpell.CancelCasting(Unit);
                _currentCastingSpellId = -1;
            }
        }

        public void FinishSpell(int id)
        {
            if (_spells[id] != null)
                if (_spells[id] == CurrentCastingSpell)
                {
                    Debug.Log("str " + CurrentCastStrenght);
                    if (CurrentCastingSpell.ActivableDuration < _currentSpellTime)
                        CurrentCastingSpell.FinishCasting(Unit, CurrentCastStrenght);
                    else
                        CurrentCastingSpell.CancelCasting(Unit);
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
            {
                FinishSpell(index);
            }
            else
            {
                StartSpell(index);
            }
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
