using System;
using System.Collections;
using System.Collections.Generic;
using Server.Model.Entities.Human;
using Shared.Content.Types;
using UnityEngine;

namespace Server.Model.Content.Spawns.NpcSpawns
{
    public class NpcShop : NpcSpawnExtension
    {
        private List<Player> _listeningPlayers = new List<Player>(); 

        private void SendRefresh()
        {
            foreach (var p in _listeningPlayers)
            {
                if (p != null)
                {
                    p.ClientUi.ProfileInterface.RefreshShop(Stock.AsReadOnly());
                }
            }
        }

        #region configuration
        public ShopType Shoptype;

        public List<NpcShopDeal> Deals;
        public List<Item.ItemInstance> Stock;

        public override void Apply(NPC n)
        {
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

            var inStock = Stock.Find(item => item.Item == itemInstance.Item);
            if (inStock != null)
            {
                inStock.Amount += itemInstance.Amount;
                SendRefresh();
            }
            else
            {
                Stock.Add(itemInstance);
                SendRefresh();
            }
            StartCoroutine(Restock(itemInstance, restockRate));
        }
        
        public enum ShopType
        {
            Trader_BuySale,
            Vendor_Buy,
            Vendor_BuySale
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
    }
}
