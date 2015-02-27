using Client.Units;
using Code.Core.Client.Units;
using UnityEngine;

namespace Code.Core.Client.UI.Interfaces
{
    public class Unit2D : MonoBehaviour
    {
        public PlayerUnit PlayerUnit;

        private void Start()
        {
            transform.parent = null;
        }

        private void LateUpdate()
        {
            if (PlayerUnit != null)
            {
                Vector3 pos = tk2dUIManager.Instance.UICamera.ScreenToWorldPoint(Camera.main.WorldToScreenPoint(PlayerUnit.transform.position));
                pos.z = -10;
                transform.position = pos;
            }
        }

    }
}
