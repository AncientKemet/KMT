#if UNITY_EDITOR
#endif
using System;
using System.Collections.Generic;
using Client.Net;
using Client.Units;
using Code.Core.Client.Settings;
using Code.Core.Client.UI.Controls;
using Code.Core.Shared.Content.Types.ItemExtensions;
using Code.Libaries.Net.Packets.ForServer;
using UnityEngine;

namespace Shared.Content.Types.ItemExtensions
{
    [Serializable]
    [RequireComponent(typeof(Collider))]
    [RequireComponent(typeof(Rigidbody))]
    public class ItemRigid : ItemExtension
    {
        [SerializeField]
        private List<RightClickAction> Actions = new List<RightClickAction>();

        [SerializeField] private bool KeepColliding = false;

        private bool _physicsEnabled;

        /// <summary>
        /// Use only in player.
        /// </summary>
        public bool PhysicsEnabled
        {
            get
            {
                return _physicsEnabled;
            }
            set
            {
                _physicsEnabled = value;

                //Only for medium and higher
                if (VideoSettings.Instance.Physics.Value >= VideoSettings.PhysicsQuality.Low)
                {
                    GetComponent<Rigidbody>().isKinematic = !value;
                }

                if (value)
                {
                    //Only for medium and higher
                    /*if (VideoSettings.Instance.Physics.Value >= VideoSettings.PhysicsQuality.Low)
                    {
                        if (!KeepColliding)
                        {
                            //automatic disable
                            StartCoroutine(Disable(0.3f));
                        }
                    }*/

                }
            }
        }

        private void AddActions(Clickable parent)
        {
            EquipmentItem equipmentItem = GetComponent<EquipmentItem>();
            ItemWithInventory itemWithInventory = GetComponent<ItemWithInventory>();

            Rigidbody parentRigidbody = parent.gameObject.GetComponent<Rigidbody>();
            if (parentRigidbody == null)
                parentRigidbody = parent.gameObject.AddComponent<Rigidbody>();
            if (parentRigidbody != null)
            {
                parentRigidbody.isKinematic = true;

                SpringJoint spring = gameObject.AddComponent<SpringJoint>();
                spring.connectedBody = parentRigidbody;
                spring.maxDistance = 1f;
            }

            if (equipmentItem != null)
            {
                if (equipmentItem.CanBeStoredInInventory)
                    parent.AddAction(new RightClickAction(
                        "Take",
                        delegate
                        {
                            if (parent == null)
                                return;

                            PlayerUnit unit = parent.GetComponent<PlayerUnit>();
                            if (unit != null)
                            {
                                UnitActionPacket p = new UnitActionPacket();
                                p.UnitId = unit.Id;
                                p.ActionName = "Take";
                                ClientCommunicator.Instance.SendToServer(p);
                            }
                        }
                        ));
                else
                    parent.AddAction(new RightClickAction(
                        "Pick-up",
                        delegate
                        {
                            if (parent == null)
                                return;

                            PlayerUnit unit = parent.GetComponent<PlayerUnit>();
                            if (unit != null)
                            {
                                UnitActionPacket p = new UnitActionPacket();
                                p.UnitId = unit.Id;
                                p.ActionName = "Pick-up";
                                ClientCommunicator.Instance.SendToServer(p);
                            }
                        }
                        ));
            }
            else
            {
                parent.AddAction(new RightClickAction(
                        "Take",
                        delegate
                        {
                            if (parent == null)
                                return;

                            PlayerUnit unit = parent.GetComponent<PlayerUnit>();
                            if (unit != null)
                            {
                                UnitActionPacket p = new UnitActionPacket();
                                p.UnitId = unit.Id;
                                p.ActionName = "Take";
                                ClientCommunicator.Instance.SendToServer(p);
                            }
                        }
                        ));
            }

            if (itemWithInventory != null)
            {
                parent.AddAction(new RightClickAction(
                    "Open",
                    delegate
                    {
                        if (parent == null)
                            return;

                        PlayerUnit unit = parent.GetComponent<PlayerUnit>();
                        if (unit != null)
                        {
                            UnitActionPacket p = new UnitActionPacket();
                            p.UnitId = unit.Id;
                            p.ActionName = "Open";
                            ClientCommunicator.Instance.SendToServer(p);
                        }
                    }
                ));
            }

            foreach (var action in Actions)
            {
                action.Action = delegate
                {
                    if (parent == null)
                        return;

                    PlayerUnit unit = parent.GetComponent<PlayerUnit>();
                    if (unit != null)
                    {
                        UnitActionPacket p = new UnitActionPacket();
                        p.UnitId = unit.Id;
                        p.ActionName = action.Name;
                        ClientCommunicator.Instance.SendToServer(p);
                    }
                };
            }
        }

        public void ForwardClicksToParent()
        {
            if (transform.parent != null)
            {
                PlayerUnit parent = transform.parent.GetComponent<PlayerUnit>();
                if (parent != null)
                {
                    Clickable c = GetComponent<Clickable>();
                    if (c == null)
                        c = gameObject.AddComponent<Clickable>();

                    parent.RegisterChildClickable(c);

                    AddActions(parent);
                }
                else
                {
                    Debug.LogError("Couldn't find parent clickable.");
                }
            }
            else
            {
                Debug.LogError("Couldn't find parent.");
            }
        }
        
    }
}
