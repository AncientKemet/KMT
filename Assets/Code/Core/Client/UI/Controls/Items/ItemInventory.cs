using System;
using System.Collections.Generic;
using Client.UI.Controls.Items;
using Code.Core.Client.UI.Scripts;
using Code.Libaries.Generic.Managers;
using Shared.Content.Types;
using UnityEngine;

namespace Code.Core.Client.UI.Controls.Items
{
    [AddComponentMenu("Kemet/UI/Item Inventory")]
    [ExecuteInEditMode]
    public class ItemInventory : UIControl
    {
        [Range(1, 10)]
        public int Width = 1;
        [Range(1, 10)]
        public int Height = 1;

        public InterfaceAnchor Anchor = InterfaceAnchor.UpperCenter;

        private bool _rebuild = false;

        [HideInInspector]
        [SerializeField] private List<ItemButton> buttons = new List<ItemButton>();

        public void ForceRebuild()
        {
            Build();
        }

        public void SetItem(int x, int y, int itemId, int amount = 1)
        {
            var button = buttons[x + y * Width];
            var item = itemId == -1 ? null : ContentManager.I.Items[itemId];
            button.Item = item;
            button.Amount.text = amount <= 1 ? "" : "" + amount;

            if(OnItemUpdate != null)
            OnItemUpdate(button, new Item.ItemInstance(item, amount));
        }

        public void SetItem(int x, int y, Item item, int amount = 1)
        {
            SetItem(x,y, item.InContentManagerIndex, amount);
        }

        private void Build()
        {
            if (UIContentManager.I == null || UIContentManager.I.ItemButtonBackGround == null || UIContentManager.I.ItemButtonBackGround.GetComponent<Renderer>() == null)
            {
                Debug.LogError("Error");
                return;
            }

            Vector3 step = Vector3.zero;

            for (int i = 0; i < buttons.Count; i++)
            {
                var itemButton = buttons[i];
                if(itemButton == null) continue;
#if UNITY_EDITOR
                DestroyImmediate(itemButton.gameObject);
#else
                Destroy(itemButton.gameObject);
#endif
            }

            buttons = new List<ItemButton>(Width * Height);

            int ax = 0;
            int ay = 0;
            switch (Anchor)
            {
                    case InterfaceAnchor.LowerCenter:
                    ay = Height;
                    break;
                    case InterfaceAnchor.LowerLeft:
                    ax = Width / 2;
                    ay = Height;
                    break;
                    case InterfaceAnchor.LowerRight:
                    ax = -Width / 2;
                    ay = Height;
                    break;
                    case InterfaceAnchor.MiddleCenter:
                    ax = 0;
                    ay = Height / 2;
                    break;
                    case InterfaceAnchor.MiddleLeft:
                    ax = Width / 2;
                    ay = Height / 2;
                    break;
                    case InterfaceAnchor.MiddleRight:
                    ax = -Width / 2;
                    ay = Height / 2;
                    break;
                    case InterfaceAnchor.UpperCenter:
                    ax = 0;
                    ay = 0;
                    break;
                    case InterfaceAnchor.UpperLeft:
                    ax = Width/2;
                    ay = 0;
                    break;
                    case InterfaceAnchor.UpperRight:
                    ax = -Width/2;
                    ay = 0;
                    break;
            }
            
            for (int y = 0; y < Height; y++)
            {
                for (int x = 0; x < Width; x++)
                {
                    ItemButton button = new GameObject("itemButton").AddComponent<ItemButton>();

                    if (button.Background == null)
                    {
                        tk2dSlicedSprite buttonBackGround =
                            ((GameObject) Instantiate(UIContentManager.I.ItemButtonBackGround.gameObject))
                                .GetComponent<tk2dSlicedSprite>();

                        button.Background = buttonBackGround;
                        button.Icon = buttonBackGround.GetComponentInChildren<Icon>();
                        button.Amount = buttonBackGround.GetComponentInChildren<tk2dTextMesh>();

                        step = buttonBackGround.GetComponent<Renderer>().bounds.size;

                        buttonBackGround.transform.parent = button.transform;
                        buttonBackGround.transform.localPosition = new Vector3(0, 0, 1);

                        buttonBackGround.gameObject.layer = gameObject.layer;
                    }

                    button.Button.Index = Index+1 + x + y*Width;
                    button.Button.InterfaceId = InterfaceId;

                    button.gameObject.layer = gameObject.layer;

                    button.transform.parent = transform;
                    button.transform.localPosition = new Vector3((-Width+1) / 2f * step.x + (ax + x) * step.x, (-ay-y) * step.y, -1);
                    button.transform.localScale = Vector3.one;

                    (button.GetComponent<Collider>() as BoxCollider).size = step;

                    buttons.Add(button);
                }
            }
        }

        public Action<ItemButton, Item.ItemInstance> OnItemUpdate;

    }
}
