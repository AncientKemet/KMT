using System.Drawing;
using Client.Units.SpellRadiuses;
using Server.Model.Entities.Items;
using UnityEngine.UI;
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
        public RangeLineRadius Radius;
        public string PowerAnim = "OneHandThrowPower";
        public string AttackAnim = "OneHandThrowAttack";
        public float ProcessesPerSecond = 10;
        public float BaseDamage = 2f;

        private RangeLineRadius CurrentRadius;
        public void OnEnable()
        {
            ClientOnStartedCasting += unit =>
            {

                CurrentRadius = Instantiate(Radius.gameObject).GetComponent<RangeLineRadius>();

                CurrentRadius.CriticalArea = unit.PlayerUnitAttributes.GetAttribute(UnitAttributeProperty.CriticalArea);
                CurrentRadius.Range = unit.PlayerUnitAttributes.GetAttribute(UnitAttributeProperty.WeaponRange);
                CurrentRadius.Width = 0.5f;

                CurrentRadius.transform.transform.parent = unit.transform;
                CurrentRadius.transform.localPosition = Vector3.up;
                CurrentRadius.transform.localRotation = Quaternion.identity;
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

#if SERVER

        public override void OnFinishCasting(ServerUnit unit, float strenght)
        {
            Vector3 direction = (unit.Spells.TargetPosition - unit.Movement.Position);
            unit.Anim.ActionAnimation = AttackAnim + (strenght > 0.66 ? "Strong" : "");
            unit.Spells.DisableSpell(0);
            unit.Spells.DisableSpell(1);
            float Range = unit.Attributes[UnitAttributeProperty.WeaponRange];

            #region straight
            unit.Movement.NextMovements.AddLast((f) =>
            {
                unit.Movement.DiscardPath();
                unit.Movement.RotateWay(direction);
                unit.Movement.Push(direction, strenght * 2, () =>
                {
                    direction = direction.normalized;
                    direction.y = 0.07f;
                    direction = direction.normalized;
                    unit.Movement.RotateWay(direction);

                    if (strenght > 0.66f)
                        unit.Attributes.AddBuff(ContentManager.I.OverpowerDebuff, 0.5f);

                    var e = unit.GetExt<UnitEquipment>();
                    var u = e.MainHandUnit;

                    u.Movement.Parent = null;
                    u.Movement.RotateTowardsMovement = true;
                    u.Movement._UnSafeMoveTo(unit.Movement.Position + Vector3.up);
                    unit.Anim.SetDefaults();

                    float rot = unit.Movement.Rotation + 90;
                    float DistanceTraveled = 0;
                    float speed = (1.5f + strenght / 1f);
                    u.Display.SendEffect(0);

                    u.StopAllCoroutines();
                    u.StartCoroutine(SEase.Action(() =>
                    {
                        if (direction != Vector3.zero)
                        {
                            //Raycast next hit
                            RaycastHit hit;
                            Physics.Raycast(u.Movement.Position, direction, out hit, speed * 2, 1 << 8 | 1 << 30);
                            if (hit.collider != null || DistanceTraveled >= Range / 2f + (Range / 2f * strenght))
                            {
                                if (hit.collider != null)
                                {
                                    var combat = hit.collider.GetComponent<UnitCombat>();
                                    if (combat != null)
                                    {
                                        combat.Hit(DamageType.Physical, HitType.Range, HitStrenght.Normal,
                                                   unit.Combat,
                                                   BaseDamage * GetStrenghtDamageRatio(strenght));
                                        u.Movement.Parent = combat.Unit.Movement;
                                        u.Movement.ParentPlaneID = 5;
                                        u.Movement.Rotation -= combat.Unit.Movement.Rotation;
                                        u.Movement.Teleport(hit.point);
                                    }
                                    else
                                    {
                                        u.Movement._UnSafeMoveTo(hit.point + new Vector3(0, 0.1f, 0));
                                    }
                                }
                                else
                                {
                                    // the projectile has hit notihing but has traveled max distance
                                    // so we raycast straight down to ground if there is ground it falls on it
                                    Physics.Raycast(u.Movement.Position, Vector3.down, out hit, 10, 1 << 8);
                                    if (hit.collider != null)
                                        u.Movement._UnSafeMoveTo(hit.point + new Vector3(0, 0.1f, 0));
                                }
                                direction = Vector3.zero;
                                u.StopAllCoroutines();
                                u.StartCoroutine(SEase.Action(() =>
                                {
                                    ReturnSpearToOwner(unit, u);
                                    u.StopAllCoroutines();
                                }, -1, 10f));

                                // every 0.5s check for distance to spear owner
                                u.StartCoroutine(SEase.Action(() =>
                                {
                                    if (u != null)
                                    {
                                        if (unit == null)
                                        {
                                            u.Display.Destroy = true;
                                            return;
                                        }

                                        if (Vector3.Distance(u.Movement.Position, unit.Movement.Position) < 2f)
                                        {
                                            ReturnSpearToOwner(unit, u);
                                            u.StopAllCoroutines();
                                        }
                                    }
                                }, 20, 0.5f));
                            }
                            else
                            {
                                u.Movement.Rotation = rot;
                                u.Movement._UnSafeMoveTo(u.Movement.Position + direction * speed);
                                direction += Vector3.down / 10f;
                                DistanceTraveled += speed;
                            }
                        }
                    }
                                                  , (int)(3 * ProcessesPerSecond), 1 / ProcessesPerSecond));

                });
            });

            #endregion
        }

        public override void OnStrenghtChanged(ServerUnit unit, float strenght)
        {
        }

        private static void ReturnSpearToOwner(ServerUnit owner, DroppedItem u)
        {
            if (owner == null)
            {
                u.Display.Destroy = true;
                return;
            }
            u.Movement.Teleport(owner.Movement.Position);
            u.Movement.Parent = owner.Movement;
            u.Movement.ParentPlaneID = 1;
            u.Movement.Rotation = 0;
            owner.Anim.SetDefaults();
            owner.Spells.EnableSpell(0);
            owner.Spells.EnableSpell(1);
            u.Movement.RotateTowardsMovement = false;
        }

        public override void OnStartCasting(ServerUnit unit)
        {
            var e = unit.GetExt<UnitEquipment>();
            var u = e.MainHandUnit;
            unit.Anim.ActionAnimation = PowerAnim;
            u.StopAllCoroutines();
            u.Movement.DiscardPath();
        }
        
        public override void CancelCasting(ServerUnit unit)
        {
            var e = unit.GetExt<UnitEquipment>();
            var u = e.MainHandUnit;

            unit.Anim.ActionAnimation = "CancelAction";
            u.StopAllCoroutines();
        }


#endif

    }
}
