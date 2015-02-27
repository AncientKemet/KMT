#if SERVER
using Server.Model.Content.Spawns;

using Server.Model.Entities;
using Server.Model.Entities.Animals;
using UnityEngine;
using System.Collections.Generic;

namespace Server.Model.ContentHandling
{
    public class ServerSpawnManager : WorldEntity
    {
        private List<SpawnMB> _managedSpawnsUnits = new List<SpawnMB>();

        public void AddItemSpawn(SpawnMB aSpawn)
        {
            _managedSpawnsUnits.Add(aSpawn);
        }

        public T SpawnAnimal<T>(Vector3 position) where T : Animal
        {
            T animal = CreateInstance<T>();
            animal.Movement.Teleport(position);
            CurrentWorld.AddEntity(animal);
            return animal;
        }

        public T Spawn<T>(Vector3 position) where T: ServerUnit
        {
            T unit = CreateInstance<T>();
            unit.Movement.Teleport(position);
            CurrentWorld.AddEntity(unit);
            return unit;
        }

        private void FixedUpdate()
        {
            foreach (var spawn in _managedSpawnsUnits)
            {
                if (spawn.SpawnedEntity == null)
                {
                    spawn.Spawn();
                    if (spawn.SpawnedEntity != null)
                        CurrentWorld.AddEntity(spawn.SpawnedEntity);
                }
            }
        }

        private static Dictionary<World, ServerSpawnManager> managers = new Dictionary<World, ServerSpawnManager>();  

        public static ServerSpawnManager Instance(World world)
        {
            if (!managers.ContainsKey(world))
            {
                managers.Add(world, CreateInstance<ServerSpawnManager>());
                world.AddEntity(managers[world]);
            }
            return managers[world];
        }
    }
}

#endif
