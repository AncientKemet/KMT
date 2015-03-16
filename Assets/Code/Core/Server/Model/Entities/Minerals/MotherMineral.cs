#if SERVER
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Server.Model.Entities.Minerals
{
    public abstract class MotherMineral : Mineral
    {
        protected abstract MineralBlock CreateChild(int id, int total);

        public override void Awake()
        {
            base.Awake();

            Display.Size = Random.Range(2, 3);

            Display.ModelID = Config.MotherModel;

            Combat.OnDeath += dictionary =>
            {
                Combat.Revive(100);

                var sortedDict = from entry in dictionary orderby entry.Value ascending select entry;

                int counter = 0;

                ServerUnit primary = null, secondary = null, third = null;

                foreach (KeyValuePair<ServerUnit, float> pair in sortedDict)
                {
                    if(counter == 3)
                        break;

                    if (counter == 0)
                    {
                        primary = pair.Key;
                    }
                    if (counter == 1)
                    {
                        secondary = pair.Key;
                    }
                    if (counter == 2)
                    {
                        third = pair.Key;
                    }

                    counter++;
                 }
                for (int i = 0; i < 3; i++)
                {
                    var block = CreateChild(i, 3);
                    if (i == 0) block.AccessOwnership.Owner = primary;
                    if (i == 1) block.AccessOwnership.Owner = secondary ?? primary;
                    if (i == 2) block.AccessOwnership.Owner = third ?? primary;
                }
            };
        }
    }
}
#endif
