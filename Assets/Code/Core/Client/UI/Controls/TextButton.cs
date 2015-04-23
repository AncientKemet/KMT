using Code.Core.Client.UI.Controls;
using UnityEngine;

namespace Client.UI.Controls
{
    public class TextButton : UIControl
    {
        [SerializeField] 
        public Color HoverColor;

        [SerializeField]
        internal tk2dTextMesh _textMesh;
        [SerializeField] internal tk2dSlicedSprite _backGround;

        private string _text;
        private Color _normalColor;

        public string Text 
        {
            get { return _text; }
            set
            {
                _text = value;
                _textMesh.text = value ?? "";
                _textMesh.ForceBuild();
                _backGround.dimensions = new Vector2(Width * 20, _backGround.dimensions.y);
                _backGround.ForceBuild();
            } 
        }

        public float Width
        {
            get { return _textMesh.GetComponent<Renderer>().bounds.size.x; }
            set
            {
                _backGround.dimensions = new Vector2(value * 20, _backGround.dimensions.y);
                _backGround.ForceBuild();
            }
        }

        protected override void Start()
        {
            base.Start();
            if (_backGround == null)
            {
                _backGround = GetComponent<tk2dSlicedSprite>();
            }
            if (_backGround == null)
            {
                _backGround = GetComponentInChildren<tk2dSlicedSprite>();
            }
            _normalColor = _backGround.color;

            OnMouseIn += () =>
            {
                _backGround.color = HoverColor;
                _backGround.ForceBuild();
            };

            OnMouseOff += () =>
            {
                _backGround.color = _normalColor;
                _backGround.ForceBuild();
            };
        }
    }
}
