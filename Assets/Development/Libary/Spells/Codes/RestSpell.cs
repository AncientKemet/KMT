#if SERVER
using Server.Model.Entities;
#endif
using Shared.Content.Types;

namespace Shared.Content.Spells.Codes
{
    public class RestSpell : Spell
    {

        public Buff RestBuff;

#if SERVER
        public override void OnStartCasting(ServerUnit unit)
        {
            unit.Anim.StandAnimation = "Rest";
            unit.Attributes.AddBuff(RestBuff);
        }

        public override void OnStrenghtChanged(ServerUnit unit, float strenght)
        {

        }

        public override void CancelCasting(ServerUnit unit)
        {
            unit.Anim.SetDefaults();
            unit.Attributes.RemoveBuff(RestBuff);
        }
        
        public override void OnFinishCasting(ServerUnit unit, float strenght)
        {
            unit.Anim.SetDefaults();
            unit.Attributes.RemoveBuff(RestBuff);
        }
#endif
#if UNITY_EDITOR
        [UnityEditor.MenuItem("Kemet/Create/Spell/Rest")]
        public static void CreateTest()
        {
            CreateSpell<RestSpell>();
        }
#endif

    }
}
