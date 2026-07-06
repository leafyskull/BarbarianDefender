using System.Collections.Generic;
using UnityEngine;

public enum VisibilityState
{
    NeverSeen,           // A tile that's never been seen
    SeenButNotVisible,   // A tile that has been seen before, but not currently
    Visible              // A tile that's currently seen.
}

public class VisionManager : MonoBehaviour
{
    public static VisionManager Instance;

    private readonly HashSet<GameTile> visibleTiles = new HashSet<GameTile>();


    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
    }

    public bool IsTileVisible(GameTile tile)
    {
        return visibleTiles.Contains(tile);
    }

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

        foreach (GameTile tile in visibleTiles)
        {
            tile.SetVisible(true);
        }

        UpdateEnemyVisibility();
    }

    private void AddVisionFromUnit(Unit unit)
    {
        GameTile origin = unit.CurrentTile;
        int range = unit.VisionRange;

        foreach (GameTile tile in GridManager.Instance.GetTilesInRange(origin, range))
        {
            visibleTiles.Add(tile);
        }
    }

    private void UpdateEnemyVisibility()
    {
        foreach (Unit enemy in UnitManager.Instance.EnemyUnits)
        {
            bool isVisible = IsTileVisible(enemy.CurrentTile);
            if (isVisible) enemy.GetComponent<SpriteRenderer>().enabled = true;
            else enemy.GetComponent<SpriteRenderer>().enabled = false;
        }
    }


}
