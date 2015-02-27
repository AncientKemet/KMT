#if SERVER
namespace Server.Model.Entities
{
    public enum ServerUnitPrioritization
    {
        Realtime = 0,
        Medium = 10,
        Low = 100,
        Off = 1000,
    }
}
#endif
