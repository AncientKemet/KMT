using Shared.Content.Types;
#if SERVER
using Server.Model.Extensions;
using Server.Model.Extensions.UnitExts;

namespace Server.Model.Entities.Minerals
{
    public abstract class Mineral : ServerUnit
    {
        public abstract MineralConfig Config
        {
            get;
        }

        public override void Awake()
        {
            AddExt<CollisionExt>();
            Attributes = AddExt<UnitAttributes>();
            Combat = AddExt<UnitCombat>();
            Attributes.BaseArmor = Config.Resistance;
            Combat.OnHitT += (type, hitType, arg3) =>
            {
                if (type == Spell.DamageType.Physical)
                {
                    if (hitType == Spell.HitType.Melee)
                    {
                        Display.SendEffect(2);
                    }
                }
            };
            base.Awake();
            Movement.CanMove = false;
        }

        public override bool IsStatic()
        {
            return false;
        }
    }
}
#endif
