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

        
    }
}
