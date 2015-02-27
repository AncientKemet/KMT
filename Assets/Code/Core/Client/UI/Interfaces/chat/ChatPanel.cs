using System.Collections.Generic;
using Client.Net;
using Client.UI.Scripts;
using Code.Core.Client.Controls;
using Code.Libaries.UnityExtensions.Independent;
using UnityEngine;
using Code.Libaries.Net.Packets.ForServer;

namespace Code.Core.Client.UI.Interfaces
{
    public class ChatPanel : UIInterface<ChatPanel> {

        public tk2dTextMesh exampleLine;
        public tk2dTextMesh inputTextField, Public, Private, Party;
        public tk2dSlicedSprite slider;
        public tk2dSlicedSprite background;

        public Transform MessageContainer;

        [SerializeField]
        private List<tk2dTextMesh> messages = new List<tk2dTextMesh>();

        private bool _isTyping = false;

        private ChatKeyListener listener = new ChatKeyListener();

        private float PressDelay = 1f;

        public bool isTyping
        {
            get
            {
                return _isTyping;
            }
            set
            {
                _isTyping = value;
                inputTextField.transform.parent.gameObject.SetActive(_isTyping);
                if(slider != null)
                    slider.gameObject.SetActive(_isTyping);
                background.gameObject.SetActive(_isTyping);

                if(_isTyping)
                {
                    listener.Attach();
                }
                else
                {
                    listener.Deattach();
                }
            }
        }


        public tk2dTextMesh AddMessage(string message)
        {
            tk2dTextMesh newLine = ((GameObject)Instantiate(exampleLine.gameObject)).GetComponent<tk2dTextMesh>();

            newLine.text = message;
            newLine.ForceBuild();

            MessageContainer.position += new Vector3(0, newLine.GetEstimatedMeshBoundsForString(message).size.y, 0);
            newLine.transform.parent = MessageContainer;

            newLine.transform.position = exampleLine.transform.position;

            messages.Add(newLine);
            newLine.gameObject.SetActive(true);

            return newLine;
        }

        private void CompleteChatInput()
        {
            ChatPacket packet = new ChatPacket();

            packet.type = CurrentChatType;
            packet.text = inputTextField.text;

            ClientCommunicator.Instance.SendToServer(packet);

            inputTextField.text = "";
            inputTextField.ForceBuild();
        }

        private ChatPacket.ChatType _currentChatType = ChatPacket.ChatType.Public;
        public ChatPacket.ChatType CurrentChatType
        {
            get { return _currentChatType; }
            set
            {
                _currentChatType = value;

                Public.gameObject.SetActive(false);
                Private.gameObject.SetActive(false);
                Party.gameObject.SetActive(false);

                switch (value)
                {
                    case ChatPacket.ChatType.Public:
                        Public.gameObject.SetActive(true);
                        break;
                    case ChatPacket.ChatType.Private:
                        Private.gameObject.SetActive(true);
                        break;
                    case ChatPacket.ChatType.Party:
                        Party.gameObject.SetActive(true);
                        break;
                }
            }
        }

        internal void AddMessage(ChatPacket p)
        {
            if (p.type == ChatPacket.ChatType.GAME)
            {
                AddMessage(p.text);
            }
            if (p.type == ChatPacket.ChatType.Public)
            {
                AddMessage(p.text).color = Public.color;
            }
            if (p.type == ChatPacket.ChatType.Private)
            {
                AddMessage(p.text).color = Private.color;
            }
            if (p.type == ChatPacket.ChatType.Party)
            {
                AddMessage(p.text).color = Party.color;
            }
        }
	
        #region implemented abstract members of Interface
        protected override void OnStart()
        {
            CurrentChatType = ChatPacket.ChatType.Public;

            exampleLine.gameObject.SetActive(false);
            isTyping = false;
            transform.localPosition = HiddenPos;
        }

        protected override void OnUpdate()
        {
            PressDelay -= Time.deltaTime;
        }

        public void EnterPressed()
        {
            if (PressDelay > 0)
            {
                return;
            }

            isTyping = !isTyping;
            PressDelay = 0.3f;
            if (inputTextField.text.Length > 0)
            {
                CompleteChatInput();
                inputTextField.text = "";
            }
        }

        protected override void OnFixedUpdate()
        {
        }
        protected override void OnLateUpdate()
        {
        }
        protected override void OnVisibiltyChanged()
        {
        }

        private Vector3 HiddenPos = Vector3.zero + Vector3.down * 20f;
        
        public override void Hide()
        {
            CorotineManager.Instance.StartCoroutine(
                Ease.Vector(
                    transform.localPosition,
                    HiddenPos,
                    delegate(Vector3 vector3)
                    {
                        transform.localPosition = vector3;
                    },
                    delegate
                    {
                        gameObject.SetActive(false);
                    },
                    0.33f
                    )
                );

        }

        public override void Show()
        {
            gameObject.SetActive(true);
            CorotineManager.Instance.StartCoroutine(
                Ease.Vector(
                    transform.localPosition,
                    Vector3.zero,
                    delegate(Vector3 vector3)
                    {
                        transform.localPosition = vector3;
                    },
                    delegate
                    {
                    },
                    0.33f
                    )
                );
        }
        #endregion

        private class ChatKeyListener : KeyboardInput.KeyboardImputListener
        {
        
            public override void KeyWasPressed(char c)
            {
                string text = ChatPanel.I.inputTextField.text;
                if (c == "\b"[0])
                {
                    if (text.Length != 0)
                    {
                        text = text.Substring(0, text.Length - 1);
                    }
                }
                else if (c == "\r"[0])
                {
                    ChatPanel.I.EnterPressed();
                }
                else if ((int)c!=9 && (int)c!=27) //deal with a Mac only Unity bug where it returns a char for escape and tab
                {
                    text += c;
                }
                ChatPanel.I.inputTextField.text = text;
                ChatPanel.I.inputTextField.ForceBuild();

            }

            public override void ListenerWasDeclined()
            {
            }
        };

    }
}
