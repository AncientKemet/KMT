#if SERVER
namespace Server.Model.Entities.Minerals.Configs
{
    public class Limestone : MineralConfig
    {
        public override int MotherModel
        {
            get { return 101; }
        }

        public override int ChildModel
        {
            get { return 100; }
        }

        public override int BrickItemId
        {
            get { return 10; }
        }

        public override int BrokenItemId
        {
            get { return 11; }
        }

        public override float Resistance
        {
            get { return 50; }
        }
    }
}
#endif
