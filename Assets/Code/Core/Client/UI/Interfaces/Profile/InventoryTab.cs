using Client.Units;
using Code.Core.Client.UI.Controls.Items;

namespace Client.UI.Interfaces.Profile
{
    public class InventoryTab : ProfileTab
    {
        public override void ReloadFromUnit(PlayerUnit unit)
        {
            base.ReloadFromUnit(unit);
        }

        public ItemInventory Inventory;

    }
}
