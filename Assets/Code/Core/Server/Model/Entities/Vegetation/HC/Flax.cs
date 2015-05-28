#if SERVER

namespace Server.Model.Entities.Vegetation.HC
{
    public class Flax : Plant
    {
        protected override int ModelID
        {
            get { return 9; }
        }

        protected override bool CanSeed
        {
            get { return true; }
        }

        protected override int MaxSeeds
        {
            get { return 8; }
        }

        public override bool Collides
        {
            get { return false; }
        }

    }
}
#endif
