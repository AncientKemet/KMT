using Server.Model.Entities.Human;

namespace Server.Model.Extensions.PlayerExtensions
{
    public class PlayerCrafting : EntityExtension
    {
        private Player player;

        private int _useIndex = -1;

        public override void Progress(float time)
        {
        }

        protected override void OnExtensionWasAdded()
        {
            base.OnExtensionWasAdded();
            player = entity as Player;
        }
    }
}
