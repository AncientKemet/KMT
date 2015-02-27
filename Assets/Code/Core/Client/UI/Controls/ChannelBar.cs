using System;
using System.Collections.Generic;
using Code.Libaries.UnityExtensions.Independent;
using UnityEngine;

namespace Code.Core.Client.UI.Controls
{
    public class ChannelBar : UIControl
    {
        [SerializeField] private bool _diableOnStart = true;

        [SerializeField]
        private float _progress, _targetProgress = 1f;

        private float MaxWidth = 0;

        public Color LowColor, HighColor;

        public float Progress
        {
            get { return _progress; }
            set { _targetProgress = Mathf.Clamp01(value); }
        }

        [SerializeField] private tk2dSlicedSprite _backSlicedSprite, _frontSlicedSprite;

        protected override void Start()
        {
            if (_diableOnStart)
                gameObject.SetActive(false);
            MaxWidth = _frontSlicedSprite.dimensions.x;
        }

        private void Update()
        {
            _progress = Mathf.Clamp01(_progress);
            if (Math.Abs(_progress - _targetProgress) > 0.001f)
            {
                _progress += (_targetProgress - _progress)*Time.deltaTime*30f;

                _frontSlicedSprite.dimensions = new Vector2(MaxWidth * Progress, _frontSlicedSprite.dimensions.y);
                _frontSlicedSprite.color = LowColor*(1f - Progress) + HighColor * Progress;

                _frontSlicedSprite.ForceBuild();
            }
        }

        public override void OnSetData(List<float> data)
        {
            _targetProgress = data[0];
        }

        public override void Hide()
        {
            gameObject.SetActive(false);
            _progress = 0;
            _targetProgress = 0;
            /*CorotineManager.Instance.StartCoroutine(
                Ease.Vector(
                    transform.localScale,
                    Vector3.zero,
                    delegate(Vector3 vector3)
                    {
                        transform.localScale = vector3;
                    },
                    delegate
                    {
                        gameObject.SetActive(false);
                    },
                    0.2f
                    )
                );*/

        }

        public override void Show()
        {
            gameObject.SetActive(true);
            _progress = 0;
            _targetProgress = 0;
            _frontSlicedSprite.dimensions = new Vector2(MaxWidth * Progress, _frontSlicedSprite.dimensions.y);
            _frontSlicedSprite.color = LowColor * (1f - Progress) + HighColor * Progress;

            _frontSlicedSprite.ForceBuild();
            /*CorotineManager.Instance.StartCoroutine(
                Ease.Vector(
                    transform.localScale,
                    Vector3.one,
                    delegate(Vector3 vector3)
                    {
                        transform.localScale = vector3;
                        _progress = 0;
                    },
                    delegate
                    {
                    },
                    0.2f
                    )
                );*/
        }

    }
}
