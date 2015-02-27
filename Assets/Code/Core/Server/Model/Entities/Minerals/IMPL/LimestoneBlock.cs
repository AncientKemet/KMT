#if SERVER
using Server.Model.Entities.Minerals.Configs;

namespace Server.Model.Entities.Minerals.IMPL
{
    public class LimestoneBlock : MineralBlock
    {
        public override MineralConfig Config
        {
            get { return MineralConfig.GetConfig<Limestone>(); }
        }
    }
}
#endif
