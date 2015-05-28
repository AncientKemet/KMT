#if SERVER


using Server.Model.Entities;
using Shared.Content.Types;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace ReferencedData.Content.Spells.Codes
{
    public class BlockSpell : Spell
    {

        public string PowerAnim = "OffHandBlockPower";




        public override void OnFinishCasting(ServerUnit unit, float strenght)
        {
            unit.Anim.SetDefaults();
            unit.Anim.ActionAnimation = "CancelAction";
        }

        public override void OnStartCasting(ServerUnit unit)
        {
            unit.Anim.ActionAnimation = PowerAnim;
            //todo
        }

        public override void OnStrenghtChanged(ServerUnit unit, float strenght)
        {

        }

        public override void CancelCasting(ServerUnit unit)
        {
            unit.Anim.ActionAnimation = "CancelAction";
        }

#if UNITY_EDITOR
        [MenuItem("Kemet/Create/Spell/Block")]
        public static void CreateTest()
        {
            CreateSpell<BlockSpell>();
        }
#endif

    }
}
#endif
