using System.Collections.Generic;
using Code.Core.Client.UI.Interfaces;
using Code.Core.Client.UI.Scripts;
using Code.Core.Shared.Content.Types;
using Code.Libaries.Generic.Managers;
using Shared.Content.Types;
using UnityEngine;

namespace Code.Core.Client.UI.Controls
{
    [RequireComponent(typeof(Icon))]
    public class SpellEffect : UIControl
    {
        private Spell _spell;

        private Icon _icon;
        
        public Spell Spell
        {
            get { return _spell; }
            private set
            {
                _spell = value;

                renderer.enabled = value != null;

                if (value != null)
                {
                    if (_icon == null)
                    {
                        _icon = GetComponent<Icon>();
                    }

                    _icon.Texture = value.Icon;
                }
            }
        }

        protected override void Start()
        {
            base.Start();

            if(_icon == null)
            _icon = GetComponent<Icon>();

            OnMouseIn += () => { if (Spell != null) SpellDescription.Show(Spell, transform.position); };
            OnMouseOff += () => SpellDescription.I.Hide();
        }

        public override void OnSetData(List<float> data)
        {
            if ((int) data[0] == -1)
            {
                Spell = null;
                return;
            }
            Spell = ContentManager.I.Spells[(int)data[0]];
        }
    }
}
