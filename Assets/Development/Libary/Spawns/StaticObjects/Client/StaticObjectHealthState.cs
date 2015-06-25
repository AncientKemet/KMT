using Client.Units;
using Shared.Content.Types;
using UnityEngine;

namespace Development.Libary.Spawns.StaticObjects.Client
{
    [RequireComponent(typeof(StaticObjectInstance))]
    [RequireComponent(typeof(Animation))]
    public class StaticObjectHealthState : ClientStaticObjectExtension
    {

        private Animation Animation;
        private PlayerUnitAttributes Attributes;

        void Update ()
        {
            if (Attributes != null && Attributes.CurrentHealth >= 0)
            {
                if (Attributes.CurrentHealth > 0)
                {
                    Animation["HealthState"].speed = 0;
                    Animation["HealthState"].time = 1f - ((float) Attributes.CurrentHealth/
                                                          (float) Attributes.GetAttribute(UnitAttributeProperty.Health));
                    Animation.Sample();
                }
                else
                {
                    Animation["HealthState"].speed = 1;
                }
            }
        }

        public override void Apply(PlayerUnit playerUnit)
        {
            Attributes = playerUnit.PlayerUnitAttributes;
            Animation = GetComponent<Animation>();
            Animation["HealthState"].wrapMode = WrapMode.ClampForever;
            Animation["HealthState"].speed = 0;
            Animation.Play("HealthState");
            Animation.Sample();
        }
    }
}
