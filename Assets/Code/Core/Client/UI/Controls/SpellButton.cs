using Client.Net;
using Client.UI.Interfaces;
using Code.Core.Client.Controls;
using Code.Core.Client.UI.Controls;
using Code.Core.Client.UI.Scripts;
using Code.Libaries.Generic.Managers;
using Code.Libaries.Net.Packets.ForServer;
using Shared.Content.Types;
using UnityEngine;

namespace Client.UI.Controls
{
    public class SpellButton : InterfaceButton
    {
        private static SpellButton currentHeldDownButton;
        public Spell spell
        {
            get { return _spell; }
            set
            {
                _spell = value;
                if (_spell == null)
                {
                    icon.Texture = null;
                    icon.gameObject.SetActive(false);
                    GetComponent<tk2dSlicedSprite>().color =
                        UIContentManager.I.SpellColors.Find(color => color.Type == SpellType.Other).Color;
                }
                else
                {
                    icon.gameObject.SetActive(true);
                    icon.Texture = _spell.Icon;
                    GetComponent<tk2dSlicedSprite>().color =
                        UIContentManager.I.SpellColors.Find(color => color.Type == spell.Type).Color;
                }
            }
        }

        public Icon icon;
        private Spell _spell;
        public KeyCode HotKey;

        protected override void Start()
        {
            base.Start();
            OnMouseIn += () =>
            {
                if (spell != null)
                {
                    var description = spell.Description;
                    DescriptionInterface.I.Show(spell.name, spell.Subtitle, description, spell.Icon);
                }
            };
            OnMouseOff += () => DescriptionInterface.I.Hide();
            OnLeftDown += SendButtonDown;
            OnLeftClick += () =>
            {
                currentHeldDownButton = null;
                spell = spell;
            };
            OnLeftClick += () => SendClickPacket("");
        }

        protected virtual void Update()
        {
            if(currentHeldDownButton == null)
            if (Input.GetKeyDown(HotKey) && KeyboardInput.Instance.FullListener == null)
            {
                if (OnLeftDown != null)
                {
                    OnLeftDown();
                }
                else if (OnLeftClick != null) OnLeftClick();
            }
            if(currentHeldDownButton == this)
            if (Input.GetKeyUp(HotKey) && KeyboardInput.Instance.FullListener == null)
            {

                if (OnLeftClick != null) OnLeftClick();
            }
        }

        private void SendButtonDown()
        {
            currentHeldDownButton = this;

            GetComponent<tk2dSlicedSprite>().color = Color.white;;

            UIInterfaceEvent p = new UIInterfaceEvent();

            p.interfaceId = InterfaceId;
            p.controlID = Index;

            p._eventType = UIInterfaceEvent.EventType.Button_Down;

            ClientCommunicator.Instance.SendToServer(p);
        }

    }
}
