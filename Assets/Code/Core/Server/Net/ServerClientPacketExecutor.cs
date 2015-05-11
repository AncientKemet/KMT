using Server.Servers;
#if SERVER
using Server.Model.Content;

using Libaries.Net;
using Libaries.Net.Packets;
using Libaries.Net.Packets.ForClient;
using Libaries.Net.Packets.ForServer;
using Server.Model.ContentHandling;
using System;
using Server.Model.Entities.Human;
using Code.Code.Libaries.Net;
using Code.Code.Libaries.Net.Packets;
using Code.Libaries.Net.Packets.ForServer;
using Code.Libaries.Net.Packets.InGame;
using Server.Model.Entities;
using Server.Model.Extensions.PlayerExtensions;
using Server.Model.Extensions.PlayerExtensions.UIHelpers;
using Server.Model.Extensions.UnitExts;
using UnityEngine;
using Libaries.Net.Packets.Data;
namespace Server.Net
{
    public class ServerClientPacketExecutor : PacketExecutor
    {
        private ServerClient client;

        public ServerClientPacketExecutor(ServerClient client)
        {
            this.client = client;
        }

        protected override void aExecutePacket(BasePacket packet)
        {
            #region From client
            #region AuthenticationPacket
            if (packet is AuthenticationPacket)
            {
                AuthenticationPacket authenticationPacket = packet as AuthenticationPacket;

                if (!(client.Server is LoginServer))
                {
                    Debug.LogError("incorrect server");
                    return;
                }

                client.ConnectionHandler.SendPacket(new LoginResponsePacket("Waiting for data server... please wait"));
                (client.Server as LoginServer).Authorize(client, authenticationPacket.Username, authenticationPacket.Password,
                    (success, DataBaseID) =>
                    {
                        if (success)
                        {
                            client.UserAccount = new UserAccount(client.Server.DataServerConnection, DataBaseID);

                            client.UserAccount.Username = authenticationPacket.Username;
                            client.UserAccount.Password = authenticationPacket.Password;

                            client.Authenticated = true;

                            //set data hes online
                            client.Server.DataServerConnection.SetData("u/" + authenticationPacket.Username + "/isOnline", "1");
                            //when he disconnects set he's offline
                            client.OnDisconnect += () => client.Server.DataServerConnection.SetData("u/" + authenticationPacket.Username + "/isOnline", "0");

                            client.ConnectionHandler.SendPacket(new LoginResponsePacket("Authorization successfull... please wait", client.UserAccount.DataServerKey));

                            Action a = () =>
                            {
                                //client.Player = ServerMonoBehaviour.CreateInstance<Player>();
                                //client.Player.Client = client;
                                /**/
                                //hide loginscreen
                                client.UI.Login.Opened = false;

                                //show lobby screen
                                client.UI.Lobby.Opened = true;
                            };
                            ServerSingleton.StuffToRunOnUnityThread.Add(a);
                        }
                        else
                        {
                            client.ConnectionHandler.SendPacket(new LoginResponsePacket("Incorrect Username or Password."));
                        }
                    })
                ;
                return;

                if (client.Player == null)
                {

                    Action actionToRunOnUnityThread = delegate
                    {
                        var player = ServerMonoBehaviour.CreateInstance<Player>();
                        player.name = authenticationPacket.Username;
                        player.Password = authenticationPacket.Password;

                        player.Client = client;
                    };

                    ServerSingleton.StuffToRunOnUnityThread.Add(actionToRunOnUnityThread);
                }
                return;
            }
            #endregion
            #region SecuredDataPacket
            if (packet is SecuredDataPacket)
            {
                if (client.Server is WorldServer)
                {
                    SecuredDataPacket p = packet as SecuredDataPacket;
                    client.UserAccount = new UserAccount(client.Server.DataServerConnection, p.DataKey);
                    client.UserChat = new UserChat(client);
                    client.UI = new ClientUI(client);
                    Action actionToRunOnUnityThread = delegate
                    {
                        var player = ServerMonoBehaviour.CreateInstance<Player>();
                        client.Player = player;
                        player.Client = client;
                        player.name = "Loading...";
                        client.UserAccount.LoadUnit(client.Server as WorldServer, player);
                    };

                    ServerSingleton.StuffToRunOnUnityThread.Add(actionToRunOnUnityThread);
                }
                else
                {
                    Debug.LogError("Incorrect server for SecuredDataPacket");
                }
                return;
            }
            #endregion

            if (packet is UIInterfaceEvent)
            {
                ClientUI ui = client.UI;
                if (ui != null)
                {
                    ui.OnUIEvent(packet as UIInterfaceEvent);
                }
                return;
            }

            if (packet is CharacterChangePacket)
            {
                CharacterChangePacket p = packet as CharacterChangePacket;
                if (client.Player != null)
                {
                    if (p.Action == CharacterChangePacket.CharAction.HairType)
                        client.Player.Display.Hairtype = p.value;

                    if (p.Action == CharacterChangePacket.CharAction.HairColor)
                        client.Player.Display.HairColor = p.value;

                    if (p.Action == CharacterChangePacket.CharAction.FaceType)
                        client.Player.Display.FaceType = p.value;

                    if (p.Action == CharacterChangePacket.CharAction.FaceColor)
                        client.Player.Display.FaceColor = p.value;

                    if (p.Action == CharacterChangePacket.CharAction.SkinColor)
                        client.Player.Display.SkinColor = p.value;

                    if (p.Action == CharacterChangePacket.CharAction.UnderwearColor)
                        client.Player.Display.UnderwearColor = p.value;

                    if (p.Action == CharacterChangePacket.CharAction.Gender)
                        client.Player.Display.ModelID = p.value == 1 ? 0 : 1;

                }
                return;
            }

            if (client.Server is WorldServer)
                if (packet is WalkRequestPacket)
                {
                    WalkRequestPacket update = packet as WalkRequestPacket;
                    UnitMovement mov = client.Player.GetExt<UnitMovement>();
                    if (mov != null)
                    {
                        mov.WalkWay(update.DirecionVector);
                    }
                    return;
                }

            if (packet is ItemDragPacket)
            {
                client.Player.ClientUi.OnItemDrag(packet as ItemDragPacket);
                return;
            }

            if (packet is InputEventPacket)
            {
                InputEventPacket inputEventPacket = packet as InputEventPacket;
                client.Player.PlayerInput.AddInput(inputEventPacket.type);
                return;
            }

            if (client.Server is WorldServer)
                if (packet is TargetUpdatePacket)
                {
                    TargetUpdatePacket p = packet as TargetUpdatePacket;

                    /*if (client.Player.Focus.FocusedUnit != null)
                        if (client.Player.Focus.FocusedUnit.ID != p.UnitId)
                        {
                            client.Player.Focus.FocusedUnit.Focus..Remove(client.Player);
                        }*/

                    if (p.UnitId == -1)
                    {
                        client.Player.Focus.FocusedUnit = null;
                    }

                    if (p.UnitId != -1)
                    {
                        ServerUnit selectedUnit = client.Player.CurrentWorld[p.UnitId];

                        client.Player.Focus.FocusedUnit = selectedUnit;
                        client.Player.Anim.LookingAt = selectedUnit;

                        if (selectedUnit != null)
                        {
                            UnitSelectionPacketData data = new UnitSelectionPacketData();

                            data.HasCombat = selectedUnit.Combat != null;
                            data.HasAttributes = selectedUnit.Attributes != null;

                            data.UnitName = selectedUnit.name;

                            if (data.HasAttributes)
                            {
                                data.Attributes = selectedUnit.Attributes.Attributes;
                            }

                            client.ConnectionHandler.SendPacket(data);
                        }
                    }
                    return;
                }

            if (client.Server is WorldServer)
                if (packet is UnitActionPacket)
                {
                    UnitActionPacket p = packet as UnitActionPacket;
                    client.Player.Actions.DoAction(p.UnitId, p.ActionName);

                    return;
                }

            if (packet is ChatPacket)
            {
                ChatPacket p = packet as ChatPacket;
                client.UserChat.HandlePacket(p);
                return;
            }
            #endregion

            if (packet is DataPacket)
            {
                #region DataPackets

                if (packet is DataRequestPacket)
                {
                    if (client.Server is DataServer)
                    {
                        DataRequestPacket p = packet as DataRequestPacket;
                        DataServer dataserver = (client.Server as DataServer);

                        dataserver.DataProvider.GetData(p.DataPath, (success, data) =>
                        {
                            DataResponsePacket response = new DataResponsePacket();
                            response.ID = p.ID;
                            response.Success = success;
                            response.Certificate = ServerSingleton.Instance.DataCertificate;
                            response.DataPath = p.DataPath;
                            response.Value = data;
                            client.ConnectionHandler.SendPacket(response);
                        });

                        return;
                    }
                    else
                    {
                        Debug.LogError("DataRequestPacket came to non dataserver wtf.");
                    }
                }

                if (packet is DataSetPacket)
                {
                    if (client.Server is DataServer)
                    {
                        DataSetPacket p = packet as DataSetPacket;
                        DataServer dataserver = (client.Server as DataServer);

                        dataserver.DataProvider.SetData(p.DataPath, p.Data, (success) =>
                        {
                            if (!success)
                            {
                                Debug.LogError("Failed to set data: " + p.Data + " to path: " + p.DataPath);
                            }
                        });

                        return;
                    }
                    else
                    {
                        Debug.LogError("DataSetPacket came to non dataserver wtf.");
                    }
                }

                if (packet is DataReplacePacket)
                {
                    if (client.Server is DataServer)
                    {
                        var p = packet as DataReplacePacket;
                        DataServer dataserver = (client.Server as DataServer);

                        dataserver.DataProvider.ReplaceData(p.DataPath, p.OldValue, p.NewValue, (success) =>
                        {
                            if (!success)
                            {
                                Debug.LogError("Failed to replace data: " + p.OldValue + " to path: " + p.DataPath);
                            }
                        });

                        return;
                    }
                    else
                    {
                        Debug.LogError("DataReplacePacket came to non dataserver wtf.");
                    }
                }

                if (packet is DataAppendPacket)
                {
                    if (client.Server is DataServer)
                    {
                        var p = packet as DataAppendPacket;
                        DataServer dataserver = (client.Server as DataServer);

                        dataserver.DataProvider.AppendData(p.DataPath, p.Data, (success) =>
                        {
                            if (!success)
                            {
                                Debug.LogError("Failed to append data: " + p.Data + " to path: " + p.DataPath);
                            }
                        });

                        return;
                    }
                    else
                    {
                        Debug.LogError("DataSetPacket came to non dataserver wtf.");
                    }
                }

                #endregion
            }

            Debug.LogError("Unable to decode packet: " + packet.GetType().Name + " that came to: " + client.Server.GetType().Name);
        }
    }
}
#endif
