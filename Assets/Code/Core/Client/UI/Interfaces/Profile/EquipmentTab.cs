using System.Collections.Generic;
using Client.UI.Controls.Items;
using Client.Units;
using Code.Libaries.Generic.Managers;
using Shared.Content.Types;
using UnityEngine;

namespace Client.UI.Interfaces.Profile
{
    public class EquipmentTab : ProfileTab
    {

        private PlayerUnit Unit { get; set; }

        public ItemButton HeadButton, ChestButton, LegsButton, BootsButton, MainHandButton, OffHandButton;

        public Transform AttributeContainer;

        public EquipmentTabDetail CurrentDetail
        {
            get { return _currentDetail; }
            set
            {
                if(_currentDetail != value)
                    _currentDetail.ChildTab.gameObject.SetActive(false);
                _currentDetail = value;
                value.ChildTab.gameObject.SetActive(true);
            }
        }

        private Dictionary<UnitAttributeProperty, Attribute> Attributes;
        [SerializeField]
        private EquipmentTabDetail _currentDetail;

        public override void ReloadFromUnit(PlayerUnit unit)
        {
            if (Unit != unit)
            {
                if (Unit != null)
                {
                    Unit.Display.OnEquipmentChanged -= RefreshItems;
                    Unit.PlayerUnitAttributes.OnChange -= RefreshStats;
                }
            }
            Unit = unit;
            base.ReloadFromUnit(unit);
            RefreshItems();

            foreach (var o in Unit.PlayerUnitAttributes)
            {
                var kv = (KeyValuePair<UnitAttributeProperty, float>) o;
                RefreshStats(kv.Key, kv.Value);
            }
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
                Unit.PlayerUnitAttributes.OnChange += RefreshStats;
            }
        }

        private void RefreshStats(UnitAttributeProperty property, float f)
        {
            if (Attributes == null)
            {
                Attributes = new Dictionary<UnitAttributeProperty, Attribute>();

                foreach (var att in AttributeContainer.GetComponentsInChildren<Attribute>(true))
                {
                    Attributes.Add(att.Property, att);
                }
            }
            try
            {
                string formatedValue = UnitAttributePropertySerializable.GetLabeledString(property, f);
                Attributes[property].ValueLabel.text = formatedValue;
            }
            catch (KeyNotFoundException e)
            {
                Debug.LogError("Missing property: "+property);
            }
        }

        private void OnDisable()
        {
            if (Unit != null)
            {
                Unit.Display.OnEquipmentChanged -= RefreshItems;
                Unit.PlayerUnitAttributes.OnChange -= RefreshStats;
            }
        }
    }
}
