using Client.UI.Interfaces;
using Code.Core.Client.UI;
using Code.Core.Client.UI.Controls;
using Code.Core.Client.UI.Scripts;
using Shared.Content.Types;
using UnityEngine;

namespace Client.UI.Controls.Items
{
    [RequireComponent(typeof(InterfaceButton))]

    public class ItemButton : MonoBehaviour 
    {
        

        private Item _item;
        [SerializeField]
        private tk2dSlicedSprite _background;
        private Color _originalColor, _onHoverColor, _onBeingDragColor, _onBeingDropepdColor;
        private GameObject ItemModel;
        private InterfaceButton _button;

        private float _timeDown = -1f;

        [SerializeField] private bool _canBeDragged = true;
        [SerializeField]
        private tk2dTextMesh _amount;
        [SerializeField]
        private Icon _icon;

        public bool CanBeDragged
        {
            get { return _canBeDragged; }
            private set { _canBeDragged = value; }
        }

        public InterfaceButton Button
        {
            get
            {
                if (_button == null)
                {
                    _button = GetComponent<InterfaceButton>();
                }
                return _button;
            }
        }

        private void Start()
        {
            if (_background != null)
            {
                _originalColor = _background.color;
                _onBeingDragColor = _background.color / 1.5f + Color.gray / 2f;
                _onBeingDropepdColor = _background.color / 1.5f + Color.yellow / 2f;
                _originalColor = _background.color;
                _onHoverColor = _originalColor / 2f;
                _onHoverColor.a = _originalColor.a;
            }

            //add drag and drops
            if (_canBeDragged)
            {
                Button.OnLeftDown += () =>
                {
                    _timeDown = Time.realtimeSinceStartup;
                };
            }

            Button.OnLeftUp += () =>
            {
                if (ItemDragManager.IndragButton != null)
                {
                    ItemDragManager.DragRelease(this);
                }
            };

            Button.OnMouseIn += () =>
            {
                Background.color = _onHoverColor;
                if(Item != null)
                DescriptionInterface.I.Show(Item.name,
                string.IsNullOrEmpty(Item.Subtitle) ? null : Item.Subtitle,
                Item.GetDescribtion());
            };

            Button.OnMouseOff += () =>
            {
                if (ItemDragManager.IndragButton != this)
                {
                    Background.color = _originalColor;
                }
                if(Item != null)
                DescriptionInterface.I.Hide();
            };
        }

        public void DragEnded()
        {
            _background.color = _originalColor;
        }

        private void FixedUpdate()
        {
            if (_timeDown > 0 && _canBeDragged)
            {
                if (Time.realtimeSinceStartup - 0.3f > _timeDown)
                {
                    _timeDown = -1f;
                    if (ItemDragManager.IndragButton == null)
                    {
                        ItemDragManager.DragBegin(this);
                        Background.color = _onBeingDragColor;
                    }
                }
            }
        }

        public Item Item
        {
            get { return _item; }
            set
            {
                if (ItemModel != null)
                {
                    Destroy(ItemModel);
                }

                _item = value;

                if (value == null)
                {
                    Icon.Texture = null;
                    return;
                }

                Icon.Texture = value.Icon;
                
                if (value != null)
                {
                    Button.MenuOn = true;
                    if (Button.InterfaceId != InterfaceType.ProfileInterface)
                    {
                        Button.ClearAllActions("Drop", "Unequip");
                        foreach (var action in value.ActionsStrings)
                        {
                            Button.AddAction(new RightClickAction(action));
                        }
                    }
                }
                else
                {
                    Button.MenuOn = false;
                }
            }
        }

        public tk2dSlicedSprite Background
        {
            get { return _background; }
            set
            {
                _background = value;
                if (value != null)
                {

                    _originalColor = _background.color;
                    _onBeingDragColor = _background.color / 1.5f + Color.gray / 2f;
                    _onBeingDropepdColor = _background.color / 1.5f + Color.yellow / 2f;
                    _originalColor = _background.color;
                    _onHoverColor = _originalColor / 2f;
                    _onHoverColor.a = _originalColor.a;
                }
            }
        }

        public tk2dTextMesh Amount
        {
            get { return _amount; }
            set { _amount = value; }
        }

        public Icon Icon
        {
            get { return _icon; }
            set { _icon = value; }
        }
    }
}
