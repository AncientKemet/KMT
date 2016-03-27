using System.Collections.Generic;
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

        public float BaseDamage = 2;

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
                    case RadiusType._90Degrees:

                        CurrentRadius = Instantiate(ContentManager.I.SpellRadiuses[1].gameObject).GetComponent<ASpellRadius>();

                        CurrentRadius.CriticalArea = unit.PlayerUnitAttributes.GetAttribute(UnitAttributeProperty.CriticalArea);
                        CurrentRadius.Range = unit.PlayerUnitAttributes.GetAttribute(UnitAttributeProperty.WeaponReach);

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

        public override string Description
        {
            get
            {
                PlayerUnitAttributes a = PlayerUnit.MyPlayerUnit.PlayerUnitAttributes;
                return base.Description
                    .Replace("$BaseDamage", "[" + (BaseDamage 
                    * (1+a.GetAttribute(UnitAttributeProperty.PhysicalDamage))
                    )+"]")
                    .Replace("$CriticalDamage", "[" + (BaseDamage 
                    * (1+a.GetAttribute(UnitAttributeProperty.PhysicalDamage)) 
                    * a.GetAttribute(UnitAttributeProperty.CriticalDamage)) 
                    + "]");
            }
        }

#if SERVER

        public override void OnFinishCasting(ServerUnit unit, float strenght)
        {
            var hitStrenght = strenght < 0.33f
                    ? HitStrenght.Weak
                    : strenght > 0.66f ? HitStrenght.Strong : HitStrenght.Normal;
                Vector3 way = unit.Spells.TargetPosition - unit.Movement.Position;
                way = way.normalized;
                unit.Anim.ActionAnimation = AttackAnim + (strenght > 0.66 ? "Strong" : "");
            unit.Movement.NextMovements.AddLast((f) =>
            {
                
                unit.Movement.DiscardPath();
                unit.Movement.RotateWay(way);
                unit.Movement.Push(way, strenght, () =>
                {
                    if (strenght > 0.66f)
                        unit.Attributes.AddBuff(ContentManager.I.OverpowerDebuff, 1f);

                    if (_radiusType != RadiusType.Line)
                    {
                        float reach = unit.Attributes.Get(UnitAttributeProperty.WeaponReach);
                        float critArea = unit.Attributes.Get(UnitAttributeProperty.CriticalArea);
                        float critDmg = unit.Attributes.Get(UnitAttributeProperty.CriticalDamage);
                        Vector3 origin = unit.Movement.Position;
                        Vector3 direction = unit.Movement.Forward;
                        List<UnitCombat> AlreadyHitUnits = new List<UnitCombat>(8);

                        //critical
                        if (critArea > 0.01f)
                        {
                            foreach (var o in unit.CurrentBranch.ObjectsVisible)
                            {
                                ServerUnit u = o as ServerUnit;

                                if (u == null)
                                    continue;

                                if (u == unit)
                                    continue;

                                UnitCombat com = u.Combat;

                                if (com != null)
                                {
                                    if (!DoCritHitBoxTestAngle(unit, u.Movement.Position, reach, critArea))
                                        continue;

                                    com.Hit(DamageType, HitType.Melee, HitStrenght.Critical, unit.Combat,
                                            BaseDamage*GetStrenghtDamageRatio(strenght)*critDmg);
                                    AlreadyHitUnits.Add(com);
                                }
                            }
                        }

                        //normal
                        foreach (var o in unit.CurrentBranch.ObjectsVisible)
                        {
                            ServerUnit u = o as ServerUnit;

                            if (u == null)
                                continue;

                            if (u == unit)
                                continue;

                            UnitCombat com = u.Combat;

                            if (com != null)
                            {
                                if (AlreadyHitUnits.Contains(com))
                                    continue;

                                if (!DoNormalHitBoxTestAngle(unit, u.Movement.Position, reach))
                                    continue;

                                com.Hit(DamageType, HitType.Melee, hitStrenght, unit.Combat,
                                        BaseDamage*GetStrenghtDamageRatio(strenght));
                            }
                        }
                    }
                    else // for line radius we use unity physics
                    {
                        float reach = unit.Attributes.Get(UnitAttributeProperty.WeaponReach);
                        float critArea = unit.Attributes.Get(UnitAttributeProperty.CriticalArea);
                        float critDmg = unit.Attributes.Get(UnitAttributeProperty.CriticalDamage);
                        const int layer = 1 << 30;
                        Vector3 origin = unit.Movement.Position;
                        Vector3 direction = unit.Movement.Forward;

                        List<UnitCombat> AlreadyHitUnits = new List<UnitCombat>(8);
                        //critical
                        if (critArea > 0.01f)
                        {
                            foreach (
                                var hit in
                                    Physics.SphereCastAll(
                                        (origin + Vector3.up*0.5f) + (direction*LineWidth/2f) +
                                        (direction*(reach - (reach*critArea) - (LineWidth/2f))), LineWidth,
                                        direction,
                                        reach - (reach*(1f - critArea)) - LineWidth/2f,
                                        layer))
                            {
                                var combat = hit.collider.GetComponent<UnitCombat>();
                                if (combat != null && combat != unit.Combat)
                                {
                                    combat.Hit(DamageType, HitType.Melee, HitStrenght.Critical, unit.Combat,
                                               BaseDamage*GetStrenghtDamageRatio(strenght)*critDmg);
                                    AlreadyHitUnits.Add(combat);
                                }
                            }
                        }
                        //Normal hit
                        {
                            foreach (
                                var hit in
                                    Physics.SphereCastAll((origin + Vector3.up*0.5f) + (direction*LineWidth/2f),
                                                          LineWidth,
                                                          direction,
                                                          reach - LineWidth/2f,
                                                          layer))
                            {
                                var combat = hit.collider.GetComponent<UnitCombat>();
                                if (combat != null && combat != unit.Combat)
                                {
                                    if (!AlreadyHitUnits.Contains(combat))
                                        combat.Hit(DamageType, HitType.Melee, hitStrenght, unit.Combat,
                                                   BaseDamage*GetStrenghtDamageRatio(strenght));
                                }
                            }
                        }

                    }
                });
            });
        }

        public override void OnStartCasting(ServerUnit unit)
        {
            unit.Anim.ActionAnimation = PowerAnim;
        }

        public override void OnStrenghtChanged(ServerUnit unit, float strenght)
        {

        }

        [Obsolete("Use Unit.Spells.CancelSpell")]
        public override void CancelCasting(ServerUnit unit)
        {
            unit.Anim.ActionAnimation = "CancelAction";
        }

        private bool DoNormalHitBoxTestAngle(ServerUnit unit, Vector3 target, float reach)
        {
            float distance = Vector3.Distance(unit.Movement.Position, target);
            //Angle stuff
            if (distance < reach)
            {
                Vector3 referenceForward = unit.Movement.Forward;
                Vector3 referenceRight = Vector3.Cross(Vector3.up, referenceForward);
                Vector3 newDirection = target - unit.Movement.Position;
                float angle = Vector3.Angle(newDirection, referenceForward);
                float sign = Mathf.Sign(Vector3.Dot(newDirection, referenceRight));
                float finalAngle = sign * angle;

                if (_radiusType == RadiusType._90Degrees)
                {
                    if (angle > -45 && angle < 45)
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

        private bool DoCritHitBoxTestAngle(ServerUnit unit, Vector3 target, float reach, float critarea)
        {
            float distance = Vector3.Distance(unit.Movement.Position, target);
            //Angle stuff
            if (distance < reach && distance > reach*(1f-critarea))
            {
                Vector3 referenceForward = unit.Movement.Forward;
                Vector3 referenceRight = Vector3.Cross(Vector3.up, referenceForward);
                Vector3 newDirection = target - unit.Movement.Position;
                float angle = Vector3.Angle(newDirection, referenceForward);
                float sign = Mathf.Sign(Vector3.Dot(newDirection, referenceRight));
                float finalAngle = sign * angle;

                if (_radiusType == RadiusType._90Degrees)
                {
                    if (angle > -45 && angle < 45)
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
