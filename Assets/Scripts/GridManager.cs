using System.Collections.Generic;
using Unity.Mathematics;
using Unity.VisualScripting;
using UnityEditor.Tilemaps;
using UnityEngine;

public class GridManager : MonoBehaviour
{
    public static GridManager Instance;

    [SerializeField] private int width = 10;
    [SerializeField] private int height = 10;
    [SerializeField] private GameObject tilePrefab;

    [Header("Terrain Data")]
    [SerializeField] private TerrainData plainsTerrain;
    [SerializeField] private TerrainData forestTerrain;
    [SerializeField] private TerrainData hillTerrain;
    [SerializeField] private TerrainData riverTerrain;


    // ******************** TERRAIN GENERATION ********************
    [Header("Base Terrain Generation")]
    [Range(0f, 1f)]
    [SerializeField] private float hillChance = 0.15f;

    [Header("Forest Generation")]
    [SerializeField] private int forestClusterCount = 5;
    [SerializeField] private int minForestClusterSize = 8;
    [SerializeField] private int maxForestClusterSize = 20;

    [Range(0f, 1f)]
    [SerializeField] private float forestSpreadChance = 0.75f;

    [Header("River Generation")]
    [SerializeField] private int riverCount = 1;

    [Range(0f, 1f)]
    [SerializeField] private float riverTurnChance = 0.35f;

    [SerializeField] private int maxRiverStepsMultiplier = 3;
    // ************************************************************

    private GameTile[,] tiles;
    private TerrainData[,] generatedTerrain;

    public GameTile[,] Tiles => tiles;

    public bool isInitialized { get; private set; }

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
    }

    public void DoSetup()
    {
        GenerateGrid();

        // TEMP - spawning test unit here for now.
        SpawnTestUnits();
    }

    public void Initialize()
    {
        isInitialized = true;
    }

    private void GenerateGrid()
    {
        tiles = new GameTile[width, height];
        generatedTerrain = new TerrainData[width, height];

        GenerateBaseTerrain();
        GenerateForests();
        GenerateRivers();
        InstantiateTiles();
    }

    private void GenerateBaseTerrain()
    {
        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                generatedTerrain[x,y] = UnityEngine.Random.value < hillChance ? hillTerrain : plainsTerrain;
            }
        }
    }



    // SpawnTestUnits(): Spawns some inital units, for testing.
    // This will be removed/repurposed later.
    private void SpawnTestUnits()
    {
        // Friendly units
        GameTile spawnTile = tiles[2,2];
        UnitSpawner.Instance.SpawnUnitOnTile(UnitType.FriendlyWeakMelee, spawnTile);
        spawnTile = tiles[3,2];
        UnitSpawner.Instance.SpawnUnitOnTile(UnitType.FriendlyStrongMelee, spawnTile);
        spawnTile = tiles[4,2];
        UnitSpawner.Instance.SpawnUnitOnTile(UnitType.FriendlyRanged, spawnTile);

        // Enemy unit(s)
        spawnTile = tiles[3,6];
        UnitSpawner.Instance.SpawnUnitOnTile(UnitType.EnemyMelee, spawnTile);
    }

    public List<GameTile> GetTilesInMoveRange(Unit unit)
    {
        List <GameTile> validTiles = new List<GameTile>();

        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                GameTile tile = tiles[x,y];

                int distance = GetManhattanDistance(unit.x, unit.y, tile.x, tile.y);

                if (distance <= unit.MovePoints)
                {
                    validTiles.Add(tile);
                }
            }
        }

        return validTiles;
    }

    public List<GameTile> GetTilesInAttackRange(Unit unit)
    {
        List<GameTile> validTiles = new List<GameTile>();

        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                GameTile tile = tiles[x,y];

                int distance = GetManhattanDistance(unit.x, unit.y, tile.x, tile.y);

                if (distance <= unit.AttackRange && tile.IsVisible)
                {
                    validTiles.Add(tile);
                }
            }
        }

        return validTiles;
    }

    public List<GameTile> GetTilesInRange(GameTile origin, int range)
    {
        List<GameTile> validTiles = new List<GameTile>();

        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                GameTile tile = tiles[x,y];

                int distance = GetManhattanDistance(origin.x, origin.y, tile.x, tile.y);
                if (distance <= range) validTiles.Add(tile);
            }
        }

        return validTiles;
    }

    private int GetManhattanDistance(int x1, int y1, int x2, int y2)
    {
        return Mathf.Abs(x1 - x2) + Mathf.Abs(y1 - y2);
    }
}
