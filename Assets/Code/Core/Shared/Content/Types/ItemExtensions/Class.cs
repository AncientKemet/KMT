namespace Shared.Content.Types.ItemExtensions
{
    public enum Class
    {
        NONE = 0,

        //Body
        Chestbody = 10,

        //Skirt
        Skirt = 20,

        //Helm
        Helm = 25,

        //Boots
        Sandals = 30,

        //Shield
        Shield = 40,

        //Melee
        Axe = 50, Pickaxe = 51, Spear = 52, Hammer = 53, Dagger = 54,
        //Range
        Bow = 100, Javelin = 101,
        //Mage
        Staff = 150,
    }
    public enum Role
    {
        NONE = 0, Melee_berserker = 1, Melee_tank = 2, Melee_assasin = 3
    }
}