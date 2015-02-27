#if SERVER

using Random = UnityEngine.Random;

namespace Server.Model.Entities.Vegetation.HC
{
    public class Wheat : Plant
    {
        protected override int ModelID
        {
            get { return Random.Range(2,3); }
        }

        protected override bool CanSeed
        {
            get { return false; }
        }

        public override bool Collides
        {
            get { return false; }
        }

    }
}
#endif
