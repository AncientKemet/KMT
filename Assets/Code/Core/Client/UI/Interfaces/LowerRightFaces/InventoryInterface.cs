using Client.UI.Scripts;
using Code.Core.Client.UI.Controls.Items;
using Code.Libaries.UnityExtensions.Independent;
using UnityEngine;

namespace Code.Core.Client.UI.Interfaces.LowerRightFaces
{
    public class InventoryInterface : UIInterface<InventoryInterface>
    {

        public ItemInventory ItemInventory;

        protected override void Awake()
        {
            base.Awake();
            gameObject.SetActive(false);
        }

        public override void Hide()
        {
            CorotineManager.Instance.StartCoroutine(
                Ease.Vector(
                    transform.localPosition,
                    Vector3.zero + Vector3.down * 20f,
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
    }
}
