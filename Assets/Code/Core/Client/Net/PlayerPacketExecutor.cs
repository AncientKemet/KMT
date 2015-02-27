using Client.UI.Interfaces;
using Client.UI.Interfaces.Lobby;
using Client.UI.Interfaces.Profile;
using Client.UI.Scripts;
using Client.Units;
using Code.Code.Libaries.Net;
using Code.Code.Libaries.Net.Packets;
using Code.Core.Client.UI.Controls.Items;
using Code.Core.Client.UI.Interfaces;
using Code.Core.Client.UI.Interfaces.UpperLeft;
using Code.Core.Client.Units;
using Code.Core.Client.Units.Managed;
using Code.Libaries.Net.Packets.ForClient;
using Code.Libaries.Net.Packets.ForServer;
using Libaries.Net;
using Libaries.Net.Packets.ForClient;
using UnityEngine;

namespace Client.Net
{
    public class PlayerPacketExecutor : PacketExecutor
    {
        protected override void aExecutePacket(BasePacket packet)
        {
            if (packet is UIPacket)
            {
                UIPacket p = packet as UIPacket;
                if (p.type == UIPacket.UIPacketType.SEND_MESSAGE)
                {
                    if (ChatPanel.I != null)
                        ChatPanel.I.AddMessage(p.textData);
                }
            }else if (packet is SpellUpdatePacket)
            {
                var p = packet as SpellUpdatePacket;
                ActionBars.I.OnPacket(p);
            }
            else if (packet is EnterWorldPacket)
            {
                EnterWorldPacket p = packet as EnterWorldPacket;
                //load some world

                PlayerUnit.MyPlayerUnit = UnitManager.Instance[p.myUnitID];
                PlayerUnit.MyPlayerUnit.transform.position = p.Position;

                /*UnitSelectionInterface.I.Show();
                ChatPanel.I.Show();
                ItemInventoryInterface.I.Show();
                LowerRight.I.Show();
                StatsBarInterfaces.I.Show();
                ChatInterface.I.Show();
                ActionBars.I.Show();*/

                if (LoginInterface.I.Visible)
                {
                    LoginInterface.I.Hide();
                }
                if (LobbyInterface.I.Visible)
                {
                    LobbyInterface.I.Hide();
                }
            }
            else if (packet is UnitUpdatePacket)
            {
                UnitUpdatePacket p = packet as UnitUpdatePacket;
                UnitManager.Instance[p.UnitID].DecodeUnitUpdate(p);
            }
            else if (packet is UIInterfaceEvent)
            {
                UIInterfaceEvent Event = packet as UIInterfaceEvent;

                if (Event._eventType == UIInterfaceEvent.EventType.HIDE_INTERFACE)
                {
                    InterfaceManager.GetInterface(Event.interfaceId).Hide();
                }
                else if (Event._eventType == UIInterfaceEvent.EventType.SHOW_INTERFACE)
                {
                    InterfaceManager.GetInterface(Event.interfaceId).Show();
                }
                else if (Event._eventType == UIInterfaceEvent.EventType.SHOW)
                {
                    InterfaceManager.GetInterface(Event.interfaceId)[Event.controlID].Show();
                }
                else if (Event._eventType == UIInterfaceEvent.EventType.HIDE)
                {
                    InterfaceManager.GetInterface(Event.interfaceId)[Event.controlID].Hide();
                }
                else if (Event._eventType == UIInterfaceEvent.EventType.SEND_DATA)
                {
                    InterfaceManager.GetInterface(Event.interfaceId)[Event.controlID].OnSetData(Event.values);
                }
                else
                {
                    Debug.LogError("Bad ui event type: " + packet.GetType());
                }
            }
            else if (packet is UnitSelectionPacketData)
            {
                UnitSelectionInterface.I.OnDataRecieved(packet as UnitSelectionPacketData);
            }
            else if (packet is UIInventoryInterfacePacket)
            {
                UIInventoryInterfacePacket p = packet as UIInventoryInterfacePacket;
                ItemInventoryInterface.I.Handle(p);
            }
            else if (packet is ChatPacket)
            {
                ChatPacket p = packet as ChatPacket;
                if (LobbyChatBar.instance != null && p.type == ChatPacket.ChatType.Private)
                {
                    LobbyChatBar.instance.GetLobbyChatPanel(p.User).AddMessage("^c77DF"+p.User+": " + p.text);
                }
                else
                {
                    ChatPanel.I.AddMessage(p);
                }
            }
            else if (packet is LoginResponsePacket)
            {
                LoginResponsePacket p = packet as LoginResponsePacket;
                LoginInterface.I.StatusLabel.text = p.Text;
                if (!string.IsNullOrEmpty(p.DataServerKey) && p.DataServerKey != "null")
                {
                    LobbyInterface.I.DataServerKey = p.DataServerKey;
                    Debug.Log("recieved datakey: -" + p.DataServerKey + "-");
                }
                if (p.Text.Contains("Incorrect"))
                {
                    LoginInterface.I.WaitingForResponse = false;
                    LoginInterface.I.LoginButton.Show();
                }
            }
            else if (packet is ProfileInterfaceUpdatePacket)
            {
                ProfileInterfaceUpdatePacket p = packet as ProfileInterfaceUpdatePacket;
                ProfileInterface.I.OnPacket(p);
            }else if (packet is CharacterCustomalizationDataPacket)
            {
                var p = packet as CharacterCustomalizationDataPacket;
                CreateCharacterInterface.Data = p;
            }
            else if (packet is BuffUpdatePacket)
            {
                var p = packet as BuffUpdatePacket;
                PlayerUnit unit = UnitManager.Instance[p.UnitId];
                if (unit != null)
                {
                    if(p.AddOrRemove)
                        unit.AddBuff(p.BuffInstance);
                    else
                        unit.RemoveBuff(p.BuffInstance);
                }
            }
            else
            {
                Debug.LogError("Unknown packet type: " + packet.GetType());
            }
        }
    }
}
