#if SERVER
using Server.Model.ContentHandling;
using Server.Model.Entities.Minerals.Configs;
using UnityEngine;

namespace Server.Model.Entities.Minerals.IMPL
{
    public class LimestoneMother : MotherMineral
    {
        public override MineralConfig Config
        {
            get { return MineralConfig.GetConfig<Limestone>(); }
        }

        protected override MineralBlock CreateChild(int id, int total)
        {
            float a = ((360f/total)*id*Mathf.PI/180f);
            float r = Display.Size/2f+0.5f;
            float x = r*Mathf.Cos(a);
            float z = r*Mathf.Sin(a);
            
            return ServerSpawnManager.Instance(CurrentWorld).Spawn<LimestoneBlock>(Movement.Position
                + new Vector3(x, 0, z));
        }
    }
}
#endif
