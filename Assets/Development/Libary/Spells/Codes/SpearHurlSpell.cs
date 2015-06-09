#if SERVER
using Server.Model.Entities;
using Server.Model.Extensions.UnitExts;
#endif
#if UNITY_EDITOR
using UnityEditor;
#endif
using Code.Libaries.Generic.Managers;

using Shared.Content.Types;

using UnityEngine;

namespace Development.Libary.Spells.Codes
{
    public class SpearHurlSpell : Spell
    {
        public MeleeSpell.RadiusType _radiusType;

        public string PowerAnim = "OneHandThrowPower";
        public string AttackAnim = "OneHandThrowAttack";

#if SERVER

        public override void OnFinishCasting(ServerUnit unit, float strenght)
        {
            unit.Anim.ActionAnimation = AttackAnim + (strenght > 0.66 ? "Strong" : "") + (strenght < 0.33 ? "Weak" : "");

            if (strenght > 0.66f)
                unit.Attributes.AddBuff(ContentManager.I.OverpowerDebuff, 0.5f);

            var e = unit.GetExt<UnitEquipment>();
            var u = e.MainHandUnit;
            
            u.Movement.Parent = null;
            u.Movement.CanMove = true;
            u.Movement.CanRotate = true;
            u.Movement.Rotation = unit.Movement.Rotation;
            u.Movement._UnSafeMoveTo(unit.Movement.Position);
            

            Vector3 direction = unit.Movement.Forward;

            u.StartCoroutine(SEase.Action(() => u.Movement.Push(direction, 1f + strenght / 2f), 10));
            u.StartCoroutine(SEase.Action(() =>
            {
                if (u != null)
                {
                    if (unit == null)
                        u.Display.Destroy = true;

                    u.Movement.Teleport(unit.Movement.Position);
                    u.Movement.Parent = unit.Movement;
                    u.Movement.Rotation = 0;
                }
            },-1,2f));
        }

        public override void OnStartCasting(ServerUnit unit)
        {
            var e = unit.GetExt<UnitEquipment>();
            var u = e.MainHandUnit;

            unit.Anim.ActionAnimation = PowerAnim;
            u.Movement.CanMove = true;
            u.Movement.CanRotate = true;
            u.Movement.Rotation = 180;
        }

        public override void OnStrenghtChanged(ServerUnit unit, float strenght)
        {

        }

        public override void CancelCasting(ServerUnit unit)
        {
            var e = unit.GetExt<UnitEquipment>();
            var u = e.MainHandUnit;

            unit.Anim.ActionAnimation = "CancelAction";
            u.Movement.Rotation = 0;
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
