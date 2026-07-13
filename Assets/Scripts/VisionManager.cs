using System;
using System.Collections.Generic;
using UnityEditor.Tilemaps;
using UnityEngine;
using UnityEngine.Tilemaps;



// THIS (VISIBILITYSTATE ENUM) IS CURRENTLY UNUSED, WILL EITHER IMPLEMENT OR REMOVE IN THE FUTURE.
// public enum VisibilityState
// {
//     NeverSeen,           // A tile that's never been seen
//     SeenButNotVisible,   // A tile that has been seen before, but not currently
//     Visible              // A tile that's currently seen.
// }

public class VisionManager : MonoBehaviour
{
    public static VisionManager Instance;

    private readonly HashSet<GameTile> visibleTiles = new HashSet<GameTile>();
    private readonly HashSet<GameTile> seenTiles = new HashSet<GameTile>();

    public bool isInitialized { get; private set; }


    // Awake(): Establishes Instance.
    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
    }

    // Initialize(): Flags to GameInitializer that the VisionManager is ready to go.
    public void Initialize()
    {
        isInitialized = true;
    }

    // RecalculateVision(): Updated vision for tiles and units.
    // This is called each time a movement/spawn/destroy of a unit is done.
    public void RecalculateVision()
    {
        List<Unit> friendlyUnits = UnitManager.Instance.FriendlyUnits;
        GameTile[,] tiles = GridManager.Instance.Tiles;

        visibleTiles.Clear();

        foreach (GameTile tile in tiles)
        {
            tile.SetVisible(false);
        }

        foreach (Unit unit in friendlyUnits)
        {
            AddVisionFromUnit(unit); // This will set "visibleTiles"
        }

        foreach (GameTile tile in seenTiles)
        {
            tile.SetVisible(true);
            if (!visibleTiles.Contains(tile)) tile.SetSeenButNotVisible(true);
        }

        foreach (GameTile tile in visibleTiles)
        {
            tile.SetVisible(true);
        }

        UpdateEnemyVisibility();
    }

    // AddVisionFromUnit(): Adds all tiles a unit can currently see to visible/seen tiles.
    private void AddVisionFromUnit(Unit unit)
    {
        GameTile origin = unit.CurrentTile;
        int range = unit.VisionRange;

        foreach (GameTile tile in GridManager.Instance.GetTilesInRange(origin, range))
        {
            visibleTiles.Add(tile);
            seenTiles.Add(tile);
        }
    }

    // UpdateEnemyVisibility(): Updates whether an enemy unit should currently be visible or not.
    private void UpdateEnemyVisibility()
    {
        foreach (Unit enemy in UnitManager.Instance.EnemyUnits)
        {
            if (visibleTiles.Contains(enemy.CurrentTile)) enemy.GetComponent<SpriteRenderer>().enabled = true;
            else enemy.GetComponent<SpriteRenderer>().enabled = false;
        }
    }


}
