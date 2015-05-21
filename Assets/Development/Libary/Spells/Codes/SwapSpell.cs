using Code.Core.Shared.Content.Types.ItemExtensions;
using Code.Libaries.Generic.Managers;
using Server.Model.Content;
using Server.Model.Entities.Items;
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

                    DroppedItem droppedItem = ServerMonoBehaviour.CreateInstance<DroppedItem>();

                    droppedItem.Movement.Teleport(unit.Movement.Position);
                    droppedItem.Item = new Item.ItemInstance(ContentManager.I.Items[item.Item.InContentManagerIndex + ItemIndex]);
                    unit.CurrentWorld.AddEntity(droppedItem);

                    i.EquipItem(droppedItem);
                }
            }
        }

        public override void OnStrenghtChanged(ServerUnit unit, float strenght)
        {
        }

        public override void CancelCasting(ServerUnit unit)
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
