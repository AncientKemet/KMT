using System.Collections.Generic;
using Client.Net;
using Client.UI.Controls.Items;
using Client.UI.Scripts;
using Code.Core.Client.UI.Controls;
using Code.Core.Client.UI.Controls.Items;
using Code.Libaries.Generic.Managers;
using Libaries.Net.Packets.ForServer;
using Shared.Content.Types;
using UnityEngine;

namespace Client.UI.Interfaces.Crafting
{
    public class CraftingInterface : UIInterface<CraftingInterface>
    {
        public tk2dTextMesh _expGained, _selectedItemName, _amountLabel;

        public Clickable Plus, Minus, Custom, CraftButton;

        public ItemInventory RecipesInventory;
        public ItemButton SelectedItemButton;
        public List<ItemButton> RequiredItems;
        public List<ItemButton> SideProducts;
        public List<Clickable> RequiredItemConsumables;

        private ItemRecipe _selectedRecipe;
        private ushort _amount;

        private ushort Amount
        {
            get { return _amount; }
            set
            {
                _amount = (ushort) Mathf.Clamp(value,1, 999);
                _amountLabel.text = _amount + "x";
                string exp = "none";
                if (SelectedRecipe != null)
                {
                    exp = "";
                    foreach (var reward in SelectedRecipe.Rewards)
                    {
                        exp += "+" + (reward.Val * Amount) + " " + reward.Skill + " exp.";
                    }
                }
                _expGained.text = SelectedRecipe == null ? "-" : exp;
            }
        }

        public ItemRecipe SelectedRecipe
        {
            private get { return _selectedRecipe; }
            set
            {
                _selectedRecipe = value;
                SelectedItemButton.Item = value == null ? null : value.Result.Item;
                SelectedItemButton.Amount.text = value == null ? " " : value.Result.Amount <= 1 ? "" : "" + value.Result.Amount;
                _selectedItemName.text = value == null ? "-" : value.Result.Item.name;

                for (int i = 0; i < RequiredItems.Count; i++)
                {
                    if (value != null && value.ItemRequirements.Count > i)
                    {
                        RequiredItems[i].gameObject.SetActive(true);
                        RequiredItems[i].Item = value.ItemRequirements[i].Item.Item;
                        RequiredItems[i].Amount.text = value.ItemRequirements[i].Item.Amount <= 1
                            ? ""
                            : "" + value.ItemRequirements[i].Item.Amount;
                        RequiredItemConsumables[i].gameObject.SetActive(value.ItemRequirements[i].IsConsumed);
                    }
                    else
                    {
                        RequiredItems[i].gameObject.SetActive(false);
                        RequiredItemConsumables[i].gameObject.SetActive(false);
                    }
                }

                for (int i = 0; i < SideProducts.Count; i++)
                {
                    if (value != null && value.SideProducts.Count > i)
                    {
                        SideProducts[i].gameObject.SetActive(true);
                        SideProducts[i].Item = value.SideProducts[i].Item;
                        SideProducts[i].Amount.text = value.SideProducts[i].Amount <= 1
                            ? ""
                            : "" + value.SideProducts[i].Amount;
                    }
                    else
                    {
                        SideProducts[i].gameObject.SetActive(false);
                    }
                }

                string exp = "none";
                if (value != null)
                {
                    exp = "";
                    foreach (var reward in value.Rewards)
                    {
                        exp += "+" + (reward.Val * Amount)+" "+reward.Skill +" exp.";
                    }
                }
                _expGained.text = value == null ? "-" : exp;
            }
        }

        void Start()
        {
            Minus.OnLeftClick += () => Amount--;
            Plus.OnLeftClick += () => Amount++;
            CraftButton.OnLeftClick += () =>
            {
                if(SelectedRecipe != null)
                if (ClientCommunicator.Instance.WorldServerConnection != null)
                {
                    CraftingPacket p = new CraftingPacket();
                    p.Amount = Amount;
                    p.ItemRecipe = SelectedRecipe;
                    ClientCommunicator.Instance.WorldServerConnection.SendPacket(p);
                }
            };
        }

        private void OnEnable()
        {
            LoadRecipes(ContentManager.I.Recipes);
            SelectedRecipe = null;
        }

        private void LoadRecipes(List<ItemRecipe> recipes)
        {
            RecipesInventory.Width = recipes.Count >= 9 ? 9 : recipes.Count;
            RecipesInventory.Height = 1 + recipes.Count / 9;
            RecipesInventory.ForceRebuild();

            int i = 0;

            foreach (var recipe in recipes)
            {
                var b = RecipesInventory[i++];
                b.Item = recipe.Result.Item;
                b.Amount.text = recipe.Result.Amount <= 1 ? "" : "" + recipe.Result.Amount;
                b.Button.ClearAllActions();
                ItemRecipe recipe1 = recipe;
                b.Button.AddAction(new RightClickAction("Select", () => { SelectedRecipe = recipe1;
                                                                            Amount = 1;
                }));
            }
        }

    }
}
