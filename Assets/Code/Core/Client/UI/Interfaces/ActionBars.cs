﻿using Client.UI.Controls;
using Client.UI.Scripts;
using Client.Units;
using Code.Core.Client.UI.Controls;
using Code.Libaries.UnityExtensions.Independent;
using JetBrains.Annotations;
using Libaries.Net.Packets.ForClient;
using Libaries.UnityExtensions.Independent;
using Shared.Content.Types;
using UnityEngine;

namespace Code.Core.Client.UI.Interfaces
{
    public class ActionBars : UIInterface<ActionBars>
    {

        private Vector3 HiddenPos = Vector3.zero + Vector3.down * 20f;

        [SerializeField]
        private ChannelBar ChannelBar;

        public SpellButton Q, W, E, R;

        [CanBeNull]
        private Spell currentCastingSpell;

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
            if (p.UpdateState == SpellUpdateState.Disable)
            {
                if (p.Index == 0)
                {
                    Q.icon.gameObject.SetActive(false);
                }
                else if (p.Index == 1)
                {
                    W.icon.gameObject.SetActive(false);
                }
                else if (p.Index == 2)
                {
                    E.icon.gameObject.SetActive(false);
                }
                else if (p.Index == 3)
                {
                    R.icon.gameObject.SetActive(false);
                }
            }
            if (p.UpdateState == SpellUpdateState.Enable)
            {
                if (p.Index == 0)
                {
                    Q.icon.gameObject.SetActive(true);
                }
                else if (p.Index == 1)
                {
                    W.icon.gameObject.SetActive(true);
                }
                else if (p.Index == 2)
                {
                    E.icon.gameObject.SetActive(true);
                }
                else if (p.Index == 3)
                {
                    R.icon.gameObject.SetActive(true);
                }
            }

            //Cast spell packets
            if (p.UpdateState == SpellUpdateState.StrenghtChange)
            {
                ChannelBar.Progress = p.Strenght;
                if (currentCastingSpell != null)
                    if (currentCastingSpell.ClientOnStrenghtChanged != null)
                        currentCastingSpell.ClientOnStrenghtChanged(PlayerUnit.MyPlayerUnit, p.Strenght);

            }
            //Set spell packets
            if (p.UpdateState == SpellUpdateState.SetSpell)
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
            else if (p.UpdateState == SpellUpdateState.StartedCasting)
            {
                if (p.Index == 0)
                {
                    if (currentCastingSpell != Q.spell)
                    {
                        currentCastingSpell = Q.spell;
                        Q.ServerStartedCasting();
                        if (Q.spell.ClientOnStartedCasting != null)
                            Q.spell.ClientOnStartedCasting(PlayerUnit.MyPlayerUnit);
                    }
                }
                else if (p.Index == 1)
                {
                    if (currentCastingSpell != W.spell)
                    {
                        currentCastingSpell = W.spell;
                        W.ServerStartedCasting();
                        if (W.spell.ClientOnStartedCasting != null)
                            W.spell.ClientOnStartedCasting(PlayerUnit.MyPlayerUnit);
                    }
                }
                else if (p.Index == 2)
                {
                    if (currentCastingSpell != E.spell)
                    {
                        E.ServerStartedCasting();
                        currentCastingSpell = E.spell;
                        if (E.spell.ClientOnStartedCasting != null)
                            E.spell.ClientOnStartedCasting(PlayerUnit.MyPlayerUnit);
                    }
                }
                else if (p.Index == 3)
                {
                    if (currentCastingSpell != R.spell)
                    {
                        R.ServerStartedCasting();
                        currentCastingSpell = R.spell;
                        if (R.spell.ClientOnStartedCasting != null)
                            R.spell.ClientOnStartedCasting(PlayerUnit.MyPlayerUnit);
                    }
                }
            }
            else if (p.UpdateState == SpellUpdateState.FinishedCasting)
            {
                
                if (p.Index == 0)
                {
                    Q.ServerFinishedCasting();
                    if (Q.spell.ClientOnFinishedCasting != null)
                        Q.spell.ClientOnFinishedCasting(PlayerUnit.MyPlayerUnit);
                }
                else if (p.Index == 1)
                {
                    W.ServerFinishedCasting();
                    if (W.spell.ClientOnFinishedCasting != null)
                        W.spell.ClientOnFinishedCasting(PlayerUnit.MyPlayerUnit);
                }
                else if (p.Index == 2)
                {
                    E.ServerFinishedCasting();
                    if (E.spell.ClientOnFinishedCasting != null)
                        E.spell.ClientOnFinishedCasting(PlayerUnit.MyPlayerUnit);
                }
                else if (p.Index == 3)
                {
                    R.ServerFinishedCasting();
                    if (R.spell.ClientOnFinishedCasting != null)
                        R.spell.ClientOnFinishedCasting(PlayerUnit.MyPlayerUnit);
                }
                currentCastingSpell = null;
            }
        }
    }
}
