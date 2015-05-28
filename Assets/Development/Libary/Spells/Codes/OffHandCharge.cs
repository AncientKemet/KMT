using Shared.Content.Types;

#if UNITY_EDITOR
using UnityEditor;
#endif
#if SERVER
using Server.Model.Entities;
#endif


namespace Development.Libary.Spells.Codes
{
    public class OffHandCharge : MeleeSpell
    {


        public Buff SlowDebuff;

#if SERVER

        public override void OnFinishCasting(ServerUnit unit, float strenght)
        {
            unit.Anim.ActionAnimation = AttackAnim;
            unit.Movement.PushForward(strenght*3);
            unit.Attributes.RemoveBuff(SlowDebuff);
        }

        public override void OnStartCasting(ServerUnit unit)
        {
            unit.Anim.ActionAnimation = PowerAnim;
            unit.Attributes.AddBuff(SlowDebuff);
        }

        public override void OnStrenghtChanged(ServerUnit unit, float strenght)
        {

        }

        public override void CancelCasting(ServerUnit unit)
        {
            unit.Attributes.RemoveBuff(SlowDebuff);
        }
#endif
#if UNITY_EDITOR
        [MenuItem("Kemet/Create/Spell/OffHandCharge")]
        public static void CreateTest()
        {
            CreateSpell<OffHandCharge>();
        }
#endif
    }
}
