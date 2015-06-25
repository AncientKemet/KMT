#if SERVER
using System.Collections.Generic;
using Code.Core.Shared.NET;
using Server.Model.Entities.Human;
using UnityEngine;

namespace Server.Model.Extensions.PlayerExtensions
{
    public class PlayerInput : EntityExtension
    {
        public Player Player { get; private set; }

        private List<PacketEnums.INPUT_TYPES> inputs = new List<PacketEnums.INPUT_TYPES>();

        public override void Progress(float time)
        {
            foreach (PacketEnums.INPUT_TYPES input in inputs)
            {
                if (input == PacketEnums.INPUT_TYPES.ToogleRun)
                {
                    Player.Movement.Running = !Player.Movement.Running;
                    continue;
                }
                if (input == PacketEnums.INPUT_TYPES.StopWalk)
                {
                    Player.Movement.DiscardPath();
                    continue;
                }
                if (input == PacketEnums.INPUT_TYPES.Jump)
                {
                    Player.Movement.Jump();
                    continue;
                }

                //we have no idea how to react to this
                Debug.LogError("Unknown input type: "+input);
            }
            inputs.Clear();
        }
        
        protected override void OnExtensionWasAdded()
        {
            base.OnExtensionWasAdded();
            Player = entity as Player;
        }

        public void AddInput(PacketEnums.INPUT_TYPES type)
        {
            inputs.Add(type);
        }
    }
}
#endif
