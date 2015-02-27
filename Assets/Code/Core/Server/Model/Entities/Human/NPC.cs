#if SERVER
using Server.Model.Extensions.UnitExts;
using UnityEngine;

namespace Server.Model.Entities.Human
{
    public class Npc : ServerUnit
    {

        private ServerUnit _followingServerUnit;

        public override void Awake()
        {
            base.Awake();
            name = "NPC ["+ID+"]";
            Movement.Running = true;
        }

        public override void Progress()
        {
            if (_followingServerUnit == null)
            {
                foreach (var unit in CurrentBranch.ObjectsVisible)
                {
                    if (unit is Player)
                        _followingServerUnit = unit as Player;
                }
            }
            else
            {
                Vector2 otherPos = _followingServerUnit.GetPosition();
                float distance = Vector2.Distance(GetPosition(), otherPos);
                if (distance > 5f)
                {
                    Movement.WalkTo(_followingServerUnit.GetExt<UnitMovement>().Position);
                }
            }
            base.Progress();
        }
    }
}

#endif
