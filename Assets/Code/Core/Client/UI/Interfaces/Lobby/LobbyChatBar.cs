using System.Collections.Generic;
using UnityEngine;

namespace Client.UI.Interfaces.Lobby
{
    public class LobbyChatBar : MonoBehaviour
    {
        [SerializeField]
        private LobbyChatPanel Prefab;

        [SerializeField] private tk2dUIItem _friendsListButton, _notificationsListButton;

        [SerializeField] private FriendsList _friendsList;

        private Vector3 CurrentOffset = new Vector3(0,0,-1);

        public static LobbyChatBar instance { get; private set; }

        private static Dictionary<string, LobbyChatPanel> ActivePanels = new Dictionary<string, LobbyChatPanel>(); 

        private void Start()
        {
            instance = this;
            _friendsListButton.OnClick += () => _friendsList.ShowOrHide();
            Prefab.gameObject.SetActive(false);
        }
        
        public LobbyChatPanel GetLobbyChatPanel(string username)
        {
            if (ActivePanels.ContainsKey(username))
            {
                return ActivePanels[username];
            }
            else
            {
                LobbyChatPanel newPanel = ((GameObject) Instantiate(Prefab.gameObject)).GetComponent<LobbyChatPanel>();
                newPanel.gameObject.SetActive(true);
                newPanel.transform.parent = Prefab.transform.parent;
                newPanel.transform.localPosition = CurrentOffset;

                newPanel._textMesh.text = username;

                CurrentOffset += new Vector3(16.22572f, 0, 0);

                ActivePanels.Add(username, newPanel);

                return newPanel;
            }
        }

        public void OnPanelWasDestroyed(LobbyChatPanel panel)
        {
            if(panel == Prefab)
                return;
            
            ActivePanels.Remove(panel._textMesh.text);

            foreach (var p in ActivePanels.Values)
            {
                if (p.transform.localPosition.x > panel.transform.localPosition.x)
                {
                    p.transform.localPosition -= new Vector3(16.22572f, 0, 0);
                }
            }
            CurrentOffset -= new Vector3(16.22572f, 0, 0);
            
        }
    }
}
