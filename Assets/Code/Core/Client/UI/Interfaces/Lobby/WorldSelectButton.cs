using Code.Core.Client.UI.Controls;
using UnityEngine;

namespace Client.UI.Interfaces.Lobby
{
    public class WorldSelectButton : UIControl
    {

        [SerializeField] private tk2dTextMesh _name, _type, _playersOnline, _latency;

        private Color _originalColor;
        private tk2dSlicedSprite _sprite;

        public Color Selected, Highlighted;

        public PlayPage Page;
        private int _latency1;
        private int _onlinePlayers;
        private string _type1;
        private string _name1;
        private Ping ping;
        private string _ipAdress;

        protected override void Start()
        {
            base.Start();
            _sprite = GetComponent<tk2dSlicedSprite>();
            OnLeftClick += () =>
            {
                if (Page.SelectedWorld != null)
                {
                    Page.SelectedWorld._sprite.color = _originalColor;
                }

                _sprite.color = Selected;
                Page.SelectedWorld = this;
            };
            OnMouseIn += () =>
            {
                if (Page.SelectedWorld != this)
                    _sprite.color = Highlighted;
            };
            OnMouseOff += () =>
            {
                if (Page.SelectedWorld != this)
                    _sprite.color = _originalColor;
            };
        }

        private void FixedUpdate()
        {
            if (ping != null)
            {
                Latency = ping.time;
            }
        }

        public string IpAdress
        {
            get { return _ipAdress; }
            set
            {
                _ipAdress = value;
                ping = new Ping(value);
            }
        }

        public string Name
        {
            get { return _name1; }
            set
            {
                _name1 = value;
                _name.text = value;
            }
        }

        public string Type
        {
            get { return _type1; }
            set
            {
                _type1 = value;
                _type.text = value;
            }
        }

        public int OnlinePlayers
        {
            get { return _onlinePlayers; }
            set
            {
                _onlinePlayers = value;
                _playersOnline.text = "" + value;
            }
        }

        public int Latency
        {
            get { return _latency1; }
            set
            {
                _latency1 = value;
                _latency.text = value + "ms";
            }
        }
    }
}
