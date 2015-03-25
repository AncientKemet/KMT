#if SERVER
using Server.Model.ContentHandling;
using Server.Model.Extensions;
using Server.Model.Extensions.UnitExts;
using UnityEngine;

namespace Server.Model.Entities.Vegetation
{
    public abstract class Plant : ServerUnit
    {

        protected abstract int ModelID { get; }

        protected abstract bool CanSeed { get; }

        public abstract bool Collides { get; }

        protected virtual string PlantName { get { return this.GetType().Name; } }

        public float MaxSize { get; private set; }

        protected virtual int MaxSeeds { get { return 3; } }
        protected virtual int MinSeeds { get { return 1; } }
        protected virtual float MaxSeedRange { get { return 15; } }

        protected internal int SeedsLeft { get; set; }

        public float Size { get { return Display.Size; } set { Display.Size = value; } }

        public virtual float LowSize { get { return 0.75f; } }
        public virtual float HighSize { get { return 1.5f; } }

        public virtual float MaxGrassLevel { get { return 1f; } }
        public virtual float MinGrassLevel { get { return 0.1f; } }

        public override bool IsStatic()
        {
            return true;
        }

        protected override ServerUnitPrioritization GetPrioritization()
        {
            return ServerUnitPrioritization.Medium;
        }

        public override void Awake()
        {
            SeedsLeft = Random.Range(MinSeeds, MaxSeeds);

                Display = AddExt<UnitDisplay>();

            Display.ModelID = ModelID;
            name = PlantName;

            Size = Random.Range(LowSize, HighSize);

            base.Awake();

            if (Collides)
            {
                AddExt<CollisionExt>();
            }
        }

        protected override void OnEnterWorld(World world)
        {
            base.OnEnterWorld(world);
            WorldVegeationManager.Instance(world).RegisterPlant(this.GetType(), this);
            Movement.Rotation = Random.Range(0, 360);
        }

        public override void Progress()
        {
            base.Progress();
            if (CanSeed && SeedsLeft > 0)
            {
                SeedsLeft--;


                Plant plant = (Plant)CreateInstance(this.GetType());
                try
                {

                    Vector3 newpos = WorldVegeationManager.Instance(CurrentWorld).GetSeedablePosition(Movement.Position, MaxSeedRange, MinGrassLevel, MaxGrassLevel);

                    plant.Movement.Teleport(newpos);
                    plant.SeedsLeft = SeedsLeft;

                    CurrentWorld.AddEntity(plant);
                }
                catch (WorldVegeationManager.ErrorSpawnException e)
                {
                    Destroy(plant.gameObject);
                }

                SeedsLeft = 0;
            }

        }
    }
}
#endif
