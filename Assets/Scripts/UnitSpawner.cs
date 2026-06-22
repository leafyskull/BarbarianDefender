using Unity.VisualScripting;
using UnityEngine;

public class UnitSpawner : MonoBehaviour
{
    [Header("Friendly unit prefabs")]
    [SerializeField] private FriendlyWeakMeleeUnit friendlyWeakMeleeUnitPrefab;
    [SerializeField] private FriendlyStrongMeleeUnit friendlyStrongMeleeUnitPrefab;
    [SerializeField] private FriendlyRangedUnit friendlyRangedUnitPrefab;

    [Header("Enemy unit prefabs")]
    [SerializeField] private EnemyMeleeUnit enemyMeleeUnitPrefab;


    public void SpawnUnitOnTile(string unitType, GameTile tile)
    {
        switch (unitType.ToLower())
        {
            case "friendlyWeakMelee":
                FriendlyWeakMeleeUnit newUnit = Instantiate(friendlyWeakMeleeUnitPrefab);
                newUnit.Init(tile);
                break;
            
            // And on and on....
        }
    }
}
