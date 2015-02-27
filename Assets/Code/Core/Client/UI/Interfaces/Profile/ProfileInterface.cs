using Client.UI.Scripts;
using Client.Units;
using Code.Core.Client.Units;
using Code.Core.Client.Units.Managed;
using Libaries.Net.Packets.ForClient;
using UnityEngine;

namespace Client.UI.Interfaces.Profile
{
    public class ProfileInterface : UIInterface<ProfileInterface>
    {

        [SerializeField]
        private tk2dTextMesh Title;

        [SerializeField] private Transform ObjectPoser;

        private PlayerUnit _unit;
        [SerializeField]
        private ProfileTab _currentTab;

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
                
                    _currentTab.transform.localPosition += new Vector3(0,0,2);
                    value.transform.localPosition += new Vector3(0, 0, -2);
                    _currentTab.ContentGameObject.SetActive(false);
                    _currentTab = value;
                    value.ContentGameObject.SetActive(true);
                
                
            }
        }

        public void OnPacket(ProfileInterfaceUpdatePacket p)
        {
            Debug.Log("recievieved profiled id: "+p.UnitID);
            Unit = UnitManager.Instance[p.UnitID];
        }

    }
}
