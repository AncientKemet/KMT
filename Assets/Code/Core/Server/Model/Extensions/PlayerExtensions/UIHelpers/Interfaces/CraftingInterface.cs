using Code.Core.Client.UI;
using Libaries.Net.Packets.ForServer;
using Server.Model.Extensions.UnitExts;
using Shared.Content.Types;

namespace Server.Model.Extensions.PlayerExtensions.UIHelpers.Interfaces
{
    public class CraftingInterface : AInterface {

        private UnitInventory _inventory;

        // Use this for initialization
        public CraftingInterface(ClientUI ui) : base(ui)
        {
        }

        public void Open(UnitInventory inventory)
        {
            _inventory = inventory;
            if (!Opened)
                Opened = true;
        }
        
        public override void OnEvent(string action, int controlId)
        { }

        public void OnCraftingPacket(CraftingPacket packet)
        {
            if (_inventory != null)
            {
                Craft(packet.ItemRecipe, packet.Amount);
            }
            else
            {
                player.SendGameMessage("CraftingInteraface._inventory is null.");
            }
        }

        private void Craft(ItemRecipe itemRecipe, ushort amount)
        {
            if(amount <= 0)
                return;
            
            bool canCraft = true;

            foreach (var requirement in itemRecipe.Requirements)
                if (player.Levels.MeetsRequirement(requirement))
                {
                    player.SendGameMessage("This recipe requires: " + requirement.Skill + " Lvl." +
                                           requirement.Val + ".");
                    canCraft = false;
                }

            foreach (var requirement in itemRecipe.ItemRequirements)
                if (!_inventory.HasItem(requirement.Item))
                {
                    player.SendGameMessage("This recipe requires item: " + requirement.Item.Item.name + " x " +
                                           requirement.Item.Amount + ".");
                    canCraft = false;
                }

            Opened = false;

            if (canCraft)
            {
                if (player.Spells.CurrentCastingSpell != null)
                {
                    player.SendGameMessage("You're too busy right now.");
                    return;
                }
                player.Spells.StartCrafting(itemRecipe, () =>
                {
                    //We have to check if the crafter still has the items so canCraft2
                    bool canCraft2 = true;
                    int _willHaveFreeSpace = _inventory.GetSpace();

                    foreach (var requirement in itemRecipe.ItemRequirements)
                        if (!_inventory.HasItem(requirement.Item))
                        {
                            player.SendGameMessage("This recipe requires item: " + requirement.Item.Item.name + " x" +
                                                   requirement.Item.Amount + ".");
                            canCraft2 = false;
                        }
                        else
                        {
                            _willHaveFreeSpace += _inventory.SpaceFreedAfterRemoving(requirement.Item);
                        }

                    if (_willHaveFreeSpace < _inventory.SpaceRequiredFor(itemRecipe.Result))
                    {
                        player.SendGameMessage("Your inventory needs more space.");
                        canCraft2 = false;
                    }

                    if (canCraft2)
                    {

                        foreach (var requirement in itemRecipe.ItemRequirements)
                        {
                            if (!_inventory.RemoveItem(requirement.Item))
                            {
                                player.SendGameMessage("Couldnt remove item : " + requirement.Item);
                            }
                        }
                        _inventory.AddItem(itemRecipe.Result);

                        if (amount > 1)
                        {
                            player.SendGameMessage("We can craft so retry");
                            Craft(itemRecipe, (ushort) (amount - 1));
                        }
                    }

                });
            }
        }


        public override InterfaceType GetInterfaceType
        {
            get { return InterfaceType.Crafting; }
        }
    }
}
