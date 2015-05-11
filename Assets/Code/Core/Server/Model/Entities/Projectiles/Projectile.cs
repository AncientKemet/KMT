#if SERVER
using System;
using Server.Model.Extensions.UnitExts;
using UnityEngine;

namespace Server.Model.Entities.Projectiles
{
    public class Projectile : ServerUnit
    {
        public float Speed = 3f;
        public UnitCombat Caster { get; set; }
        public Action<UnitCombat> OnHit;

        /// <summary>
        /// Amount of targets it can hit before stoping.
        /// </summary>
        public int Piercing = 1;
        
        public void Fire(bool fromCaster = true)
        {
            if (fromCaster)
            {
                var casterMov = Caster.Unit.Movement;
                Movement.Teleport(casterMov.Position);
                Movement.Rotation = casterMov.Rotation;
                UpdateTrajectory();
            }
        }

        public void UpdateTrajectory(){}
        
        protected override ServerUnitPrioritization GetPrioritization()
        {
            return ServerUnitPrioritization.Realtime;
        }

        public override bool IsStatic()
        {
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
    }
}
#endif
