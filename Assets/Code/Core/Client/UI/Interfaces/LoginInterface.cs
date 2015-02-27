﻿using System.Security.Cryptography;
using Client.Net;
using Client.UI.Controls;
using Client.UI.Controls.Inputs;
using Client.UI.Scripts;
using Code.Code.Libaries.Net.Packets;
using Code.Core.Client.UI;
using Code.Core.Client.UI.Controls;
using Libaries.Net.Packets;
using UnityEngine;

namespace Client.UI.Interfaces
{
    public class LoginInterface : UIInterface<LoginInterface>
    {
        public TextButton LoginButton;
        public TextField Username;
        public TextField Password;
        public tk2dTextMesh StatusLabel;

        public bool WaitingForResponse { get; set; }

        protected override void OnStart()
        {
            base.OnStart();

            LoginButton.OnLeftClick += () =>
            {
                if (!WaitingForResponse)
                {
                    WaitingForResponse = true;
                    LoginButton.Hide();

                    AuthenticationPacket packet = new AuthenticationPacket();

                    packet.Username = Username.Text;
                    packet.Password = Password.Text;

                    ClientCommunicator.Instance.SendToServer(packet);
                    ClientCommunicator.Instance.LastUsernameUsedToLogin = packet.Username;
                }
            };
        }

        public override void Hide()
        {
            Destroy(gameObject);
            ScreenScene.Instance.gameObject.SetActive(false);
        }
    }
}
