using System.Collections.Generic;
using Client.Units;

namespace Client.UI.Interfaces.Profile
{
    public class LevelTab : ProfileTab
    {
        public override void ReloadFromUnit(PlayerUnit unit)
        {
            base.ReloadFromUnit(unit);
            if (unit.PlayerUnitAttributes != null)
                CombatLevelLabel.text = "" + unit.PlayerUnitAttributes.Combatlevel;
            else
                CombatLevelLabel.text = "X";
        }

        public tk2dTextMesh CombatLevelLabel;
        public List<SkillButton> Buttons;
    }
}
