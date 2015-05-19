using System.Collections.Generic;
using Code.Code.Libaries.Net;
using Libaries.Net.Packets.ForClient;
using Server.Model.Entities;
using Server.Model.Entities.Animals;
using Server.Model.Entities.Human;
using Server.Model.Entities.Items;
using UnityEngine;

namespace Server.Model.Extensions.UnitExts
{
    public class UnitDetails : UnitUpdateExt
    {
        private ServerUnit _unit;

        private List<string> _actions = new List<string>();

        /// <summary>
        /// Adds an unit action, forces an _wasUpdate.
        /// </summary>
        /// <param name="_action"></param>
        public void AddAction(string _action)
        {
            _actions.Add(_action);
            _wasUpdate = true;
        }

        /// <summary>
        /// Removes an unit action, forces an _wasUpdate.
        /// </summary>
        /// <param name="_action"></param>
        public void RemoveAction(string _action)
        {
            _actions.Remove(_action);
            _wasUpdate = true;
        } 

        public override byte UpdateFlag()
        {
            return 0x20;
        }

        protected override void pSerializeState(ByteStream packet)
        {
            packet.AddString(_unit.name);
            packet.AddByte(_actions.Count);
            foreach (var action in _actions)
            {
                packet.AddString(action);
            }
        }

        protected override void pSerializeUpdate(ByteStream packet)
        {
            pSerializeState(packet);
        }

        protected override void OnExtensionWasAdded()
        {
            base.OnExtensionWasAdded();
            _unit = entity as ServerUnit;
        }

        /// <summary>
        /// Forces this unit to perform an Action on other unit.
        /// If the other unit is more than 32m away, the action is canceled.
        /// </summary>
        /// <param name="unitId">Other unit ID</param>
        /// <param name="actionName">Action name</param>
        public void DoAction(int unitId, string actionName)
        {
            ServerUnit selectedUnit = _unit.CurrentWorld[unitId];
            if (selectedUnit == null)
                return;
            
            float distance = Vector3.Distance(_unit.Movement.Position, selectedUnit.Movement.Position);
            if (distance > _unit.Display.Size + selectedUnit.Display.Size)
            {
                if (distance < 32f)
                    _unit.Movement.WalkTo(selectedUnit.Movement.Position, _unit.Details.DoAction, unitId, actionName);
            }
            else
            {
                selectedUnit.Details.HandleIncommingAction(_unit, actionName);
            }
        }

        /// <summary>
        /// Handles incomming action.
        /// </summary>
        /// <param name="fromUnit">Unit which it comes from</param>
        /// <param name="actionName">Action Name</param>
        protected void HandleIncommingAction(ServerUnit fromUnit, string actionName)
        {
            #region DroppedItem
            if (_unit is DroppedItem)
            {
                bool unitHasAccess = false;
                var item = _unit as DroppedItem;

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
            #endregion

            #region Open
            else if (actionName == "Open")
            {
                UnitAccessOwnership acc = _unit.Access;
                UnitInventory inventory = _unit.GetExt<UnitInventory>();

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
            #endregion

            #region Inspect
            else if (actionName == "Inspect")
            {
                if (fromUnit is Player)
                {
                    Player p = fromUnit as Player;
                    p.ClientUi.ProfileInterface.Open(_unit, ProfileInterfaceUpdatePacket.PacketTab.Main);
                    p.ClientUi.ProfileInterface.Opened = true;
                }
                return;
            }
            #endregion

            #region Animal-Eating
            else if (actionName == "Animal-Eating")
            {
                Animal a = fromUnit as Animal;
                a.Hunger -= 20f;
                _unit.Display.Size -= a.Display.Size / 10f;
                if (_unit.Display.Size < 0.01f)
                {
                    _unit.Display.Destroy = true;
                }
                return;
            }
            #endregion

            #region Vendor

            if (entity is NPC)
            {
                if (actionName == "Trade")
                {
                    Debug.LogError("Unhandled action: " + actionName + " TODO");
                    return;
                }
            }

            #endregion

            Debug.LogError("Unhandled action: " + actionName);
        }
    }
}
