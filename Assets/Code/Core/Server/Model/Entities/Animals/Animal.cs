using Server.Model.Entities.Vegetation;
using Random = UnityEngine.Random;
#if SERVER
using System;
using System.Collections.Generic;
using Server.Model.Extensions;
using Server.Model.Extensions.AnimalExtensions;
using Server.Model.Extensions.UnitExts;
using UnityEngine;

namespace Server.Model.Entities.Animals
{
    public abstract class Animal : ServerUnit
    {
        public enum AnimalCurrentAction
        {
            Idle,
            Resting,
            Investigating,
            Playing,
            Patrolling,
            Escaping,
            Attacking
        }

        protected enum AnimalClass
        {
            Herbivore,
            Carcaseater,
            Carnivore,
        }

        public AnimalMovement AMovement { get; private set; }

        protected HashSet<ServerUnit> UnitsSeen = new HashSet<ServerUnit>();
        protected Action<ServerUnit> EncounterUnit;
        private float _scared;
        private float _visionRadius = 32;
        private AnimalCurrentAction _currentAction;
        private float _hunger;

        private int _ecounterTimer = 0;
        private const int EncounterRate = 50;

        public Vector3 Threat = Vector3.zero;

        /// <summary>
        /// This enum represents what is this animal doing at this moment.
        /// </summary>
        public AnimalCurrentAction CurrentAction
        {
            get { return _currentAction; }
            set
            {
                _currentAction = value;
            }
        }
        /// <summary>
        /// From range 0 to 64, will progress units.
        /// </summary>
        protected float VisionRadius
        {
            get { return _visionRadius; }
            set { _visionRadius = value; }
        }

        /// <summary>
        /// From range 0 to 100. The lower this value is The less hunger it is.
        /// </summary>
        public float Hunger
        {
            get { return _hunger; }
            set
            {
                _hunger = Mathf.Clamp(value, 0, 100);
                if (_hunger > 50)
                {
                    if (CurrentAction == AnimalCurrentAction.Idle || CurrentAction == AnimalCurrentAction.Playing ||
                        CurrentAction == AnimalCurrentAction.Resting)
                    {
                        CurrentAction = AnimalCurrentAction.Patrolling;
                    }
                }
                else
                {
                    if (CurrentAction == AnimalCurrentAction.Patrolling)
                    {
                        CurrentAction = AnimalCurrentAction.Idle;
                    }
                }
            }
        }

        /// <summary>
        /// From range 0 to 100. The higher this value is the more scared it is.
        /// When the value hits 50.0 the animal will attempt to retreat.
        /// </summary>
        protected float Scared
        {
            get { return _scared; }
            set
            {
                _scared = Mathf.Clamp(value, 0, 100);
                if (_scared > 50)
                {
                    CurrentAction = AnimalCurrentAction.Escaping;
                }
                else if (CurrentAction == AnimalCurrentAction.Escaping)
                {
                    CurrentAction = AnimalCurrentAction.Idle;
                }
            }
        }

        protected override ServerUnitPrioritization GetPrioritization()
        {
            return ServerUnitPrioritization.Realtime;
        }

        protected abstract AnimalClass Class { get; }

        protected abstract int ModelId { get; }

        public override bool IsStatic()
        {
            return false;
        }

        public override void Progress(float time)
        {
            base.Progress(time);
            Hunger += 0.01f;
            Scared -= 3f;

            _ecounterTimer ++;
            if (_ecounterTimer == EncounterRate)
            {
                _ecounterTimer = 0 + Random.Range(0,2);
                foreach (var o in CurrentBranch.ActiveObjectsVisible)
                {
                    var su = o as ServerUnit;
                    if (o != null)
                    {
                        Vector3 difference = o.GetPosition() - GetPosition();
                        float squareDistance = Mathf.Abs(difference.x) + Mathf.Abs(difference.z);

                        if (squareDistance < VisionRadius)
                        {
                            if (EncounterUnit != null)
                                EncounterUnit(su);
                        }
                        else
                        {
                            if (UnitsSeen.Contains(su))
                            {
                                UnitsSeen.Remove(su);
                            }
                        }
                    }
                }
                foreach (var unit in UnitsSeen)
                {
                    if (unit == null)
                        continue;

                    if (unit is Animal)
                    {
                        Animal a = unit as Animal;
                        if (a.Class == AnimalClass.Carnivore)
                        {
                            float distance = Vector2.Distance(unit.GetPosition(), GetPosition());
                            //the unit is bigger and is carnivore so we should be scared of it.
                            if (distance < VisionRadius * 0.3f)
                                if (unit.Display.Size > Display.Size)
                                {
                                    Scared += 10f * EncounterRate;
                                    Threat += unit.Movement.Position;
                                    Threat /= 2f*EncounterRate;
                                }
                        }
                    }
                }
            }

            
        }

        protected override void OnEnterWorld(World world)
        {
            base.OnEnterWorld(world);
            Threat = Movement.Position;
        }

        protected override void Awake()
        {
            base.Awake();

            Anim = AddExt<UnitAnim>();
            Combat = AddExt<UnitCombat>();
            AMovement = AddExt<AnimalMovement>();

            AddExt<CollisionExt>();

            Display.ModelID = ModelId;

            EncounterUnit += unit =>
            {
                if (!unit.IsStatic())
                {
                    if (!UnitsSeen.Contains(unit))
                        UnitsSeen.Add(unit);
                }
            };
        }

        public bool FindFood(ref ServerUnit food)
        {
            if (Class == AnimalClass.Herbivore)
            {
                List<ServerUnit> foodAround = new List<ServerUnit>(2);
                foreach (var o in CurrentBranch.StaticObjectsVisible)
                {
                    Plant p = o as Plant;
                    if (p != null)
                    {
                        foodAround.Add(p);
                    }
                }
                int index = -1;
                float distance = 1000f;
                for (int i = 0; i < foodAround.Count; i++)
                {
                    if (foodAround[i] != null)
                    {
                        var dis2 = Vector3.Distance(foodAround[i].Movement.Position, Movement.Position);
                        if (dis2 < distance)
                        {
                            index = i;
                            distance = dis2;
                        }
                    }
                }
                if (index != -1)
                {
                    food = foodAround[index];
                    return true;
                }
            }
            else
            {
                foreach (var unit in UnitsSeen)
                {

                }
            }
            return false;
        }
    }
}
#endif
