using Client.Units;
using Client.Units.SpellRadiuses;
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
    public class MeleeSpell : Spell
    {

        public enum RadiusType
        {
            _180Degrees,
            _90Degrees,
            _360Degrees,
            Line
        }

        public RadiusType _radiusType;
        public float LineWidth = 0.5f;

        public string PowerAnim = "LeftHandSlashPower";
        public string AttackAnim = "LeftHandSlashAttack";

        public float BaseDamage = 10;

        public DamageType DamageType = DamageType.Physical;

#if CLIENT
        private ASpellRadius CurrentRadius;
        public void OnEnable()
        {
            ClientOnStartedCasting += unit =>
            {
                switch (_radiusType)
                {
                    case RadiusType.Line:

                        CurrentRadius = Instantiate(ContentManager.I.SpellRadiuses[0].gameObject).GetComponent<ASpellRadius>();

                        CurrentRadius.CriticalArea = unit.PlayerUnitAttributes.GetAttribute(UnitAttributeProperty.CriticalArea);
                        CurrentRadius.Range = unit.PlayerUnitAttributes.GetAttribute(UnitAttributeProperty.WeaponReach);
                        (CurrentRadius as MeleeLineRadius).Width = LineWidth;

                        CurrentRadius.transform.transform.parent = unit.transform;
                        CurrentRadius.transform.localPosition = Vector3.up;
                        CurrentRadius.transform.localRotation = Quaternion.identity;

                        break;
                }
            };

            ClientOnStrenghtChanged += (unit, f) =>
            {
                if (CurrentRadius != null)
                    CurrentRadius.Strenght = f;
            };

            ClientOnFinishedCasting += unit =>
            {
                if (CurrentRadius != null)
                    Destroy(CurrentRadius.gameObject);
            };
        }
#endif
#if SERVER

        public override void OnFinishCasting(ServerUnit unit, float strenght)
        {
            var hitStrenght = strenght < 0.33f
                ? HitStrenght.Weak
                : strenght > 0.66f ? HitStrenght.Strong : HitStrenght.Normal;
            unit.Anim.ActionAnimation = AttackAnim + (strenght > 0.66 ? "Strong" : "") + (strenght < 0.33 ? "Weak" : "");

            if (strenght > 0.66f)
                unit.Attributes.AddBuff(ContentManager.I.OverpowerDebuff, 0.5f);

            if (_radiusType != RadiusType.Line)
                foreach (var o in unit.CurrentBranch.ActiveObjectsVisible)
                {
                    ServerUnit u = o as ServerUnit;

                    if (u == null)
                        continue;

                    if (u == unit)
                        continue;

                    UnitCombat com = u.Combat;

                    if (com != null)
                    {
                        if (!DoNormalHitBoxTestAngle(unit, u.Movement.Position, u.Display.Size / 2f))
                            continue;

                        com.Hit(DamageType, HitType.Melee, hitStrenght, unit.Combat, BaseDamage * strenght);
                    }
                }
            else // for line radius we use unity physics
            {
                Vector3 origin = unit.Movement.Position;
                Vector3 direction = unit.Movement.Forward;
                int layer = 1 << 30;
                foreach (var hit in Physics.SphereCastAll(origin + direction* LineWidth/2f, LineWidth, direction, unit.Attributes.Get(UnitAttributeProperty.WeaponReach) - LineWidth/2f, layer))
                {
                    var combat = hit.collider.GetComponent<UnitCombat>();
                    if (combat != null && combat != unit.Combat)
                    {
                        combat.Hit(DamageType, HitType.Melee, hitStrenght, unit.Combat, BaseDamage * strenght);
                    }
                }
            }
        }

        public override void OnStartCasting(ServerUnit unit)
        {
            unit.Anim.ActionAnimation = PowerAnim;
        }

        public override void OnStrenghtChanged(ServerUnit unit, float strenght)
        {

        }

        public override void CancelCasting(ServerUnit unit)
        {
            unit.Anim.ActionAnimation = "CancelAction";
        }

        private bool DoNormalHitBoxTestAngle(ServerUnit unit, Vector3 target, float targetRadius)
        {
            float distance = Vector3.Distance(unit.Movement.Position, target);
            float weaponReach = unit.Attributes.Get(UnitAttributeProperty.WeaponReach);
            //Angle stuff
            if (distance < unit.Display.Size + weaponReach + targetRadius)
            {
                Vector3 referenceForward = unit.Movement.Forward;
                Vector3 referenceRight = Vector3.Cross(Vector3.up, referenceForward);
                Vector3 newDirection = target - unit.Movement.Position;
                float angle = Vector3.Angle(newDirection, referenceForward);
                float sign = Mathf.Sign(Vector3.Dot(newDirection, referenceRight));
                float finalAngle = sign * angle;

                if (_radiusType == RadiusType._90Degrees)
                {
                    float f = (float)(targetRadius / (Math.PI * distance * 180));
                    if (angle > Mathf.Min(0, -45 + f) && angle < Mathf.Max(0, 45 - f))
                        return true;
                }
                else if (_radiusType == RadiusType._180Degrees)
                {
                    if (angle > -90 && angle < 90)
                        return true;
                }
                else if (_radiusType == RadiusType._360Degrees)
                {
                    return true;
                }
            }
            return false;
        }


#endif
#if UNITY_EDITOR
        [MenuItem("Kemet/Create/Spell/Melee")]
        public static void CreateTest()
        {
            CreateSpell<MeleeSpell>();
        }
#endif

    }
}
