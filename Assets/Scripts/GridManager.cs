using Unity.VisualScripting;
using UnityEngine;

public class GridManager : MonoBehaviour
{
    [SerializeField] private int width = 10;
    [SerializeField] private int height = 10;

    [SerializeField] private GameObject tilePrefab;
    [SerializeField] private Unit unitPrefab; // TEMP: For spawning test unit
    
    private GameTile[,] tiles;


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        GenerateGrid();

        // TEMP - spawning test unit here for now.
        SpawnTestUnit();
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

    private void SpawnTestUnit()
    {
        Unit unit = Instantiate(unitPrefab);
        unit.Init(2,2);
    }
}
