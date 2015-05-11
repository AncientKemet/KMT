#if SERVER
using System;
using System.Collections.Generic;
using System.Linq;
using Server.Model.Entities.Vegetation;
using Server.Model.Entities.Vegetation.HC;
using UnityEngine;

namespace Server.Model.ContentHandling
{
    public class WorldVegeationManager : WorldEntity {

        private static Dictionary<World, WorldVegeationManager> managers = new Dictionary<World, WorldVegeationManager>();

        public static WorldVegeationManager Instance(World world)
        {
            if (!managers.ContainsKey(world))
            {
                managers.Add(world, CreateInstance<WorldVegeationManager>());
                world.AddEntity(managers[world]);

                managers[world].SpawnPlants();
            }
            return managers[world];
        }

        private Dictionary<Type, int> _amountOfPlants = new Dictionary<Type, int>();
        private List<Plant> _plants = new List<Plant>();

        public int TotalPlantAmount = 0;

        public void RegisterPlant(Type type, Plant plant)
        {
            _plants.Add(plant);

            if (!_amountOfPlants.ContainsKey(type))
            {
                _amountOfPlants.Add(type, 0);
            }
            _amountOfPlants[type]++;
            TotalPlantAmount++;
        }

        public void DeregisterPlant(Type type, Plant plant)
        {
            _plants.Remove(plant);

            if (!_amountOfPlants.ContainsKey(type))
            {
                _amountOfPlants.Add(type, 1);
            }
            _amountOfPlants[type]--;
            TotalPlantAmount--;
        }

        public int GetAmountOfPlants(Type type)
        {
            if (!_amountOfPlants.ContainsKey(type))
            {
                _amountOfPlants.Add(type, 0);
            }
            return _amountOfPlants[type];
        }

        private int _indexProgressingPointer = 0;
        private void FixedUpdate()
        {
            for (int i = 0; i < 1; i++)
            {
              
                if (_indexProgressingPointer > _plants.Count)
                    _indexProgressingPointer = 0;
                try
                {
                    Plant p = _plants[_indexProgressingPointer++];
                    if (p == null || (!p.Collides && p.SeedsLeft == 0))
                    {
                    }
                    else
                    {
                        p.Progress(Time.fixedDeltaTime);
                    }
                }
                catch (Exception e)
                {
                    Debug.LogException(e);
                }  
            }
        }

        private void SpawnPlants()
        {
            for (int i = 0; i < 10; i++)
            {
                SpawnPlantRandom<PumpkinPlant>();
                SpawnPlantRandom<Scrub>();
                SpawnPlantRandom<Scrub>();
                SpawnPlantRandom<Scrub>();
                SpawnPlantRandom<Scrub>();
                SpawnPlantRandom<Scrub>();
                SpawnPlantRandom<Flax>();
                SpawnPlantRandom<Flax>();
                SpawnPlantRandom<Flax>();
                SpawnPlantRandom<Flax>();
                SpawnPlantRandom<Palm>();
                SpawnPlantRandom<Palm>();
                SpawnPlantRandom<Palm>();
                SpawnPlantRandom<Palm>();
                SpawnPlantRandom<Palm>();
                SpawnPlantRandom<HighGrassJungle>();
                SpawnPlantRandom<HighGrassJungle>();
                SpawnPlantRandom<HighGrassJungle>();
                SpawnPlantRandom<HighGrassJungle>();
                SpawnPlantRandom<HighGrassJungle>();
                SpawnPlantRandom<HighGrassJungle>();
                SpawnPlantRandom<HighGrassJungle>();
                SpawnPlantRandom<HighGrassJungle>();
                SpawnPlantRandom<HighGrassJungle>();
                SpawnPlantRandom<HighGrassJungle>();
                SpawnPlantRandom<HighGrassJungle>();
                SpawnPlantRandom<HighGrassJungle>();
                SpawnPlantRandom<HighGrassJungle>();
                SpawnPlantRandom<HighGrassJungle>();
                SpawnPlantRandom<HighGrassJungle>();
                SpawnPlantRandom<HighGrassJungle>();
                SpawnPlantRandom<HighGrassJungle>();
                SpawnPlantRandom<HighGrassJungle>();

            }
        }

        private void SpawnPlantRandom<T>()  where T : Plant
        {
            T plant = CreateInstance<T>();
            try
            {
                Vector3 pos =
                    GetSeedablePosition(
                        new Vector3(Terrain.activeTerrain.terrainData.alphamapWidth/2, 0,
                            Terrain.activeTerrain.terrainData.alphamapHeight/2),
                        Terrain.activeTerrain.terrainData.alphamapWidth/2, plant.MinGrassLevel, plant.MaxGrassLevel);
                plant.Movement.Teleport(pos);

            }
            catch (ErrorSpawnException e)
            {
                Destroy(plant.gameObject);
            }
            CurrentWorld.AddEntity(plant);
        }

        public Vector3 GetSeedablePosition(Vector3 seeder, float range, float minGrass, float maxGrass)
        {
            int tries = 2;
            int tryIndex = 0;
            Vector4[] positionsFound = new Vector4[tries];
            for (int i = 0; i < 500; i++)
            {
                float[] val = null;

                float x = seeder.x + UnityEngine.Random.Range(-range, range);
                float z = seeder.z + UnityEngine.Random.Range(-range, range);

                x = Mathf.Clamp(x, 0, 1024);
                z = Mathf.Clamp(z, 0, 1024);

                try {
                    val = GetTerrainData((int) (x), (int) (z));
                } catch (Exception e) {Debug.LogException(e); }

                float height = val[0];
                float grass = val[3];
                float rock = val[1];
                float sand = val[4];
                float mud = val[2];

                if (grass > minGrass && grass < maxGrass)
                {
                    tries --;
                    positionsFound[tryIndex++] = new Vector4(x, height*20, z, grass);
                }
                if (tries == 0)
                {
                    return positionsFound.OrderByDescending(vector4 => vector4.w).First();
                }
            }

            throw new ErrorSpawnException();
        }

        private float[, ,] alphamaps;
        private float[,] heightMaps;

        private float[] GetTerrainData(int x, int z)
        {
            if (alphamaps == null)
            {

                float time = Time.realtimeSinceStartup;
                alphamaps = Terrain.activeTerrain.terrainData.GetAlphamaps(0, 0,  Terrain.activeTerrain.terrainData.alphamapWidth,  Terrain.activeTerrain.terrainData.alphamapHeight);
                heightMaps = Terrain.activeTerrain.terrainData.GetHeights(0, 0,  Terrain.activeTerrain.terrainData.heightmapWidth, Terrain.activeTerrain.terrainData.heightmapHeight);
                Debug.Log("Vegetation manager Initializing done took: "+(Time.realtimeSinceStartup-time)+"sec");
            }

            float[] data = new float[Terrain.activeTerrain.terrainData.alphamapLayers + 1];

            //Random stuff fixes, it needs to be z / x , lol why Unity
            //also i need to multiply the z,x coz its incorrect
            data[0] = heightMaps[z, x];

            for (int i = 1; i < data.Length; i++)
            {
                data[i] = alphamaps[z, x, i-1];
            }

            return data;
        }


        public class ErrorSpawnException : Exception { }
    }
}
#endif
