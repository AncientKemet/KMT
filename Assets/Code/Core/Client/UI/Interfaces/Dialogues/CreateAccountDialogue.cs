using Client.Net;
using Client.UI.Controls;
using Client.UI.Controls.Inputs;
using Libaries.IO;
using Libaries.Net.Packets.ForServer;
using UnityEngine;

namespace Client.UI.Interfaces.Dialogues
{
    public class CreateAccountDialogue : DialogueInterface
    {

        [SerializeField] private TextField Username, Password, PasswordVerify, eMail;

        [SerializeField] private TextButton Ok, Cancel;
        [SerializeField] private tk2dTextMesh PasswordLabel, eMailLabel;

        private void Start()
        {
            Ok.OnLeftClick += () =>
            {
                var packet = new AccountPacket();

                packet.JsonObject = new JSONObject();
                packet.JsonObject.AddField("username", Username.Text);
                packet.JsonObject.AddField("password", Password.Text);
                packet.JsonObject.AddField("email", eMail.Text);

                ClientCommunicator.Instance.LoginServerConnection.SendPacket(packet);
            };
            Cancel.OnLeftClick += Close;
        }

        private void FixedUpdate()
        {
            if (!eMail.Text.Contains("@") || !eMail.Text.Contains("."))
            {
                eMailLabel.text = "Incorrect e-mail.";
            }
            else
            {
                eMailLabel.text = "";
            }
            if (Password.Text != PasswordVerify.Text)
            {
                PasswordLabel.text = "Incorrect e-mail.";
            }
            else if (Password.Text.Length < 6)
            {
                PasswordLabel.text = "Password must contain atleas 6 characters.";
            }
            else
            {
                PasswordLabel.text = "";
            }
        }

        public static void Open()
        {
            Create<CreateAccountDialogue>();
        }

    }
}
