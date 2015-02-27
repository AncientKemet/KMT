using System.Collections;
using System.Collections.Generic;
using Client.Net;
using Client.UI.Controls;
using Client.UI.Controls.Tool;
using Client.UI.Interfaces.Dialogues;
using Code.Core.Client.UI.Controls;
using Code.Libaries.Net.Packets.ForServer;
using Code.Libaries.UnityExtensions.Independent;
using Code.Scripts;
using Libaries.Net.Packets.Data;
using UnityEngine;

namespace Client.UI.Interfaces.Lobby
{
    public class FriendsList : MonoBehaviour
    {

        public tk2dTextMesh StatusLabel;

        [SerializeField]
        private Clickable AddFriendButton;
        [SerializeField]
        private TextButton FriendButtonPrefab;

        [SerializeField]
        private Slider FriendsListSlider;

        private bool visible = false;
        private float _refreshTimer = 1f;
        private float _friendStatusTimer = 1f;

        private List<FriendButton> _friendButtons = new List<FriendButton>();
        private int _friendRefreshIndex = 0;

        public void Show()
        {
            if (!visible)
            {
                visible = true;
                gameObject.SetActive(true);
            }
        }

        private void RefreshFriendsListData()
        {
            if (string.IsNullOrEmpty(LobbyInterface.I.DataServerKey))
            {
                LobbyInterface.I.OnDataServerKeyRecieved += (key) =>
                {
                    _requestFriendsListData();
                };
            }
            else
            {
                _requestFriendsListData();
            }
        }

        private void _requestFriendsListData()
        {
            StatusLabel.gameObject.SetActive(true);
            StatusLabel.text = "Loading friends list...";
            DataServerConnection.DataRequest request =
                new DataServerConnection.DataRequest("s/" + LobbyInterface.I.DataServerKey + "/friendsList",
                    (b, s) =>
                    {
                        if (b)
                        {
                            string[] friendsStrings = s.Split(","[0]);
                            StatusLabel.gameObject.SetActive(false);
                            foreach (var child in FriendsListSlider.ContentGameObject.GetComponentsInChildren<TextButton>())
                            {
                                if (child != FriendButtonPrefab)
                                    Destroy(child.gameObject);
                            }

                            _friendButtons.Clear();

                            float yOffset = 0;
                            foreach (var friendName in friendsStrings)
                            {
                                var newButton = ((GameObject)Instantiate(FriendButtonPrefab.gameObject)).GetComponent<TextButton>();
                                newButton.transform.parent = FriendButtonPrefab.transform.parent;
                                newButton.transform.localPosition = new Vector3(0, yOffset, -1);
                                newButton._textMesh.text = friendName;

                                string name1 = friendName;
                                newButton.AddAction(new RightClickAction("Send message", () =>
                                {
                                    LobbyChatBar.instance.GetLobbyChatPanel(name1).Opened = true;
                                    LobbyChatBar.instance.GetLobbyChatPanel(name1).TextField.GainFocus();
                                }));

                                newButton.gameObject.SetActive(true);
                                yOffset -= newButton._backGround.dimensions.y / 20f * newButton._backGround.scale.y;

                                _friendButtons.Add(newButton as FriendButton);
                            }
                        }
                        else
                        {
                            StatusLabel.text = "ERROR, friends list.";
                        }
                    });
            ClientCommunicator.Instance.DataServerConnection.RequestData(request);
        }

        private void RefreshNextFriendData()
        {
            if (_friendButtons.Count == 0)
                return;

            string friendName = _friendButtons[_friendRefreshIndex]._textMesh.text;
            int index = _friendRefreshIndex;

            //Debug.Log("refreshing: " + "u/" + friendName + "/isOnline");
            DataServerConnection.DataRequest request =
                new DataServerConnection.DataRequest("u/" + friendName + "/isOnline",
                    (b, s) =>
                    {
                        if (b)
                        {
                            _friendButtons[index].IsOnline = s == "1";
                        }
                    });
            ClientCommunicator.Instance.DataServerConnection.RequestData(request);

            _friendRefreshIndex++;
            if (_friendRefreshIndex > _friendButtons.Count - 1)
            {
                _friendRefreshIndex = 0;
            }
        }

        public void Hide()
        {
            if (visible)
            {
                visible = false;
                gameObject.SetActive(false);
            }
        }

        public void AddFriend()
        {
            InputText.Create("Friend username", s =>
            {
                ChatPacket packet = new ChatPacket();
                packet.type = ChatPacket.ChatType.AddFriend;
                packet.text = s;
                ClientCommunicator.Instance.LoginServerConnection.SendPacket(packet);
            });
        }

        void FixedUpdate()
        {
            _refreshTimer -= Time.fixedDeltaTime;
            if (_refreshTimer < 0)
            {
                _refreshTimer = 60;
                RefreshFriendsListData();
            }
            _friendStatusTimer -= Time.fixedDeltaTime;
            if (_friendStatusTimer < 0)
            {
                _friendStatusTimer = 1;
                RefreshNextFriendData();
            }
        }



        void Start()
        {
            Show();
            AddFriendButton.OnLeftClick += AddFriend;
        }

        public void ShowOrHide()
        {
            if (visible)
                Hide();
            else
                Show();

        }
    }
}
