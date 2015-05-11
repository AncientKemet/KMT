#if SERVER
using Server.Model.Entities.Human;
using UnityEngine;

namespace Server.Model.Entities.Animals.Birds
{
    public class Chicken : Bird
    {
        protected override AnimalClass Class
        {
            get { return AnimalClass.Herbivore; }
        }

        protected override int ModelId
        {
            get { return 32; }
        }

        protected override bool CanFly
        {
            get { return false; }
        }

        private int _updateCounter = 0;
        private const int UpdateRate = 10;

        public override void Progress(float time)
        {
            base.Progress(time);
            Hunger += 0.1f;
            _updateCounter++;
            if (_updateCounter == UpdateRate)
            {
                _updateCounter = 0;
                foreach (var unit in UnitsSeen)
                {

                    if (unit == null)
                        continue;
                    if (unit is Player)
                    {
                        if (Vector3.Distance(unit.Movement.Position, Movement.Position) < 2f)
                        {
                            Scared += 10f*UpdateRate;
                            Threat += unit.Movement.Position;
                            Threat /= 2f*UpdateRate;
                        }
                    }
                }
            }
        }
    }
}
#endif
