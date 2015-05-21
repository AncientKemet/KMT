using Client.UI.Controls;
using Client.UI.Scripts;
using Code.Core.Client.UI.Controls;
using Code.Libaries.UnityExtensions.Independent;
using Libaries.Net.Packets.ForClient;
using Libaries.UnityExtensions.Independent;
using UnityEngine;

namespace Code.Core.Client.UI.Interfaces
{
    public class ActionBars : UIInterface<ActionBars>
    {

        private Vector3 HiddenPos = Vector3.zero + Vector3.down*20f;

        [SerializeField]
        private ChannelBar ChannelBar;

        public SpellButton Q,W,E,R;

        protected override void OnStart()
        {
            base.OnStart();
            transform.localPosition = HiddenPos;
        }

        public override void Hide()
        {
            CorotineManager.Instance.StartCoroutine(
                Ease.Vector(
                    Vector3.zero,
                    HiddenPos,
                    delegate(Vector3 vector3)
                    {
                        transform.localPosition = vector3;
                    },
                    delegate
                    {
                        gameObject.SetActive(false);
                    },
                    1f
                    )
                );

        }

        public override void Show()
        {
            gameObject.SetActive(true);
            CorotineManager.Instance.StartCoroutine(
                Ease.Vector(
                    HiddenPos,
                    Vector3.zero,
                    delegate(Vector3 vector3)
                    {
                        transform.localPosition = vector3;
                    },
                    delegate
                    {
                    },
                    1f
                    )
                );
        }

        public void OnPacket(SpellUpdatePacket p)
        {
            //Cast spell packets
            if (p.IsCasting)
            {
                ChannelBar.Progress = p.Strenght;
            }
            //Set spell packets
            if (p.SetSpell)
            {
                if (p.Index == 0)
                {
                    Q.spell = p.Spell;
                }
                else if (p.Index == 1)
                {
                    W.spell = p.Spell;
                }
                else if (p.Index == 2)
                {
                    E.spell = p.Spell;
                }
                else if (p.Index == 3)
                {
                    R.spell = p.Spell;
                }
            }
        }
    }
}
