#if SERVER
using Random = UnityEngine.Random;

namespace Server.Model.Entities.Vegetation.HC
{
    public class HighGrassJungle : Plant
    {
        protected override int ModelID
        {
            get { return Random.Range(10,16); }
        }

        protected override bool CanSeed
        {
            get { return true; }
        }

        public override bool Collides
        {
            get { return false; }
        }

        protected override int MaxSeeds
        {
            get { return 50; }
        }

        protected override int MinSeeds
        {
            get { return 30; }
        }

        protected override float MaxSeedRange
        {
            get { return Size *1.5f; }
        }

        public override float LowSize
        {
            get { return 1.5f; }
        }

        public override float HighSize
        {
            get { return 3f; }
        }

        public override float MinGrassLevel
        {
            get { return 0.5f; }
        }
    }
}
#endif
