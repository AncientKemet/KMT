using System;
using Development.Libary.Spells.Codes;
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
        /// 
        /// 5 = rest spell
        /// 6 = crafting spell
        /// </summary>
        private readonly List<Spell> _spells = new List<Spell>(8);
        private readonly bool[] _spellsEnabled = new bool[8];

        private int _currentCastingSpellId = -1;
        private float _currentSpellTime;
        private float _globalCooldown = 1f;
        private int _startQueuedSpell = -1;
        private float _craftingSpeedModifier = 1f;

        private bool _forceFinish = false;

        public Vector3 TargetPosition { get; set; }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="time">Time that has passed since last call in seconds. (0.033f usually)</param>
        public override void Progress(float time)
        {
            //Basically if the cooldown is > 0 we reduce it, and if after thre reducement its < 0 then we start casting the queued spell
            if (_globalCooldown > 0)
            {
                _globalCooldown -= time * (1f + Unit.Attributes[UnitAttributeProperty.ChargeSpeed]);
                if (_globalCooldown < 0)
                {
                    if (_startQueuedSpell != -1)
                    {
                        StartSpell(_startQueuedSpell);
                        _startQueuedSpell = -1;
                    }
                }
            }
            if (CurrentCastingSpell != null)
            {
                if (CurrentCastingSpell is CraftingSpell)
                {
                    _currentSpellTime += time * _craftingSpeedModifier;
                    CurrentCastingSpell.StrenghtChanged(Unit, CurrentCastStrenght);

                    if (CurrentCastStrenght >= 0.99f)
                    {
                        FinishCrafting();
                    }
                }
                else
                {
                    if (CurrentCastingSpell.HasEnergyCost)
                    {
                        Unit.Combat.ReduceEnergy(time * CurrentCastingSpell.ChargeEnergyCost);
                    }
                    _currentSpellTime += time * (1f + (Unit.Attributes[UnitAttributeProperty.ChargeSpeed]));
                    CurrentCastingSpell.StrenghtChanged(Unit, CurrentCastStrenght);
                }


                //If the unit is player we'll send them direct Spell strenght Update packet.
                if (Unit is Player)
                {
                    Player p = Unit as Player;

                    SpellUpdatePacket packet = new SpellUpdatePacket();
                    packet.Index = _currentCastingSpellId;
                    packet.UpdateState = SpellUpdateState.StrenghtChange;
                    packet.Strenght = CurrentCastStrenght;

                    p.Client.ConnectionHandler.SendPacket(packet);
                }

                if (CurrentCastingSpell is CraftingSpell)
                {
                    if (CurrentCastStrenght >= 0.99f)
                    {
                        FinishSpell(_currentCastingSpellId);
                        FinishCrafting();
                    }
                }

                if (_forceFinish)
                {
                    FinishSpell(_currentCastingSpellId);
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
                packet.UpdateState = SpellUpdateState.SetSpell;
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
                packet.UpdateState = SpellUpdateState.SetSpell;
                packet.Spell = spell;

                p.Client.ConnectionHandler.SendPacket(packet);
            }

            if (spell.Validate(Unit))
            {
                EnableSpell(i);
            }
            else
            {
                DisableSpell(i);
            }
        }

        public void StartSpell(int id)
        {
            if (_globalCooldown > 0)
            {
                _startQueuedSpell = id;
            }
            else
            {
                if (CurrentCastingSpell == null)
                    if (_spells[id] != null && _spellsEnabled[id])
                    {
                        _currentCastingSpellId = id;
                        _spells[id].StartCasting(Unit);
                        if (Unit is Player)
                        {
                            Player p = Unit as Player;

                            SpellUpdatePacket packet = new SpellUpdatePacket();
                            packet.Index = _currentCastingSpellId;
                            packet.UpdateState = SpellUpdateState.StartedCasting;

                            p.Client.ConnectionHandler.SendPacket(packet);
                        }
                    }
            }
        }

        public void CancelSpell(int id)
        {
            if (CurrentCastingSpell == null)
                if (_spells[id] != null)
                {
                    _globalCooldown = 1f;
                    _currentCastingSpellId = id;
                    _spells[id].CancelCasting(Unit);

                    if (Unit is Player)
                    {
                        Player p = Unit as Player;

                        SpellUpdatePacket packet = new SpellUpdatePacket();
                        packet.Index = _currentCastingSpellId;
                        packet.UpdateState = SpellUpdateState.FinishedCasting;

                        p.Client.ConnectionHandler.SendPacket(packet);
                    }

                    _currentSpellTime = 0;
                    _currentCastingSpellId = -1;
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
                    {
                        _forceFinish = false;

                        _globalCooldown = 1f;
                        CurrentCastingSpell.FinishCasting(Unit, CurrentCastStrenght);
                        if (Unit is Player)
                        {
                            Player p = Unit as Player;

                            SpellUpdatePacket packet = new SpellUpdatePacket();
                            packet.Index = _currentCastingSpellId;
                            packet.UpdateState = SpellUpdateState.FinishedCasting;

                            p.Client.ConnectionHandler.SendPacket(packet);
                        }
                        _currentSpellTime = 0;
                        _currentCastingSpellId = -1;
                    }
                    else
                    {
                        _forceFinish = true;
                    }
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

        #region Crafting and crafting spells

        private Action _onFinishCraftinAction;

        //This is being called whenever the crafting should be interrupted
        public void CraftInterupt()
        {
            if (CurrentCastingSpell != null)
                if (CurrentCastingSpell is CraftingSpell)
                {
                    CancelCurrentSpell();
                    _onFinishCraftinAction = null;
                }
        }

        public void StartCrafting(ItemRecipe itemRecipe, Action OnFinish)
        {
            if (CurrentCastingSpell != null)
                CancelCurrentSpell();
            EquipSpell(itemRecipe.CraftingSpell, 6);
            StartSpell(6);
            _craftingSpeedModifier = 1f / itemRecipe.CraftTime;
            _onFinishCraftinAction = OnFinish;
        }

        public void FinishCrafting()
        {
            if (CurrentCastingSpell is CraftingSpell)
            {
                if (_onFinishCraftinAction != null)
                {
                    _onFinishCraftinAction();
                    _onFinishCraftinAction = null;
                }
            }
        }

        #endregion

        public void DisableMainHandSpells()
        {
            DisableSpell(0);
            DisableSpell(1);
        }

        public void DisableSpell(int id)
        {
            _spellsEnabled[id] = false;

            if (Unit is Player)
            {
                Player p = Unit as Player;

                SpellUpdatePacket packet = new SpellUpdatePacket();
                packet.Index = id;
                packet.UpdateState = SpellUpdateState.Disable;

                p.Client.ConnectionHandler.SendPacket(packet);
            }
        }

        public void EnableSpell(int id)
        {
            _spellsEnabled[id] = true;

            if (Unit is Player)
            {
                Player p = Unit as Player;

                SpellUpdatePacket packet = new SpellUpdatePacket();
                packet.Index = id;
                packet.UpdateState = SpellUpdateState.Enable;

                p.Client.ConnectionHandler.SendPacket(packet);
            }
        }

    }
}
#endif
