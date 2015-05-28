using Server.Model.Entities.Human;

namespace Server.Model.Content.Spawns.NpcSpawns
{
    public class NpcAnimation : NpcSpawnExtension
    {

        public string Stand = "Idle";
        public string Walk = "Walk";
        public string Run = "Run";

        public override void Apply(NPC n)
        {
            base.Apply(n);
            n.Anim.StandAnimation = Stand;
            n.Anim.WalkAnimation = Walk;
            n.Anim.RunAnimation = Run;
        }
    }
}
