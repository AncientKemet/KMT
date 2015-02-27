using System;
using System.Collections.Generic;
using Client.Net;
using Code.Core.Client.UI.Controls;
using Libaries.Net.Packets.ForServer;
using UnityEngine;

namespace Client.UI.Interfaces.CreateCharacter
{
    public abstract class SelectionButton : Clickable
    {
        private static List<SelectionButton> SelectionButtons = new List<SelectionButton>(); 
        public tk2dSlicedSprite SelectSprite;

        private bool _isLocked = false;

        public bool IsLocked
        {
            get { return _isLocked; }
            set
            {
                _isLocked = value;
                if(collider != null)
                collider.enabled = !value;
            }
        }

        protected override void Start () {
	        base.Start();

            SelectionButtons.Add(this);

            OnLeftClick += OnSelected;
            OnLeftClick += delegate
            {
                var p = new CharacterChangePacket();
                p.Action = this.CharAction;
                p.value = this.Index;
                ClientCommunicator.Instance.SendToServer(p);
            };
            OnLeftClick += delegate
            {
                SelectionButtons.RemoveAll(button => button == null);
                foreach (var b in SelectionButtons)
                {
                    if (b.GetType() == GetType())
                    {
                        b.SelectSprite.gameObject.SetActive(false);
                    }
                }
                SelectSprite.gameObject.SetActive(true);
            };
        }

        protected abstract void OnSelected();

        public CharacterChangePacket.CharAction CharAction { get; set; }

        public int Index { get; set; }

    }
}
