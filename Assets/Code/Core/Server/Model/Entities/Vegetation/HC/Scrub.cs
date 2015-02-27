#if SERVER
using Random = UnityEngine.Random;

namespace Server.Model.Entities.Vegetation.HC
{
    public class Scrub : Plant
    {
        protected override int ModelID
        {
            get { return 8; }
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
            get { return 5; }
        }

    }
}
#endif
