using System.Collections.Generic;
using Client.UI.Controls.Items;
using Client.UI.Scripts;
using Code.Core.Client.Controls;
using Code.Core.Client.UI.Controls;
using Code.Core.Client.UI.Controls.Items;
using Libaries.UnityExtensions.Independent;
using UnityEngine;

namespace Code.Core.Client.UI.Interfaces.LowerRightFaces
{
    public class InventoryInterface : UIInterface<InventoryInterface>
    {
        public enum State
        {
            Hidden,
            HalfVisible,
            Full
        }

        public Clickable Button;

        public List<ItemButton> Buttons123;

        public ItemInventory Inventory;
        
        private static ItemButton UseItemButton;

        public State CurrentState
        {
            get { return _state; }
            set
            {
                _state = value;
                StopAllCoroutines();

                if (value == State.HalfVisible)
                {
                    StartCoroutine(
                        Ease.Vector(
                            transform.localPosition,
                            Vector3.down * 7.355438f,
                            delegate(Vector3 vector3)
                            {
                                transform.localPosition = vector3;
                            },
                            null,
                            0.33f
                            )
                        );
                }else if (value == State.Full)
                {
                    StartCoroutine(
                        Ease.Vector(
                            transform.localPosition,
                            Vector3.zero,
                            delegate(Vector3 vector3)
                            {
                                transform.localPosition = vector3;
                            },
                            null,
                            0.33f
                            )
                        );
                }
                else
                {
                    StartCoroutine(
                        Ease.Vector(
                            transform.localPosition,
                            Vector3.down * 9.237873f,
                            delegate(Vector3 vector3)
                            {
                                transform.localPosition = vector3;
                            },
                            null,
                            0.33f
                            )
                        );
                }
            }
        }

        private State _state;

        protected override void Awake()
        {
            base.Awake();

            CurrentState = State.Hidden;

            Inventory.OnItemUpdate += (_itemButton, itemInstance) =>
            {
                if (itemInstance.Item != null)
                {
                    _itemButton.Button.AddAction(new RightClickAction("Drop"));
                    foreach (var action in itemInstance.Item.ActionsStrings)
                        _itemButton.Button.AddAction(new RightClickAction(action));
                    

                    _itemButton.Button.AddAction(new RightClickAction("Use", () =>
                    {
                        if (UseItemButton == null)
                        {
                            UseItemButton = _itemButton;
                        }
                        else
                        {
                            UseItemButton = null;
                        }
                    }));
                    
                    if (itemInstance.Item.EQ != null)
                        _itemButton.Button.AddAction(new RightClickAction("Equip"));
                }
                else
                {
                    _itemButton.Button.ClearAllActions();
                }
            };

            Button.OnLeftClick += () =>
            {
                if (_state == State.Full)
                    CurrentState = State.Hidden;
                else
                    CurrentState++;
                
            };
        }

        void OnGUI()
        {
            if (ItemDragManager.IndragButton == null && UseItemButton != null && UseItemButton.Item != null)
                GUI.DrawTexture(new Rect(Input.mousePosition.x, Screen.height - Input.mousePosition.y, 50,50), UseItemButton.Item.Icon);
        }

        protected virtual void Update()
        {
            if(_state >= State.HalfVisible)
            if (KeyboardInput.Instance.FullListener == null)
            {
                if (Input.GetKeyDown(KeyCode.Alpha1))
                {
                    Buttons123[0].Button.OnLeftClick();
                }
                if (Input.GetKeyDown(KeyCode.Alpha2))
                {
                    Buttons123[1].Button.OnLeftClick();
                }
                if (Input.GetKeyDown(KeyCode.Alpha3))
                {
                    Buttons123[2].Button.OnLeftClick();
                }
            }
        }


    }
}
