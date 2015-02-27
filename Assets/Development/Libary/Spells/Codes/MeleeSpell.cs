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
        [Range(0,2f)]
        public float CriticalAreaMultiplier = 1f;

        public float RadiusSize = 1f;

        public string PowerAnim = "LeftHandSlashPower";
        public string AttackAnim = "LeftHandSlashAttack";

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
                    if (!DoLocationTest(unit, u.Movement.Position, u.Display.Size / 2f))
                        continue;

                    com.Hit(DamageType, Spell.HitType.Melee, Spell.HitStrenght.Normal, unit.Combat, LowestDamage + (HighestDamage-LowestDamage) * strenght);
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

        public override void CancelCasting(ServerUnit Unit)
        {
            Unit.Anim.ActionAnimation = "CancelAction";
        }

        protected bool DoLocationTest(ServerUnit unit, Vector3 target, float targetRadius)
        {
            float distance = Vector3.Distance(unit.Movement.Position, target);
            if (distance < unit.Display.Size + RadiusSize + targetRadius)
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
            if (_radiusType == RadiusType.Line)
            {
                return true;
            }
            return false;
        }

        public static float LineHitDetection(Vector3 _dealerPos, float _width, float _length, float _angletoX, Vector3 _targetPos, float _targetRadius)
        {
            _targetPos.x -= _dealerPos.x;
            _targetPos.z -= _dealerPos.z;

            float r;
            float f;
            r = Vector3.Distance(Vector3.zero, _targetPos);
            f = (float)Math.Asin(_targetPos.z / r);
            r -= _targetRadius;
            f -= _angletoX;
            if (Math.Abs(r * Math.Sin(f)) <= _length / 2 && r * Math.Cos(f) <= _width)
            {
                return (float)(Math.Abs(r * Math.Cos(f)));
            }
            return (-1);
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
