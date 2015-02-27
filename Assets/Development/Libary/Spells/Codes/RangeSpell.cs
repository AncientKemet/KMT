#if SERVER
using Server.Model.Entities;
using Server.Model.Extensions.UnitExts;
#endif
#if UNITY_EDITOR
using UnityEditor;
#endif
using System;
using Code.Libaries.Generic.Managers;

using Shared.Content.Types;

using UnityEngine;

namespace Development.Libary.Spells.Codes
{
    public class RangeSpell : Spell
    {


        public MeleeSpell.RadiusType _radiusType;
        [Range(0, 2f)]
        public float CriticalAreaMultiplier = 1f;

        public float RadiusSize = 1f;

        public string PowerAnim = "OneHandThrowPower";
        public string AttackAnim = "OneHandThrowAttack";

        public float LowestDamage = 0;
        public float HighestDamage = 0;

        public Spell.DamageType DamageType = Spell.DamageType.Physical;

        public string GetDescription()
        {
            return "";
        }

#if SERVER

        public override void OnFinishCasting(ServerUnit unit, float strenght)
        {

            unit.Anim.ActionAnimation = AttackAnim + (strenght > 0.66 ? "Strong" : "") + (strenght < 0.33 ? "Weak" : "");

            if (strenght > 0.66f)
                unit.Attributes.AddBuff(ContentManager.I.OverpowerDebuff, 0.5f);


            
        }

        public override void OnStartCasting(ServerUnit unit)
        {
            unit.Anim.ActionAnimation = PowerAnim;
        }

        public override void OnStrenghtChanged(ServerUnit unit, float strenght)
        {

        }

        public override void CancelCasting(ServerUnit Unit)
        {
            Unit.Anim.ActionAnimation = "CancelAction";
        }


#endif
#if UNITY_EDITOR
        [MenuItem("Kemet/Create/Spell/Range")]
        public static void CreateTest()
        {
            CreateSpell<RangeSpell>();
        }
#endif

    }
}
