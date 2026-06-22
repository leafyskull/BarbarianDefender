using System.Collections.Generic;
using Unity.Mathematics;
using Unity.VisualScripting;
using UnityEditor.Tilemaps;
using UnityEngine;

public class GridManager : MonoBehaviour
{
    [SerializeField] private int width = 10;
    [SerializeField] private int height = 10;

    [SerializeField] private GameObject tilePrefab;
    [SerializeField] private Unit unitPrefab; // TEMP: For spawning test unit
    
    private GameTile[,] tiles;
    public static GridManager Instance;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        GenerateGrid();

        // TEMP - spawning test unit here for now.
        SpawnTestUnits();
    }

    private void GenerateGrid()
    {
        tiles = new GameTile[width, height];

        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < width; y++)
            {
                Vector3 position = new Vector3(x, y, 0);
                GameObject tileObject = Instantiate(tilePrefab, position, Quaternion.identity);
                GameTile tile = tileObject.GetComponent<GameTile>();
                tile.Init(x, y);
                tiles[x,y] = tile;
            }
        }
    }

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
                if (distance <= unit.AttackRange)
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
