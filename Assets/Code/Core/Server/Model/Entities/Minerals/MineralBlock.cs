#if SERVER
using System.Collections.Generic;
using System.Linq;
using Server.Model.Entities.Items;
using Server.Model.Extensions.UnitExts;

namespace Server.Model.Entities.Minerals
{
    public abstract class MineralBlock : Mineral
    {

        public UnitOwnership Ownership { get; private set; }

        public override void Awake()
        {
            AddExt(Ownership = new UnitOwnership());
            base.Awake();
            
            Display.ModelID = Config.ChildModel;

            Combat.OnHit += delegate(float newHealth)
            {
                if (newHealth < 66f && newHealth > 33f)
                {
                    DroppedItem item = DroppedItem.Spawn(Config.BrickItemId, Movement.Position, CurrentWorld);
                    if (Ownership.Owner != null)
                    {
                        item.AccessList.Add(Ownership.Owner);
                        item.AccessType = DroppedItem.DroppedItemAccessType.List;
                        item.AccessDelay = 60f;
                    }
                    Display.Destroy = true;
                }
            };
            
            Combat.OnDeath += dictionary =>
            {
                Display.Destroy = true;
                for (int i = 0; i < 3; i++)
                {
                    DroppedItem item = DroppedItem.Spawn(Config.BrokenItemId, Movement.Position, CurrentWorld);
                    if (Ownership.Owner != null)
                    {
                        item.AccessList.Add(Ownership.Owner);
                        item.AccessType = DroppedItem.DroppedItemAccessType.List;
                        item.AccessDelay = 60f;
                    }
                }
            };
         }

    }
}
#endif
