using Code.Core.Shared.Content.Types.ItemExtensions;
using Code.Libaries.Generic.Managers;
#if SERVER
using Server.Model.Entities;
using Server.Model.Extensions.UnitExts;
using UnityEditor;
#endif
using Shared.Content.Types;

namespace Development.Libary.Spells.Codes
{
    public class SwapSpell : Spell
    {

        public EquipmentItem.Type ItemType;

        public int ItemIndex;

#if SERVER
        public override void OnStartCasting(ServerUnit unit)
        {
        }

        public override void OnFinishCasting(ServerUnit unit, float strenght)
        {
            UnitEquipment i = unit.GetExt<UnitEquipment>();
            if (i != null)
            {
                EquipmentItem item;
                if (ItemType == EquipmentItem.Type.MainHand)
                {
                    item = i.MainHand;
                    //Debug.Log("has: " + item.Item.InContentManagerIndex + " " + (item.Item.InContentManagerIndex + ItemIndex));
                    i.DestroyItem(item.EquipType);
                    i.EquipItem(ContentManager.I.Items[item.Item.InContentManagerIndex + ItemIndex]);
                }
            }
        }

        public override void OnStrenghtChanged(ServerUnit unit, float strenght)
        {
        }

        public override void CancelCasting(ServerUnit Unit)
        {
        }

        [MenuItem("Kemet/Create/Spell/Swap")]
        public static void CreateTest()
        {
            CreateSpell<SwapSpell>();
        }
#endif
    }
}
