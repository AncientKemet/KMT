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
    public class Palm : Plant
    {
        protected override int ModelID
        {
            get { return Random.Range(5,8); }
        }

        protected override bool CanSeed
        {
            get { return true; }
        }

        public override bool Collides
        {
            get { return true; }
        }

        public override float LowSize
        {
            get { return 1f; }
        }

        public override float HighSize
        {
            get { return 2.5f; }
        }

        public override float MinGrassLevel
        {
            get { return 0.05f; }
        }
    }
}
#endif
