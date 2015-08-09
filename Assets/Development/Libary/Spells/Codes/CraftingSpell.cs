using Server.Model.Entities;
using Server.Model.Entities.Human;
using Shared.Content.Types;
using UnityEngine;

namespace Development.Libary.Spells.Codes
{
    /// <summary>
    /// Crafting spells are unique as their cast speeds are not dependant on charge speed.
    /// </summary>
    public class CraftingSpell : Spell
    {
        [SerializeField]
        private string Animation = "Craft";

#if SERVER
        public override void OnStartCasting(ServerUnit unit)
        {
            unit.Anim.ActionAnimation = Animation;
        }

        public override void OnStrenghtChanged(ServerUnit unit, float strenght)
        {
        }

        public override void CancelCasting(ServerUnit unit)
        {
            unit.Anim.SetDefaults();
        }

        public override void OnFinishCasting(ServerUnit unit, float strenght)
        {
            unit.Anim.SetDefaults();
        }
#endif
#if UNITY_EDITOR
        [UnityEditor.MenuItem("Kemet/Create/Spell/Crafting Spell")]
        public static void CreateTest()
        {
            CreateSpell<CraftingSpell>();
        }
#endif
    }
}
