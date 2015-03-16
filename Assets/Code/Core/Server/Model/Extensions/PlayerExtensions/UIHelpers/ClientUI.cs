using Libaries.Net.Packets.ForClient;
using Server.Model.Extensions.UnitExts;
using Shared.Content;
#if SERVER
using System;
using Libaries.Net.Packets.ForServer;

using Code.Libaries.Generic.Managers;
using System.Collections.Generic;
using Code.Core.Client.UI;
using Code.Libaries.Net.Packets.ForServer;
using Server.Model.Entities.Human;
using Server.Model.Extensions.PlayerExtensions.UIHelpers.Interfaces;
using UnityEngine;

namespace Server.Model.Extensions.PlayerExtensions.UIHelpers
{

    public class ClientUI
    {

        private Dictionary<InterfaceType, bool> OpenedInterfaces = new Dictionary<InterfaceType, bool>();
        private ServerClient Client;
        private ClientInventoryInterface _inventoryInterface;
        private LobbyInterface _lobby;
        private ProfileInterface _profileInterface;
        private CreateCharacterInterface _createCharacterInterface;

        public Player Player { get { return Client.Player; } }

        public ClientInventoryInterface Inventories
        {
            get
            {
                if (_inventoryInterface == null)
                {
                    _inventoryInterface = new ClientInventoryInterface(Player);
                }
                return _inventoryInterface;
            }
        }

        public ProfileInterface ProfileInterface
        {
            get
            {
                if (_profileInterface == null)
                {
                    _profileInterface = new ProfileInterface(this);
                }
                return _profileInterface;
            }
        }

        public LobbyInterface Lobby
        {
            get
            {
                if (_lobby == null)
                {
                    _lobby = new LobbyInterface(this);
                }
                return _lobby;
            }
        }

        public LoginInterface Login { get; private set; }

        public CreateCharacterInterface CreateCharacterInterface
        {
            get
            {
                if (_createCharacterInterface == null)
                {
                    _createCharacterInterface = new CreateCharacterInterface(this);
                }
                return _createCharacterInterface;
            }
            set { _createCharacterInterface = value; }
        }

        public ClientUI(ServerClient client)
        {
            Client = client;
            Login = new LoginInterface(this);
        }

        public void OnUIEvent(UIInterfaceEvent e)
        {
            if (e.interfaceId == InterfaceType.ActionBars)
            {
                int spell = e.controlID;
                if (e._eventType == UIInterfaceEvent.EventType.Button_Down)
                {
                    Player.Actions.StartSpell(spell);
                    return;
                }

                if (e._eventType == UIInterfaceEvent.EventType.CLICK)
                {
                    Player.Actions.FinishSpell(spell);
                    return;
                }

            }
            if (e.interfaceId == InterfaceType.MyCharacterInventory)
            {
                Player.Inventory.OnInterfaceEvent(Player, e);
                return;
            }
            if (e.interfaceId == InterfaceType.ProfileInterface)
            {
                ProfileInterface.OnEvent(e.Action, e.controlID);
                return;
            }
            if (e.interfaceId == InterfaceType.CreateCharacterInterface)
            {
                CreateCharacterInterface.OnEvent(e.Action, e.controlID);
                return;
            }

            if (e.interfaceId == InterfaceType.LowerLeftMenu)
            {
                if (e._eventType == UIInterfaceEvent.EventType.CLICK)
                {
                    if (e.Action == "Open Bag")
                    {

                        OpenOrCloseInterface(InterfaceType.MyCharacterInventory);
                        return;

                    }
                    if (e.Action == "View Equipment")
                    {

                        if (ProfileInterface.Opened)
                        {
                            ProfileInterface.Opened = false;
                        }
                        else
                        {
                            ProfileInterface.Open(Player, ProfileInterfaceUpdatePacket.PacketTab.Equipment);
                        }
                        return;
                    }
                    if (e.Action == "View Profile")
                    {

                        if (ProfileInterface.Opened)
                        {
                            ProfileInterface.Opened = false;
                        }
                        else
                        {
                            ProfileInterface.Open(Player, ProfileInterfaceUpdatePacket.PacketTab.Main);
                        }
                        return;
                    }
                }
            }

            if (e.interfaceId == InterfaceType.Chat)
            {
                if (e.controlID == 0) // start talking
                {
                    return;
                }
                return;
            }

            if (e.interfaceId == InterfaceType.StatsBars)
            {
                if (e.Action == "Rest")
                {
                    var Actions = Player.Actions;
                    if (!Actions.HasSpell(ContentManager.I.RestSpell, 5))
                        Actions.EquipSpell(ContentManager.I.RestSpell, 5);
                    Player.Actions.StartOrStopSpell(5);

                    return;
                }
                if (e.Action == "Run")
                {
                    Player.Movement.Running = !Player.Movement.Running;
                    return;
                }
            }

            if (e.Action == "Close interface")
            {
                Close(e.interfaceId);
                return;
            }

            if (e.interfaceId == InterfaceType.LobbyInterface)
            {
                Lobby.OnEvent(e.Action, e.controlID);
            }

            Debug.LogError("Unknown event: " + e.Action + " " + e._eventType + " interface id: " + e.interfaceId + " control id: " + e.controlID);
        }

        internal void OpenOrCloseInterface(InterfaceType type)
        {
            if (!_IsInterfaceOpened(type))
            {
                _OpenInterface(type);
            }
            else
            {
                _CloseInterface(type);
            }
        }

        private void _CloseInterface(InterfaceType type)
        {
            UIInterfaceEvent closePacket = new UIInterfaceEvent();

            closePacket.interfaceId = type;
            closePacket._eventType = UIInterfaceEvent.EventType.HIDE_INTERFACE;

            Client.ConnectionHandler.SendPacket(closePacket);

            OpenedInterfaces[type] = false;
        }

        private void _OpenInterface(InterfaceType type)
        {
            UIInterfaceEvent showPacket = new UIInterfaceEvent();

            showPacket.interfaceId = type;
            showPacket._eventType = UIInterfaceEvent.EventType.SHOW_INTERFACE;

            Client.ConnectionHandler.SendPacket(showPacket);

            OpenedInterfaces[type] = true;
        }

        private bool _IsInterfaceOpened(InterfaceType type)
        {
            if (!OpenedInterfaces.ContainsKey(type))
            {
                OpenedInterfaces.Add(type, false);
            }
            return OpenedInterfaces[type];
        }

        public void ShowControl(InterfaceType interfaceType, int id)
        {
            UIInterfaceEvent showPacket = new UIInterfaceEvent();

            showPacket.interfaceId = interfaceType;
            showPacket._eventType = UIInterfaceEvent.EventType.SHOW;
            showPacket.controlID = id;

            Client.ConnectionHandler.SendPacket(showPacket);
        }

        public void HideControl(InterfaceType interfaceType, int id)
        {
            UIInterfaceEvent hidePacket = new UIInterfaceEvent();

            hidePacket.interfaceId = interfaceType;
            hidePacket._eventType = UIInterfaceEvent.EventType.HIDE;
            hidePacket.controlID = id;

            Client.ConnectionHandler.SendPacket(hidePacket);
        }

        public void SetControlValues(InterfaceType interfaceType, int id, List<float> values)
        {
            UIInterfaceEvent packet = new UIInterfaceEvent();

            packet.interfaceId = interfaceType;
            packet._eventType = UIInterfaceEvent.EventType.SEND_DATA;
            packet.controlID = id;
            packet.values = values;

            Client.ConnectionHandler.SendPacket(packet);
        }

        public void Open(InterfaceType type)
        {
            if (_IsInterfaceOpened(type))
            {
                Debug.LogError("err");
                return;
            }
            _OpenInterface(type);
        }

        public void Close(InterfaceType type)
        {
            if (!_IsInterfaceOpened(type))
            {
                return;
            }
            _CloseInterface(type);
        }

        public void OnItemDrag(ItemDragPacket p)
        {
            if (p.BeingDragInterfaceID == InterfaceType.MyCharacterInventory && p.DropOnInterfaceID == InterfaceType.MyCharacterInventory)
            {
                Player.Inventory.MoveItem(p.BeingDragID, p.DropOnID, Player.Inventory);
                return;
            }
            if (p.BeingDragInterfaceID == InterfaceType.MyCharacterInventory && p.DropOnInterfaceID == InterfaceType.ProfileInterface)
            {
                UnitAccess a = ProfileInterface.Unit.Access.GetAccessFor(Player);
                if (a.View_Inventory && a.Add_To_Inventory && p.DropOnID > 500)
                    Player.Inventory.MoveItem(p.BeingDragID, p.DropOnID - 500, ProfileInterface.Unit.GetExt<UnitInventory>());
                return;
            }
            if (p.BeingDragInterfaceID == InterfaceType.ProfileInterface && p.DropOnInterfaceID == InterfaceType.MyCharacterInventory)
            {
                UnitAccess a = ProfileInterface.Unit.Access.GetAccessFor(Player);
                if (a.View_Inventory && a.Add_To_Inventory && p.BeingDragID > 500)
                    ProfileInterface.Unit.GetExt<UnitInventory>().MoveItem(p.BeingDragID - 500, p.DropOnID, Player.Inventory);
                return;
            }
            throw new Exception("Unhandled item drag. p.BeingDragInterfaceID == " + p.BeingDragInterfaceID + " p.DropOnInterfaceID ==" + p.DropOnInterfaceID);
        }

        public bool IsOpened(InterfaceType type)
        {
            return _IsInterfaceOpened(type);
        }

        public bool IsClosed(InterfaceType type)
        {
            return !IsOpened(type);
        }
    }
}

#endif
