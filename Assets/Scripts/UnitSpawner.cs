using Unity.VisualScripting;
using UnityEngine;

public class UnitSpawner : MonoBehaviour
{
    public static UnitSpawner Instance;

    [Header("Friendly unit prefabs")]
    [SerializeField] private FriendlyWeakMeleeUnit friendlyWeakMeleeUnitPrefab;
    [SerializeField] private FriendlyStrongMeleeUnit friendlyStrongMeleeUnitPrefab;
    [SerializeField] private FriendlyRangedUnit friendlyRangedUnitPrefab;

    [Header("Enemy unit prefabs")]
    [SerializeField] private EnemyMeleeUnit enemyMeleeUnitPrefab;



    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
    }

    // SpawnUnitOnTile(): Spawns a unit on the given tile.
    //
    // Spawn will fail if unit or tile is invalid, or if tile is already occupied.
    public Unit SpawnUnitOnTile(UnitType unitType, GameTile tile)
    {
        if (tile == null)
        {
            Debug.LogError("Requested spawn tile is null!");
            return null;
        }

        if (tile.IsOccupied)
        {
            Debug.LogError("Cannot spawn unit here, tile is already occupied!");
            return null;
        }

        Unit newUnit = unitType switch
        {
            UnitType.FriendlyWeakMelee => Instantiate(friendlyWeakMeleeUnitPrefab),
            UnitType.FriendlyStrongMelee => Instantiate(friendlyStrongMeleeUnitPrefab),
            UnitType.FriendlyRanged => Instantiate(friendlyRangedUnitPrefab),
            UnitType.EnemyMelee => Instantiate(enemyMeleeUnitPrefab),
            _ => null
        };

        if (newUnit == null)
        {
            Debug.LogError($"Unknown unit type: {unitType}");
            return null;
        }

        newUnit.Init(tile);

        return newUnit;
    }
}
