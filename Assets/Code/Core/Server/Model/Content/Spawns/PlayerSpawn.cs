using Server.Model.ContentHandling;
using UnityEngine;

namespace Server.Model.Content.Spawns
{
    public class PlayerSpawn : MonoBehaviour
    {
        public Type type;

        void OnEnable()
        {
            ServerSpawnManager.Current.PlayerSpawns.Add(this);
        }

        void OnDisable()
        {
            ServerSpawnManager.Current.PlayerSpawns.Remove(this);
        }

        public enum Type
        {
            Default,
            Mining,
            Woodcutting,
            Crafting,
            Hunting,
        }
    }
}
