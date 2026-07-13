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

    // ***** TRYING SOME CHATGPT-GENERATED MAP GENERATION (START) *****
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

    private void GenerateForests()
    {
        for (int cluster = 0; cluster < forestClusterCount; cluster++)
        {
            Vector2Int seedPosition = new Vector2Int(
                UnityEngine.Random.Range(0, width),
                UnityEngine.Random.Range(0, height)
            );

            int targetClusterSize = UnityEngine.Random.Range(
                minForestClusterSize,
                maxForestClusterSize + 1
            );

            GrowForestCluster(seedPosition, targetClusterSize);
        }
    }

    private void GrowForestCluster(
        Vector2Int seedPosition,
        int targetClusterSize)
    {
        Queue<Vector2Int> frontier = new Queue<Vector2Int>();
        HashSet<Vector2Int> visited = new HashSet<Vector2Int>();

        frontier.Enqueue(seedPosition);
        visited.Add(seedPosition);

        int forestTilesCreated = 0;

        while (frontier.Count > 0 &&
               forestTilesCreated < targetClusterSize)
        {
            Vector2Int current = frontier.Dequeue();

            generatedTerrain[current.x, current.y] = forestTerrain;
            forestTilesCreated++;

            List<Vector2Int> neighbors = GetCardinalNeighbors(current);
            Shuffle(neighbors);

            foreach (Vector2Int neighbor in neighbors)
            {
                if (visited.Contains(neighbor))
                {
                    continue;
                }

                visited.Add(neighbor);

                if (UnityEngine.Random.value <= forestSpreadChance)
                {
                    frontier.Enqueue(neighbor);
                }
            }

            /*
             * A cluster can occasionally stop growing early if none of its
             * neighbors pass the spread chance. Restart growth from one of
             * the already visited forest tiles.
             */
            if (frontier.Count == 0 &&
                forestTilesCreated < targetClusterSize)
            {
                List<Vector2Int> visitedTiles =
                    new List<Vector2Int>(visited);

                Vector2Int restartPosition =
                    visitedTiles[UnityEngine.Random.Range(0, visitedTiles.Count)];

                foreach (Vector2Int neighbor
                         in GetCardinalNeighbors(restartPosition))
                {
                    if (!visited.Contains(neighbor))
                    {
                        visited.Add(neighbor);
                        frontier.Enqueue(neighbor);
                    }
                }
            }
        }
    }

    private void GenerateRivers()
    {
        for (int riverIndex = 0;
             riverIndex < riverCount;
             riverIndex++)
        {
            GenerateRiver();
        }
    }

    private void GenerateRiver()
    {
        bool flowsHorizontally = UnityEngine.Random.value < 0.5f;

        Vector2Int currentPosition;
        Vector2Int mainDirection;

        if (flowsHorizontally)
        {
            bool startsOnLeft = UnityEngine.Random.value < 0.5f;

            currentPosition = startsOnLeft
                ? new Vector2Int(0, UnityEngine.Random.Range(0, height))
                : new Vector2Int(width - 1, UnityEngine.Random.Range(0, height));

            mainDirection = startsOnLeft
                ? Vector2Int.right
                : Vector2Int.left;
        }
        else
        {
            bool startsOnBottom = UnityEngine.Random.value < 0.5f;

            currentPosition = startsOnBottom
                ? new Vector2Int(UnityEngine.Random.Range(0, width), 0)
                : new Vector2Int(UnityEngine.Random.Range(0, width), height - 1);

            mainDirection = startsOnBottom
                ? Vector2Int.up
                : Vector2Int.down;
        }

        int maximumSteps =
            Mathf.Max(width, height) * maxRiverStepsMultiplier;

        int stepsTaken = 0;

        while (IsWithinGrid(currentPosition) &&
               stepsTaken < maximumSteps)
        {
            generatedTerrain[
                currentPosition.x,
                currentPosition.y
            ] = riverTerrain;

            Vector2Int nextDirection = mainDirection;

            if (UnityEngine.Random.value < riverTurnChance)
            {
                nextDirection = GetPerpendicularDirection(mainDirection);
            }

            Vector2Int nextPosition =
                currentPosition + nextDirection;

            /*
             * Do not allow a turn to move the river outside the map.
             * When that happens, continue in the main direction instead.
             */
            if (!IsWithinGrid(nextPosition) &&
                nextDirection != mainDirection)
            {
                nextPosition = currentPosition + mainDirection;
            }

            currentPosition = nextPosition;
            stepsTaken++;
        }
    }

    private Vector2Int GetPerpendicularDirection(
        Vector2Int mainDirection)
    {
        if (mainDirection == Vector2Int.left ||
            mainDirection == Vector2Int.right)
        {
            return UnityEngine.Random.value < 0.5f
                ? Vector2Int.up
                : Vector2Int.down;
        }

        return UnityEngine.Random.value < 0.5f
            ? Vector2Int.left
            : Vector2Int.right;
    }

    private void InstantiateTiles()
    {
        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                Vector3 position = new Vector3(x, y, 0);

                GameObject tileObject = Instantiate(
                    tilePrefab,
                    position,
                    Quaternion.identity,
                    transform
                );

                GameTile tile =
                    tileObject.GetComponent<GameTile>();

                tile.Init(
                    x,
                    y,
                    generatedTerrain[x, y]
                );

                tiles[x, y] = tile;
            }
        }
    }

    private List<Vector2Int> GetCardinalNeighbors(
        Vector2Int position)
    {
        List<Vector2Int> neighbors = new List<Vector2Int>();

        Vector2Int[] directions =
        {
            Vector2Int.up,
            Vector2Int.down,
            Vector2Int.left,
            Vector2Int.right
        };

        foreach (Vector2Int direction in directions)
        {
            Vector2Int neighbor = position + direction;

            if (IsWithinGrid(neighbor))
            {
                neighbors.Add(neighbor);
            }
        }

        return neighbors;
    }

    private bool IsWithinGrid(Vector2Int position)
    {
        return position.x >= 0 &&
               position.x < width &&
               position.y >= 0 &&
               position.y < height;
    }

    private void Shuffle<T>(List<T> list)
    {
        for (int i = list.Count - 1; i > 0; i--)
        {
            int randomIndex = UnityEngine.Random.Range(0, i + 1);

            (list[i], list[randomIndex]) =
                (list[randomIndex], list[i]);
        }
    }

    public GameTile GetTile(int x, int y)
    {
        if (x < 0 || x >= width ||
            y < 0 || y >= height)
        {
            return null;
        }

        return tiles[x, y];
    }
    // ***** TRYING SOME CHATGPT-GENERATED MAP GENERATION (END) *****



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
