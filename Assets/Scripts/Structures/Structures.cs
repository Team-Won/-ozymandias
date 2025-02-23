﻿using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Managers;
using Map;
using UnityEngine;
using Utilities;
using static Managers.GameManager;
using Random = UnityEngine.Random;

namespace Structures
{
    public class Structures : MonoBehaviour
    {
        public static Action<Structure> OnBuild;
        public static Action<Structure> OnDestroyed;
        public static Action OnGuildHallDemolished;

        [Serializable] private struct Location { public int root, rotation; }

        [SerializeField] private GameObject rockSection, treeSection, structurePrefab;
        [SerializeField] private Material outlineMaterial;
        [SerializeField] private Location dockLocation;
        [SerializeField] private List<Location> spawnLocations; // TODO: Pick new spawn point from random if no guild hall exists

        private readonly List<Structure> _buildings = new List<Structure>();
        private readonly List<Structure> _terrain = new List<Structure>();
        private readonly List<Structure> _ruins = new List<Structure>();
        public readonly Dictionary<string, Section.SectionData> BuildingCache = new Dictionary<string, Section.SectionData>();

        public int Count => _buildings.Count;
        public int Ruins => _ruins.Count;
        public GameObject TreeSection => treeSection;
        public GameObject RockSection => rockSection;
        public GameObject StructurePrefab => structurePrefab;
        public Material OutlineMaterial => outlineMaterial;

        private Location SpawnLocation { get; set; }
        public Vector3 TownCentre { get; private set; }
        public Vector3 Dock => Manager.Map.GetCell(dockLocation.root).WorldSpace;

        private void Awake()
        {
            State.OnNewGame += () => { if (!Tutorial.Tutorial.Active) SpawnGuildHall(); };
            var buildingsText = Resources.LoadAll("SectionData/", typeof(TextAsset)).Cast<TextAsset>().ToArray();
            foreach (TextAsset building in buildingsText)
            {
                BuildingCache.Add(building.name, JsonUtility.FromJson<Section.SectionData>(building.text));
            }
        }

        public int GetStat(Stat stat)
        {
            return _buildings.Sum(b => (b.Stats.ContainsKey(stat) ? b.Stats[stat] : 0) + (b.Bonus == stat ? 1 : 0));
        }

        public int GetCount(BuildingType type)
        {
            return _buildings.Count(x => x.Blueprint.type == type);
        }

        public List<Structure> GetAll(BuildingType type)
        {
            return _buildings.Where(x => x.Blueprint.type == type).ToList();
        }
        
        public Structure GetRandom()
        {
            return _buildings.Where(x => x.Blueprint.type != BuildingType.GuildHall).ToList().SelectRandom();
        }
        public Structure GetRandom(BuildingType type)
        {
            return GetAll(type).SelectRandom();
        }

        public float GetClosestDistance(Vector3 position)
        {
            // Find distance of closest building to the camera
            try
            {
                return _buildings.Select(building => Vector3.Distance(position, building.transform.position)).Min();
            }
            catch
            {
                return float.MaxValue;
            }
        }

        public Structure GetClosest(Vector3 position)
        {
            Structure closestBuilding = null;
            float closestDistance = float.MaxValue;
            foreach (Structure building in _buildings)
            {
                float distance = Vector3.Distance(building.transform.position, position);
                if (!(distance < closestDistance)) continue;
                closestBuilding = building;
                closestDistance = distance;
            }

            return closestBuilding;
        }

        public Cell RandomCell => _buildings.SelectRandom().Occupied.SelectRandom();

        public bool AddBuilding(Blueprint blueprint, int rootId, int rotation = 0, bool isRuin = false)
        {
            Structure structure = Instantiate(structurePrefab, transform).GetComponent<Structure>();

            if (!structure.CreateBuilding(blueprint, rootId, rotation, isRuin))
            {
                Debug.LogError("Building has failed to place " + structure.name);
                Destroy(structure.gameObject);
                return false;
            }

            // Add to correct collection for querying
            if (structure.IsRuin) _ruins.Add(structure);
            else _buildings.Add(structure);

            if (!Manager.State.Loading)
            {
                CheckAdjacencyBonuses();
                OnBuild?.Invoke(structure);
                UpdateUi();
            }
            return true;
        }

        public void AddTerrain(int rootId, int sectionCount = 4)
        {
            Structure structure = Instantiate(structurePrefab, transform).GetComponent<Structure>();
            structure.CreateTerrain(rootId, sectionCount);
            _terrain.Add(structure);
        }

        public void Remove(Structure structure)
        {
            if (structure.Blueprint && structure.Blueprint.type == BuildingType.GuildHall)
            {
                Manager.EventQueue.AddGuildHallDestroyedEvents();
                Manager.State.EnterState(GameState.NextTurn);
                OnGuildHallDemolished?.Invoke();
            }

            if (structure.IsTerrain) _terrain.Remove(structure);
            else if (structure.IsRuin) _ruins.Remove(structure);
            else _buildings.Remove(structure);

            structure.Destroy();
            CheckAdjacencyBonuses();
            OnDestroyed?.Invoke(structure);
            if (!Manager.State.Loading) UpdateUi();
        }

        public List<Cell> GetAdjacencyBonusCells(Blueprint blueprint)
        {
            AdjacencyConfiguration config = blueprint.adjacencyConfig;
            if(!config.hasBonus || !Manager.Upgrades.IsUnlocked(config.upgrade)) return new List<Cell>();

            if (config.specialCheck)
            {
                if (config.structureType == StructureType.Ruins)
                    return _ruins.SelectMany(b => Manager.Map.GetValidAdjacencies(b)).Distinct().ToList();
                
                if (config.structureType == StructureType.Terrain)
                    return _terrain.SelectMany(b => Manager.Map.GetValidAdjacencies(b)).Distinct().ToList();
                
                switch (blueprint.type)
                {
                    case BuildingType.Farm:
                        return Manager.Map.GetCellsNextToTwoFarms();
                    case BuildingType.BathHouse:
                        return Manager.Map.GetCellsByWater();
                    case BuildingType.Monastery:
                    case BuildingType.Watchtower:
                        return Manager.Map.GetCellsWithNoNeighbours();
                }
            }
            else
            {
                return _buildings
                    .Where(b => b.Blueprint.type == config.neighbourType)
                    .SelectMany(b => Manager.Map.GetValidAdjacencies(b))
                    .Distinct()
                    .ToList();
            }

            return new List<Cell>();
        }
        
        public void CheckAdjacencyBonuses()
        {
            _buildings.ForEach(building => building.CheckAdjacencyBonus());
        }

        public IEnumerator ResetGrid()
        {
            int countPerFrame = 0;
            int newRuinsCount = 0;
            List<Structure> dupList = _buildings.OrderByDescending(b => Vector3.Distance(b.transform.position, TownCentre)).ToList();
            foreach (Structure building in dupList)
            {
                if (building.Blueprint.type != BuildingType.GuildHall &&
                    building.Blueprint.type != BuildingType.Farm &&
                    building.Blueprint.type != BuildingType.Markets &&
                    building.Blueprint.type != BuildingType.Plaza &&
                    building.Blueprint.type != BuildingType.Barracks &&
                    building.Blueprint.type != BuildingType.Armoury &&
                    building.Blueprint.type != BuildingType.BathHouse &&
                    building.Blueprint.type != BuildingType.Monastery &&
                    building.Blueprint.type != BuildingType.FightingRing &&
                    Random.Range(0, newRuinsCount + 1) == 0
                )
                {
                    ToRuin(building);
                    newRuinsCount++;
                }
                else building.Destroy();
                
                if (++countPerFrame < Manager.MaxStructuresPerFrame) continue;
                countPerFrame = 0;
                yield return null;
            }
            
            _buildings.Clear();
            yield return Manager.Map.FillGrid();
            
            SpawnLocation = NewSpawnLocation();
            TownCentre = Manager.Map.GetCell(SpawnLocation.root).WorldSpace;
        }

        private void ToRuin(Structure structure)
        {
            _buildings.Remove(structure);
            _ruins.Add(structure);
            structure.ToRuin();
        }

        public int NewQuestSpawn()
        {
            List<Structure> furthestBuildings = _buildings
                .OrderByDescending(building => Vector3.Distance(building.transform.position, TownCentre))
                .ToList();

            foreach (Structure building in furthestBuildings)
            {
                Vector3 position = Vector3.MoveTowards(building.transform.position, TownCentre, -Random.Range(3f, 10f));
                Cell cell = Manager.Map.GetClosestCell(position);
                if (cell != null && cell.Active && (!cell.Occupied || cell.Occupant.IsTerrain) && Manager.Quests.FarEnoughAway(cell.WorldSpace))
                {
                    return cell.Id;
                }
            }

            Debug.LogWarning("Quests: Couldn't find valid location for quest");
            //TODO: A backup better than this
            return Random.Range(200, 1200);
        }

        private Location NewSpawnLocation() => spawnLocations.SelectRandom();

        public void RemoveRandomNearbyTerrain()
        {
            List<Structure> nearby = Manager.Map
                .GetCells(TownCentre, 12)
                .Where(cell => cell.Occupied && cell.Occupant.IsTerrain)
                .Select(cell => cell.Occupant)
                .Distinct()
                .ToList();

            for (int i = 0; i < 8 && nearby.Count > 0; i++) Remove(nearby.PopRandom());
            SaveFile.SaveState(false);
        }
        
        public void SpawnGuildHall()
        {
            foreach (Cell cell in Manager.Map.GetCells(TownCentre, 4).Where(cell => cell.Occupied && cell.Occupant.IsTerrain)) Remove(cell.Occupant);
            Manager.Structures.AddBuilding(Manager.Cards.GuildHall, SpawnLocation.root, SpawnLocation.rotation);
        }

        public void SpawnTutorialRuins()
        {
            AddBuilding(Manager.Cards.Find(BuildingType.Herbalist), 158, 1, true);
            AddBuilding(Manager.Cards.Find(BuildingType.Herbalist), 225, 3, true);
            AddBuilding(Manager.Cards.Find(BuildingType.Herbalist), 445, 2, true);
        }

        public StructureDetails Save()
        {
            return new StructureDetails
            {
                buildings = _buildings.Concat(_ruins).Select(x => x.SaveBuilding()).ToList(),
                terrain = _terrain.Select(x => x.SaveTerrain()).ToList()
            };
        }

        public Coroutine Load(StructureDetails details)
        {
            return StartCoroutine(LoadCoroutine(details));
        }

        private IEnumerator LoadCoroutine(StructureDetails details)
        {
            int countPerFrame = 0;

            foreach (BuildingDetails building in details.buildings ?? new List<BuildingDetails>())
            {
                Blueprint blueprint = Manager.Cards.Find(building.type);
                if (!blueprint) Debug.LogError(
                    "Blueprint of type \"" + building.type + "\" could not be found." +
                    "It may not yet be available to the player.");
                AddBuilding(blueprint, building.rootId, building.rotation, building.isRuin);

                if (++countPerFrame < Manager.MaxStructuresPerFrame) continue;
                countPerFrame = 0;
                yield return null;
            }

            foreach (TerrainDetails terrain in details.terrain ?? new List<TerrainDetails>())
            {
                AddTerrain(terrain.rootId, terrain.sectionCount);

                if (++countPerFrame < Manager.MaxStructuresPerFrame) continue;
                countPerFrame = 0;
                yield return null;
            }

            Structure guildHall = _buildings.Find(structure => structure.Blueprint.type == BuildingType.GuildHall);
            if (guildHall)
                TownCentre = guildHall.Occupied[0].WorldSpace;
            else
            {
                SpawnLocation = Tutorial.Tutorial.Active ? spawnLocations[0] : NewSpawnLocation();
                TownCentre = Manager.Map.GetCell(SpawnLocation.root).WorldSpace;
            }

            CheckAdjacencyBonuses(); // Checks at end of load so it doesn't repeat
        }
    }
}
