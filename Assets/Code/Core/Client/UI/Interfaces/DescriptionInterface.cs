﻿using Client.UI.Scripts;
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

            _title.text = title;
            _title.ForceBuild();

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
            Vector3 mousePos = tk2dUIManager.Instance.camera.ScreenToViewportPoint(Input.mousePosition);

            transform.position = tk2dUIManager.Instance.camera.ViewportToWorldPoint(mousePos);
            //Update background sprie
            _mainSprite.RecalculateBounds();

            Vector3 vpCenter = tk2dUIManager.Instance.camera.WorldToViewportPoint(_mainSprite.Bounds.center);
            Vector3 vpSize = tk2dUIManager.Instance.camera.WorldToViewportPoint(_mainSprite.Bounds.size);
            Vector3 finalPos = mousePos;

            finalPos.x += - vpSize.x/2f;
            finalPos.z = 0;
            
            //Calculate final position
            /*if (mousePos.x > 0.5f)
            {
                if (mousePos.x + vpSize.x > 1.0f)
                {
                    finalPos.x -= vpSize.x;
                }
                if (mousePos.y - vpSize.y < 0)
                {
                    finalPos.y = vpSize.y;
                }
            }*/

            transform.position = tk2dUIManager.Instance.camera.ViewportToWorldPoint(finalPos);
            _mainSprite.RecalculateBounds();
        }

        public override void Hide()
        {
            gameObject.SetActive(false);
        }
    }
}