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

        public string PowerAnim = "BowPower";
        public string AttackAnim = "BowShot";

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

            var e = unit.GetExt<UnitEquipment>();
            var u = e.OffHandUnit;

            e.DestroyItem(u.Item.Item.EQ.EquipType);
            
            u.Movement.Parent = null;
            u.Movement.Rotation = unit.Movement.Rotation - 90;
            u.Movement._UnSafeMoveTo(unit.Movement.Position);
            u.Movement.CanMove = true;

            Vector3 direction = unit.Movement.Forward;

            u.StartCoroutine(SEase.Action(() => u.Movement.Push(direction, 1f), 10));
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
