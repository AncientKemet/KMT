#if SERVER
using Server.Model.Entities;
using Server.Model.Entities.Animals;
using UnityEngine;

namespace Server.Model.Extensions.AnimalExtensions
{
    public class AnimalMovement : EntityExtension
    {
        public Animal Animal { get; private set; }
        public int tick = 1;
        public int maxTick = 2;

        public override void Progress(float time)
        {
            tick++;
            if (tick == maxTick)
            {
                tick = 0;
                if (Animal.CurrentAction == Animal.AnimalCurrentAction.Patrolling)
                {
                    if (!Animal.Movement.IsWalkingSomeWhere)
                    {
                        Patroll();
                    }
                }
                if (Animal.CurrentAction == Animal.AnimalCurrentAction.Escaping)
                {
                    Escape();
                }
            }
        }
        
        private void Escape()
        {
            Animal.Movement.Running = true;
            Vector3 pos = Animal.Movement.Position;
            Vector3 way = pos - Animal.Threat;
            Animal.Movement.WalkTo(pos + way.normalized + new Vector3(Random.Range(-0.5f, 0.5f), 0, Random.Range(-0.5f, 0.5f)) + Animal.Movement.Forward * 0.5f, null);
        }

        private void Patroll()
        {
            Animal.Movement.Running = false;
            ServerUnit food = null;
            if (!Animal.FindFood(ref food))
            {
                Animal.Movement.WalkTo(Animal.Movement.Position +
                                       new Vector3(Random.Range(-5f, 5f), 0, Random.Range(-5f, 5f)) +
                                       Animal.Movement.Forward*2f, null);
            }
            else
            {
                Animal.Details.DoAction(food.ID, "Animal-Eating");
            }
        }

        protected override void OnExtensionWasAdded()
        {
            base.OnExtensionWasAdded();
            Animal = entity as Animal;
        }
    }
}
#endif
