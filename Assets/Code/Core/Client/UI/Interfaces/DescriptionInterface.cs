using System.Collections;
using Client.UI.Scripts;
using Code.Core.Client.UI.Scripts;
using Code.Libaries.UnityExtensions;
using UnityEngine;

namespace Client.UI.Interfaces
{
    public class DescriptionInterface : UIInterface<DescriptionInterface>
    {
        [SerializeField] private BoundsFittingSlicedSprite _mainSprite;
        [SerializeField] private tk2dTextMesh _title, _subtitle, _description;
        [SerializeField] private Icon _icon;
        [SerializeField] private GameObject _iconParent;

        public void Show(string title, string subtitle = null, string description = null, Texture2D icon = null)
        {
            gameObject.SetActive(true);
            bool enableTtitle = !string.IsNullOrEmpty(title);
            _title.gameObject.SetActive(enableTtitle);
            if (enableTtitle)
            {
                _title.text = title;
                _title.ForceBuild();
            }

            bool enableSubtitle = !string.IsNullOrEmpty(subtitle);
            _subtitle.gameObject.SetActive(enableSubtitle);
            if (enableSubtitle)
            {
                _subtitle.text = subtitle;
                _subtitle.ForceBuild();
            }

            bool enableDescription = !string.IsNullOrEmpty(description);
            _description.gameObject.SetActive(enableDescription);
            if (enableDescription)
            {
                _description.text = description;
                _description.ForceBuild();
            }

            bool enableIcon = icon != null;
            _iconParent.gameObject.SetActive(enableIcon);
            if (enableIcon)
            {
                _icon.Texture = icon;
            }

            FitViewport();
        }


        private void FitViewport()
        {
            //Convert background sprite size to viewport size
            Vector3 mousePos = tk2dUIManager.Instance.UICamera.ScreenToViewportPoint(Input.mousePosition);

            transform.position = tk2dUIManager.Instance.UICamera.ViewportToWorldPoint(mousePos);
            //Update background sprie
            _mainSprite.RecalculateBounds();

            Vector3 vpSize = tk2dUIManager.Instance.UICamera.WorldToViewportPoint(_mainSprite.Bounds.size) / 3f;
            Vector3 finalPos = mousePos;

            finalPos.z = 0;

            Debug.Log(vpSize);

            if (finalPos.x + vpSize.x > 1.0f)
            {
                finalPos.x = 1f - vpSize.x;
            }
            if (finalPos.y - vpSize.y < 0.0f)
            {
                finalPos.y = vpSize.y;
            }

            transform.position = tk2dUIManager.Instance.UICamera.ViewportToWorldPoint(finalPos);
        }
        
        public override void Hide()
        {
            gameObject.SetActive(false);
        }
    }
}
