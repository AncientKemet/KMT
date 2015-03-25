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
    public class UnitActions : EntityExtension
    {
        public ServerUnit Unit { get; private set; }

        /// <summary>
        /// List of spells. The first four will be placed into players action bars.
        /// </summary>
        private readonly List<Spell> _spells = new List<Spell>(8);

        private int _currentCastingSpellId = -1;
        private float _currentSpellTime;

        public override void Progress()
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

        public override void Serialize(ByteStream bytestream)
        { }

        public override void Deserialize(ByteStream bytestream)
        { }

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

        public void DoAction(int unitId, string actionName)
        {

            ServerUnit selectedUnit = Unit.CurrentWorld[unitId];

            if (selectedUnit == null)
            {
                //Debug.LogError("Trying to execute action ["+actionName+"] on a null unit.");
                return;
            }

            float distance = Vector3.Distance(Unit.Movement.Position, selectedUnit.Movement.Position);
            if (distance > Unit.Display.Size + selectedUnit.Display.Size)
            {
                if (distance < 32f)
                    Unit.Movement.WalkTo(selectedUnit.Movement.Position, Unit.Actions.DoAction, unitId, actionName);
            }
            else
            {
                selectedUnit.Actions.HandleIncommingAction(Unit, actionName);
            }
        }

        protected void HandleIncommingAction(ServerUnit fromUnit, string actionName)
        {
            if (Unit is DroppedItem)
            {
                bool unitHasAccess = false;
                var item = Unit as DroppedItem;

                if (item.AccessType == DroppedItem.DroppedItemAccessType.ALL)
                    unitHasAccess = true;

                if (item.AccessType == DroppedItem.DroppedItemAccessType.List)
                    unitHasAccess = item.AccessList.Contains(fromUnit);

                if (unitHasAccess)
                {
                    if (actionName == "Take")
                    {
                        UnitInventory inventory = fromUnit.GetExt<UnitInventory>();
                        if (inventory != null)
                        {
                            if (inventory.AddItem(item.Item))
                            {
                                item.Display.PickupingUnit = fromUnit;
                            }
                        }
                        return;
                    }
                    if (actionName == "Pick-up")
                    {
                        UnitEquipment eq = fromUnit.GetExt<UnitEquipment>();
                        if (eq != null)
                        {
                            if (eq.EquipItem(item))
                            {
                            }
                        }
                        return;
                    }
                }
                else
                {
                    return;
                }
            }

            if (actionName == "Open")
            {
                UnitAccessOwnership acc = Unit.Access;
                UnitInventory inventory = Unit.GetExt<UnitInventory>();

                if (inventory == null)
                    Debug.LogError("Not an inventory.");

                if (acc == null || acc.GetAccessFor(fromUnit).View_Inventory)
                {
                    if (fromUnit is Player)
                    {
                        Player p = fromUnit as Player;
                        p.ClientUi.Inventories.ShowInventory(inventory);
                    }
                }
                else
                {
                    Debug.Log("i have no access.");
                }
                return;
            }

            if (actionName == "Inspect")
            {
                if (fromUnit is Player)
                {
                    Player p = fromUnit as Player;
                    p.ClientUi.ProfileInterface.Open(Unit, ProfileInterfaceUpdatePacket.PacketTab.Main);
                    p.ClientUi.ProfileInterface.Opened = true;
                }
                return;
            }

            if (actionName == "Animal-Eating")
            {
                Animal a = fromUnit as Animal;
                a.Hunger -= 20f;
                Unit.Display.Size -= a.Display.Size / 10f;
                if (Unit.Display.Size < 0.01f)
                {
                    Unit.Display.Destroy = true;
                }
                Unit.Progress();
                return;
            }

            Debug.LogError("Unhandled action: " + actionName);
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
