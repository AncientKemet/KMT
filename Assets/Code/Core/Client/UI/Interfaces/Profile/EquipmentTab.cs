using System;
using Client.UI.Controls.Items;
using Client.Units;
using Code.Core.Client.Units;
using Code.Libaries.Generic.Managers;

namespace Client.UI.Interfaces.Profile
{
    public class EquipmentTab : ProfileTab
    {

        private PlayerUnit Unit { get; set; }

        public ItemButton HeadButton, ChestButton, LegsButton, BootsButton, MainHandButton, OffHandButton;

        public override void ReloadFromUnit(PlayerUnit unit)
        {
            if (Unit != unit)
            {
                if (Unit != null)
                {
                    Unit.Display.OnEquipmentChanged -= RefreshItems;
                }
            }
            Unit = unit;
            base.ReloadFromUnit(unit);
            RefreshItems();
        }

        private void RefreshItems()
        {
            HeadButton.Item = Unit.Display.HeadId >= 0 ? ContentManager.I.Items[Unit.Display.HeadId] : null;
            ChestButton.Item = Unit.Display.ChestId >= 0 ? ContentManager.I.Items[Unit.Display.ChestId] : null;
            LegsButton.Item = Unit.Display.LegsId >= 0 ? ContentManager.I.Items[Unit.Display.LegsId] : null;
            BootsButton.Item = Unit.Display.BootsId >= 0 ? ContentManager.I.Items[Unit.Display.BootsId] : null;
            MainHandButton.Item = Unit.Display.MainHandId >= 0 ? ContentManager.I.Items[Unit.Display.MainHandId] : null;
            OffHandButton.Item = Unit.Display.OffHandId >= 0 ? ContentManager.I.Items[Unit.Display.OffHandId] : null;
        }

        private void OnEnable()
        {
            if (Unit != null)
            {
                Unit.Display.OnEquipmentChanged += RefreshItems;
            }
        }
        private void OnDisable()
        {
            if (Unit != null)
            {
                Unit.Display.OnEquipmentChanged -= RefreshItems;
            }
        }
    }
}
