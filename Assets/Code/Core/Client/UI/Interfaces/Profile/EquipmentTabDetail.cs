using UnityEngine;

namespace Client.UI.Interfaces.Profile
{
    [RequireComponent(typeof(tk2dUIItem))]
    public class EquipmentTabDetail : MonoBehaviour
    {
        public EquipmentTab EquipmentTab;
        [SerializeField]
        private tk2dTextMesh TextMesh;
        public Transform ChildTab;

        private tk2dUIItem Item;

        void Start ()
        {
            Item = GetComponent<tk2dUIItem>();
            Item.OnClick += () => EquipmentTab.CurrentDetail = this;
        }
	
    }
}
