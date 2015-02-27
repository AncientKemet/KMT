#if SERVER
namespace Server.Model.Entities.Animals.Birds
{
    public abstract class Bird : Animal
    {
        protected abstract bool CanFly { get; }

    }
}
#endif
