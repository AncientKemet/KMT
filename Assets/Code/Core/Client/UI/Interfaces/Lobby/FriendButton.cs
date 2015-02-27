using Client.UI.Controls;
using UnityEngine;

namespace Client.UI.Interfaces.Lobby
{
    public class FriendButton : TextButton
    {

        [SerializeField]
        public tk2dSprite OnlineStatus;
        [SerializeField]
        public tk2dTextMesh LevelField;

        private bool _isOnline;

        public bool IsOnline
        {
            get { return _isOnline; }
            set
            {
                if (_isOnline != value || !OnlineStatus.gameObject.activeSelf)
                {
                    OnlineStatus.SetSprite("Friend-"+ (value ? "Online" : "Offline"));
                    OnlineStatus.gameObject.SetActive(true);
                    _isOnline = value;
                }
            }
        }

        protected override void Start()
        {
            base.Start();
            LevelField.text = "";
            OnlineStatus.gameObject.SetActive(false);
        }
    }
}
