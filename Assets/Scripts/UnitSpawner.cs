using System.Collections.Generic;
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

    private Dictionary<UnitType, Unit> unitPrefabs;



    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;

        unitPrefabs = new Dictionary<UnitType, Unit>()
        {
            { UnitType.FriendlyWeakMelee, friendlyWeakMeleeUnitPrefab },
            { UnitType.FriendlyStrongMelee, friendlyStrongMeleeUnitPrefab },
            { UnitType.FriendlyRanged, friendlyRangedUnitPrefab },
            { UnitType.EnemyMelee, enemyMeleeUnitPrefab },
        };
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

        Unit newUnit = Instantiate(unitPrefabs[unitType]);

        if (newUnit == null)
        {
            Debug.LogError($"Unknown unit type: {unitType}");
            return null;
        }

        newUnit.Init(tile);
        UnitManager.Instance.AddUnitToManager(newUnit);

        return newUnit;
    }
}
