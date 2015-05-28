using System.Collections.Generic;
using Client.UI.Scripts;
using Client.Units;
using Code.Code.Libaries.Net.Packets;
using Code.Core.Client.Units.Managed;
using Libaries.Net.Packets.ForClient;
using UnityEngine;

namespace Client.UI.Interfaces.Profile
{
    public class ProfileInterface : UIInterface<ProfileInterface>
    {

        [SerializeField]
        private tk2dTextMesh Title;

        private PlayerUnit _unit;

        [SerializeField]
        private ProfileTab _currentTab;

        [SerializeField]
        private MainTab _mainTab;
        [SerializeField]
        private EquipmentTab _equipmentTab;
        [SerializeField]
        private AccessTab _accessTab;
        [SerializeField]
        private DialogTab _dialogTab;
        [SerializeField]
        private LevelTab _levelTab;
        [SerializeField]
        private InventoryTab _inventoryTab;
        [SerializeField]
        private VendorTab _vendorTab;
        [SerializeField]
        private TradeTab _tradeTab;

        public tk2dTextMesh LevelLabel;

        public PlayerUnit Unit
        {
            get { return _unit; }
            set
            {
                foreach (var tab in GetComponentsInChildren<ProfileTab>())
                {
                    tab.ReloadFromUnit(value);
                }
                if (value != null)
                {
                    Title.text = value.Name;
                }
                _unit = value;
            }
        }

        public ProfileTab CurrentTab
        {
            get { return _currentTab; }
            set
            {
                if (_currentTab != null) _currentTab.ContentGameObject.SetActive(false);
                _currentTab = value;
                if (value != null) value.ContentGameObject.SetActive(true);
            }
        }

        public void OnPacket(ProfileInterfaceUpdatePacket p)
        {
            List<ProfileTab> tabs = new List<ProfileTab>();

            _mainTab.gameObject.SetActive(p.HasMainTab);
            _accessTab.gameObject.SetActive(p.HasAccessTab && UnitManager.Instance[p.UnitID] != PlayerUnit.MyPlayerUnit);
            _equipmentTab.gameObject.SetActive(p.HasEquipmentTab);
            _dialogTab.gameObject.SetActive(p.HasDialogueTab);
            _inventoryTab.gameObject.SetActive(p.HasInventoryTab && UnitManager.Instance[p.UnitID] != PlayerUnit.MyPlayerUnit);
            _vendorTab.gameObject.SetActive(p.HasVendorTradeTab);
            _tradeTab.gameObject.SetActive(p.HasTradeTab && UnitManager.Instance[p.UnitID] != PlayerUnit.MyPlayerUnit);
            _levelTab.gameObject.SetActive(p.HasLevelsTab);

            tabs.AddRange(new ProfileTab[] { _equipmentTab, _accessTab, _dialogTab, _inventoryTab, _vendorTab, _tradeTab });

            Vector3 offset = _equipmentTab.transform.localPosition;
            foreach (var tab in tabs)
            {
                if (tab.gameObject.activeSelf)
                {
                    tab.transform.localPosition = offset;
                    offset += new Vector3(0, -2.9f, 0);
                }
            }

            Unit = UnitManager.Instance[p.UnitID];
            _accessTab.Access = p.Access;

            if (p.Tab == ProfileInterfaceUpdatePacket.PacketTab.Access)
                CurrentTab = _accessTab;
            if (p.Tab == ProfileInterfaceUpdatePacket.PacketTab.Dialogue)
                CurrentTab = _dialogTab;
            if (p.Tab == ProfileInterfaceUpdatePacket.PacketTab.Equipment)
                CurrentTab = _equipmentTab;
            if (p.Tab == ProfileInterfaceUpdatePacket.PacketTab.Inventory)
                CurrentTab = _inventoryTab;
            if (p.Tab == ProfileInterfaceUpdatePacket.PacketTab.Levels)
                CurrentTab = _levelTab;
            if (p.Tab == ProfileInterfaceUpdatePacket.PacketTab.Main)
                CurrentTab = _mainTab;
            if (p.Tab == ProfileInterfaceUpdatePacket.PacketTab.Trade)
                CurrentTab = _tradeTab;
            if (p.Tab == ProfileInterfaceUpdatePacket.PacketTab.Vendor)
                CurrentTab = _vendorTab;
        }
        
        public void Handle(UIInventoryInterfacePacket packet)
        {
            switch (packet.type)
            {
                case UIInventoryInterfacePacket.PacketType.SHOW:
                    CurrentTab = _inventoryTab;
                    _inventoryTab.Inventory.Width = packet.X;
                    _inventoryTab.Inventory.Height = packet.Y;
                    _inventoryTab.Inventory.ForceRebuild();
                    break;

                case UIInventoryInterfacePacket.PacketType.HIDE:
                    if(Visible)
                    Hide();
                    break;

                case UIInventoryInterfacePacket.PacketType.SetItem:
                    _inventoryTab.Inventory.SetItem(packet.X, packet.Y, packet.Value, packet.Amount);
                    break;
            }
        }
    }
}
