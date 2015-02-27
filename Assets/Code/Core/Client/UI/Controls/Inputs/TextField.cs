using System;
using Code.Core.Client.Controls;
using Code.Core.Client.UI.Controls;
using UnityEngine;

namespace Client.UI.Controls.Inputs
{
    [RequireComponent(typeof(BoxCollider))]
    public class TextField : UIControl
    {
        [SerializeField]
        private tk2dTextMesh _textMesh;
        [SerializeField]
        private tk2dTextMesh _overridenTextMesh;

        public bool IsPassword = false;

        public event Action<string> OnFocusLost;

        private tk2dTextMesh _inputCursor;
        private string _text = "";
        private Bounds _bounds;
        private float _cursorBlinkTime = 0.5f;
        
        private Listener _listener;

        public bool HasFocus
        {
            get { return KeyboardInput.Instance.FullListener == _listener; }
        }

        public Bounds Bounds
        {
            get
            {
                return _bounds;
            }
        }

        public string Text
        {
            get { return _text; }
            set
            {
                _overridenTextMesh.gameObject.SetActive(string.IsNullOrEmpty(value));
                _text = value;
                if(IsPassword)
                    _textMesh.text = new string('*', value.Length);
                else
                    _textMesh.text = value;
                _bounds = _textMesh.renderer.bounds;

                if(_inputCursor != null)
                    _inputCursor.transform.position = string.IsNullOrEmpty(value) ? _textMesh.transform.position :  _bounds.center + new Vector3(_bounds.size.x / 2f, _bounds.size.y  / 4f, 0);
            }
        }

        public bool LoseFocusOnEnter = true;

        protected override void Start()
        {
            base.Start();

            _listener = new Listener(this);

            OnLeftClick += () =>
            {
                GainFocus();
            };
        }

        public void GainFocus()
        {
            KeyboardInput.Instance.FullListener = _listener;

            if (_inputCursor == null)
            {
                _inputCursor = ((GameObject) Instantiate(_textMesh.gameObject)).GetComponent<tk2dTextMesh>();
                _inputCursor.transform.parent = _textMesh.transform;
                _inputCursor.transform.localPosition = Vector3.zero;
                _inputCursor.text = "|";
            }

            if (_inputCursor != null)
                _inputCursor.gameObject.SetActive(true);

            _overridenTextMesh.gameObject.SetActive(false);
        }

        private void Update()
        {
            _cursorBlinkTime -= Time.deltaTime;
            if (_cursorBlinkTime < 0)
            {
                _cursorBlinkTime += 0.5f;
                if(HasFocus)
                if (_inputCursor != null)
                    _inputCursor.gameObject.SetActive(!_inputCursor.gameObject.activeSelf);
            }
        }

        private void OnDestroy()
        {
            _listener.Deattach();
        }

        public void LoseFocus()
        {
            if (this != null)
            {
                if (OnFocusLost != null)
                    OnFocusLost(_text);
                _overridenTextMesh.gameObject.SetActive(string.IsNullOrEmpty(Text));
                if (_inputCursor != null)
                    _inputCursor.gameObject.SetActive(false);
            }
        }

        public Action OnEnter;
    }

    public class Listener : KeyboardInput.KeyboardImputListener
    {
        private TextField _textField;

        public Listener(TextField textField)
        {
            this._textField = textField;
        }

        public override void KeyWasPressed(char c)
        {
            string text = _textField.Text;

            if (c == "\b"[0])
            {
                if (text.Length != 0)
                {
                    text = text.Substring(0, text.Length - 1);
                    _textField.Text = text;
                }
            }
            else if (c == "\r"[0])
            {
                if (_textField.OnEnter != null)
                    _textField.OnEnter();
                if (_textField.LoseFocusOnEnter)
                    Deattach();
            }
            else if ((int)c != 9 && (int)c != 27) //deal with a Mac only Unity bug where it returns a char for escape and tab
            {
                text += c;
                _textField.Text = text;
            }

            
        }

        public override void ListenerWasDeclined()
        {
            _textField.LoseFocus();
        }
    }
}
