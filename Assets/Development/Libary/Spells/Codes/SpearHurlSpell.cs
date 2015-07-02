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

        public float ProjectileSpeed = 5f;
        public float FallOffSpeed = 5f;
        public float ProcessesPerSecond = 10;

#if SERVER

        public override void OnFinishCasting(ServerUnit unit, float strenght)
        {
            unit.Anim.ActionAnimation = AttackAnim + (strenght > 0.66 ? "Strong" : "") + (strenght < 0.33 ? "Weak" : "");

            if (strenght > 0.66f)
                unit.Attributes.AddBuff(ContentManager.I.OverpowerDebuff, 0.5f);

            var e = unit.GetExt<UnitEquipment>();
            var u = e.MainHandUnit;
            
            u.Movement.Parent = null;
            u.Movement._UnSafeMoveTo(unit.Movement.Position + Vector3.up * 3);
           
            Vector3 direction = unit.Movement.Forward + Vector3.up;
            float rot = unit.Movement.Rotation + 90;
            float _time = Time.time;
            u.StartCoroutine(SEase.Action(() =>
            {
                _time = Time.time - _time;
                float force = (0.5f + strenght / 1f) * ProjectileSpeed;
                if (direction != Vector3.zero)
                {
                    //Raycast next hit
                    RaycastHit hit;
                    Physics.Raycast(u.Movement.Position, direction, out hit, force * 2, 1 << 8);
                    if (hit.collider != null)
                    {
                        Debug.Log("Hit "+hit.collider.gameObject);
                        u.Movement._UnSafeMoveTo(hit.point + new Vector3(0, 0.3f, 0));
                        direction = Vector3.zero;
                        
                    }
                    else
                    {
                        u.Movement.Rotation =rot;
                        u.Movement._UnSafeMoveTo(u.Movement.Position + direction * force);
                        direction -= (Vector3.up / FallOffSpeed);
                    }
                }
            }
            , (int) (3 * ProcessesPerSecond), 1 / ProcessesPerSecond));
            u.StartCoroutine(SEase.Action(() =>
            {
                if (u != null)
                {
                    if (unit == null)
                    {
                        u.Display.Destroy = true;
                        return;
                    }

                    u.Movement.Teleport(unit.Movement.Position);
                    u.Movement.Parent = unit.Movement;
                    u.Movement.Rotation = 0;
                }
            },-1,5f));
        }

        public override void OnStartCasting(ServerUnit unit)
        {
            var e = unit.GetExt<UnitEquipment>();
            var u = e.MainHandUnit;

            unit.Anim.ActionAnimation = PowerAnim;
            u.Movement.DiscardPath();
            u.Movement.Rotation = 180;

        }

        public override void OnStrenghtChanged(ServerUnit unit, float strenght)
        {
            var e = unit.GetExt<UnitEquipment>();
            var u = e.MainHandUnit;
            u.Movement.DiscardPath();
            u.Movement.Rotation = 180;
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
