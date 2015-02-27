#if SERVER
using System;
using System.Collections.Generic;
using Code.Libaries.Generic.Managers;
using Server.Model.Entities.Items;
using Server.Model.Extensions;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Server.Model.Entities.Vegetation.HC
{
    public class PumpkinPlant : Plant
    {
        protected override int ModelID
        {
            get { return 4; }
        }

        protected override bool CanSeed
        {
            get { return true; }
        }

        public override bool Collides
        {
            get { return false; }
        }

        private List<DroppedItem> _droppedItems = new List<DroppedItem>(); 

        public override void Awake()
        {
            base.Awake();
            //AddExt(new CollisionExt());
            
        }

        protected override void OnEnterWorld(World world)
        {
            base.OnEnterWorld(world);
            for (int i = 0; i < Random.Range(1, 2); i++)
            {
                DroppedItem item = CreateInstance<DroppedItem>();
                item.Item = ContentManager.I.Items[8];

                item.Movement.Teleport(Movement.Position + new Vector3(Random.Range(-1.5f, 1.5f), 0, Random.Range(-1.5f, 1.5f)));
                CurrentWorld.AddEntity(item);
            }
        }
    }
}
#endif
