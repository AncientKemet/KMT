using Client.Enviroment;
using Client.Net;
using Client.UI.Interfaces;
using Code.Core.Client.Controls;
using Code.Core.Client.UI.Controls;
using Code.Core.Client.UI.Scripts;
using Code.Libaries.Generic.Managers;
using Code.Libaries.Net.Packets.ForServer;
using Libaries.Net.Packets.ForServer;
using Shared.Content.Types;
using UnityEngine;

namespace Client.UI.Controls
{
    public class SpellButton : InterfaceButton
    {
        private tk2dSlicedSprite Sprite
        {
            get { return _sprite ?? (_sprite = GetComponent<tk2dSlicedSprite>()); }
        }

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
                    Sprite.color =
                        UIContentManager.I.SpellColors.Find(color => color.Type == SpellType.Other).Color;
                }
                else
                {
                    icon.gameObject.SetActive(true);
                    icon.Texture = _spell.Icon;
                    Sprite.color =
                        UIContentManager.I.SpellColors.Find(color => color.Type == spell.Type).Color;
                }
            }
        }

        public Icon icon;
        private Spell _spell;
        public KeyCode HotKey;
        private tk2dSlicedSprite _sprite;

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
            OnLeftDown += SendStartCasting;
            OnLeftUp += SendFinishCasting;
        }

        protected virtual void Update()
        {
            if (Input.GetKeyDown(HotKey) && KeyboardInput.Instance.FullListener == null)
            {
                if (OnLeftDown != null)
                {
                    OnLeftDown();
                }
                else if (OnLeftClick != null) OnLeftClick();
            }
            if (Sprite.color == Color.white)
            {
                if (Input.GetKeyUp(HotKey) && KeyboardInput.Instance.FullListener == null)
                {
                    if (OnLeftUp != null) OnLeftUp();
                }
                else
                {
                    if (!Input.GetKey(HotKey))
                    {
                        if (OnLeftUp != null) OnLeftUp();
                        Sprite.color = Color.red;
                    }
                }
            }
        }

        private void SendStartCasting()
        {
            SpellCastPacket p = new SpellCastPacket();

            p.buttonIndex = Index;
            p.Action = SpellCastPacket.CastAction.StartCasting;
            p.TargetPosition = KemetMap.Instance.MouseAt.point;

            ClientCommunicator.Instance.SendToServer(p);
        }

        private void SendFinishCasting()
        {
            SpellCastPacket p = new SpellCastPacket();

            p.buttonIndex = Index;
            p.Action = SpellCastPacket.CastAction.FinishCasting;
            p.TargetPosition = KemetMap.Instance.MouseAt.point;

            ClientCommunicator.Instance.SendToServer(p);
        }

        public void ServerStartedCasting()
        {
            Sprite.color = Color.white;
        }

        public void ServerFinishedCasting()
        {
            Sprite.color =
                        UIContentManager.I.SpellColors.Find(color => color.Type == spell.Type).Color;
        }
    }
}
