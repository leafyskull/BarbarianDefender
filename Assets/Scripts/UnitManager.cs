// The UnitManager will keep track of all units, friendly and enemy.
// This is currently being used for enemy visibility.

using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class UnitManager : MonoBehaviour
{
    public static UnitManager Instance;

    private List<Unit> enemyUnits = new List<Unit>();
    private List<Unit> friendlyUnits = new List<Unit>();
    public List<Unit> EnemyUnits => enemyUnits;
    public List<Unit> FriendlyUnits => friendlyUnits;

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

    public void Initialize()
    {
        isInitialized = true;
    }

    // AddUnitToManager(): Starts tracking of unit
    //
    // This will be called when a unit is spawned.
    public void AddUnitToManager(Unit unit)
    {
        if (unit.IsFriendly)
            friendlyUnits.Add(unit);
        else
            enemyUnits.Add(unit);
    }

    // RemoveUnitFromManager(): Stops tracking of unit
    //
    // This will be called when a unit dies.
    public void RemoveUnitFromManager(Unit unit)
    {
        if (unit.IsFriendly)
            friendlyUnits.Remove(unit);
        else
            enemyUnits.Remove(unit);
    }
}
