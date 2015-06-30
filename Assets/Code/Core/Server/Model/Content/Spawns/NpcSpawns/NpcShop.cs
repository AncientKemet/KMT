using System;
using System.Collections;
using System.Collections.Generic;
using Libaries.Net.Packets.ForClient;
using Server.Model.Entities.Human;
using Shared.Content.Types;
using UnityEngine;

namespace Server.Model.Content.Spawns.NpcSpawns
{
    public class NpcShop : NpcSpawnExtension
    {
        private List<Player> _listeningPlayers = new List<Player>();

        private void _onShopUpdate(int index)
        {
            Item.ItemInstance it = Stock[index];

            ShopUpdatePacket packet = new ShopUpdatePacket();
            packet.Index = index;
            packet.Instance = it;
            packet.UnitId = Npc.ID;

            foreach (var p in _listeningPlayers)
            {
                if (p != null)
                {
                    p.Client.ConnectionHandler.SendPacket(packet);
                }
            }
        }

        #region configuration
        public ShopType Shoptype;
        public ShopPayment Payment;

        public List<NpcShopDeal> Deals;
        public List<Item.ItemInstance> Stock;

        public override void Apply(NPC n)
        {
            base.Apply(n);

            //add actions
            n.Details.AddAction("Trade");

            //initialize stock
            Stock = new List<Item.ItemInstance>();

            foreach (var deal in Deals)
            {
                if (deal.RestockRate > 1)
                {
                    Stock.Add(new Item.ItemInstance(deal.item, deal.MaxStock));

                    StartCoroutine(Restock(new Item.ItemInstance(deal.item, 1), deal.RestockRate));
                }
            }
        }

        private IEnumerator Restock(Item.ItemInstance itemInstance, int restockRate)
        {
            yield return new WaitForSeconds(restockRate);

            Add(itemInstance);

            StartCoroutine(Restock(new Item.ItemInstance(itemInstance.Item, 1), restockRate));
        }
        
        public enum ShopType
        {
            Trader,
            Vendor,
        }

        public enum ShopPayment
        {
            Copper,Bronze,MildBronze,Tin,Silver
        }

        [Serializable]
        public class NpcShopDeal
        {
            public Item item;

            public int MaxStock = 1;

            /// <summary>
            /// In seconds.
            /// 
            /// -1 means it doesnt restock at all.
            /// </summary>
            public int RestockRate = 60;

            /// <summary>
            /// -1  stands for use Item value.
            /// </summary>
            public int Value = -1;
        }
        #endregion

        private void Add(Item.ItemInstance itemInstance)
        {
            var inStock = Stock.Find(item => item.Item == itemInstance.Item);
            if (inStock != null)
            {
                inStock.Amount += itemInstance.Amount;
                _onShopUpdate(Stock.IndexOf(inStock));
            }
            else
            {
                Stock.Add(itemInstance);
                _onShopUpdate(Stock.IndexOf(itemInstance));
            }
        }

        private int Remove(Item.ItemInstance itemInstance)
        {
            var inStock = Stock.Find(item => item.Item == itemInstance.Item);
            if (inStock != null)
            {
                int _oldAmount = inStock.Amount;
                inStock.Amount -= itemInstance.Amount;
                if (inStock.Amount < 0)
                {
                    if (this.Shoptype == ShopType.Vendor)
                        inStock.Amount = 0;
                    else
                        Stock.Remove(inStock);
                }
                _onShopUpdate(Stock.IndexOf(inStock));
                return _oldAmount - Mathf.Clamp(_oldAmount - itemInstance.Amount, 0, int.MaxValue);
            }
            return 0;
        }
        /*
        public int Purchase(Item.ItemInstance purchaseRequest, Player p)
        {
            int amountOfPurchasedItems = 0;

            //fix amount by space
            {
                int freeSpace = p.Inventory.AmountOfItemsPossibleToBePutIn(purchaseRequest);
                purchaseRequest.Amount = Mathf.Min(purchaseRequest.Amount, freeSpace);
            }

            //fix amount by what is left
            {
                int amountOfItemsInStock = Stock.Find(instance => instance.Item == purchaseRequest.Item).Amount;
                purchaseRequest.Amount = Mathf.Min(purchaseRequest.Amount, amountOfItemsInStock);
            }

            //fix amount by value
            {
                int coinsIHave = p.Inventory.GetCoinValue();
                int amountOfItemsInStock = Stock.Find(instance => instance.Item == purchaseRequest.Item).Amount;
                int totalStockValue = (int) ((purchaseRequest.Amount - 100)*purchaseRequest.Item.Value * 0.666666f);

                int maxPurchasable = 0;

                if (amountOfItemsInStock > 100)
                {
                    //my part
                    totalStockValue += (int) ((purchaseRequest.Amount - 100)*purchaseRequest.Item.Value * 0.666666f);
                    if (totalStockValue > coinsIHave)
                    {
                        maxPurchasable = (int)(coinsIHave / purchaseRequest.Item.Value * 0.666666f);
                    }
                    else
                    {
                        //I have enough coins to buy out whole stock
                        maxPurchasable = purchaseRequest.Amount - 100;
                    }

                    
                    int amountOfCurrenlyPurhcasing = Mathf.Min(purchaseRequest.Amount, maxPurchasable);
                    
                    //Remove coins and add items
                    p.Inventory.RemoveCoins((int)(amountOfCurrenlyPurhcasing * coinsIHave / purchaseRequest.Item.Value * 0.666666f));
                    p.Inventory.AddItem(purchaseRequest.Item, amountOfCurrenlyPurhcasing);
                    
                    //Repeat purchase request without amount of we already have purchased
                    amountOfPurchasedItems += amountOfCurrenlyPurhcasing;
                    amountOfPurchasedItems += Purchase(new Item.ItemInstance(purchaseRequest.Item, purchaseRequest.Amount - amountOfCurrenlyPurhcasing), p);
                }
                else
                {
                    //Your part
                    int changingtotalcost = -(33/200)*(Mathf.Pow((amountOfItemsInStock - purchaseRequest.Amount), 2)
                                                       - 600*(amountOfItemsInStock
                                                              - purchaseRequest.Amount)
                                                       - (amountOfItemsInStock - 600)*amountOfItemsInStock)*
                                            purchaseRequest.Item.Value/100;
                }

            }

            return amountOfPurchasedItems;
        }*/

        public void AttachPlayer(Player player)
        {
            _listeningPlayers.Add(player);
        }

        public void DeattachPlayer(Player player)
        {
            _listeningPlayers.Remove(player);
        }

        public void SendFullStockTo(Player player)
        {
            for (int i = 0; i < Stock.Count; i++)
            {
                Item.ItemInstance it = Stock[i];

                ShopUpdatePacket packet = new ShopUpdatePacket();
                packet.Index = i;
                packet.Instance = it;
                packet.UnitId = Npc.ID;

                player.Client.ConnectionHandler.SendPacket(packet);
            }
        }
    }
}
