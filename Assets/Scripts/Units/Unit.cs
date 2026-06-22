using System.Diagnostics.Tracing;
using JetBrains.Annotations;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Tilemaps;

public enum UnitType
{
    FriendlyWeakMelee,
    FriendlyStrongMelee,
    FriendlyRanged,
    EnemyMelee,
}

public enum UnitTeam
{
    Friendly,
    Enemy
}

public class Unit : MonoBehaviour
{
    [Header("Visuals")]
    [SerializeField] private GameObject selectionVisual;

    public GameTile CurrentTile { get; private set; }

    public int x => CurrentTile.x;
    public int y => CurrentTile.y;
    
    [Header("Stats")]
    private bool isSelected;
    protected int maxHealth = 10;
    protected int health;
    protected int movePoints = 2;
    protected int attackRange = 1;
    protected int attackStrength = 1;
    protected int defendStrength = 1;

    public int MovePoints => movePoints;
    public int AttackRange => attackRange;
    public int AttackStrength => attackStrength;
    public int DefendStrength => defendStrength;
    public int Health => health;

    [SerializeField] protected UnitTeam team;
    public UnitTeam Team => team;
    public bool IsFriendly => team == UnitTeam.Friendly;
    public bool IsEnemy => team == UnitTeam.Enemy;

    protected virtual void Awake()
    {
        this.health = maxHealth;
    }

    // Init(): Creates a unit, sets it's position.
    //
    // x: x coordinate to spawn unit at.
    // y: y cooridnate to spawn unit at.
    public void Init(GameTile startingTile)
    {
        this.CurrentTile = startingTile;
        CurrentTile.SetOccupyingUnit(this);
        
        transform.position = new Vector3(startingTile.x, startingTile.y, -1);
        SetSelected(false);
    }
    
    // SetSelected(): Sets the unit as the selected unit
    // If unit is selected, a border will display aroung them.
    //
    // isSelected(): Whether the unit should be selected or not.
    public void SetSelected(bool isSelected)
    {
        if (selectionVisual != null)
            selectionVisual.SetActive(isSelected);
        else
            Debug.Log($"Selection visual for unit is null!");
        
        this.isSelected = isSelected;
    }


    // MoveTo(): Moves unit to a new tile.
    //
    // destinationTile: The tile to move to.
    public void MoveTo(GameTile destinationTile)
    {
        if (CurrentTile != null)
            CurrentTile.ClearOccupyingUnit();

        this.CurrentTile = destinationTile;
        this.CurrentTile.SetOccupyingUnit(this);

        transform.position = new Vector3(destinationTile.x, destinationTile.y, -1);
    }

    // AttackUnit(): Makes this unit attack a target unit.
    //
    // enemyUnit: The unit to attack.
    //
    // For now, I'll just have the damage done be the difference between attacker's attack
    // strength and the target's defense strength. (TEMP)
    public void AttackUnit(Unit targetUnit)
    {
        int damage = Mathf.Max(0, this.attackStrength - targetUnit.DefendStrength);
        if (damage > targetUnit.Health) damage = targetUnit.Health;

        targetUnit.ReduceHealth(damage);
        
        Debug.Log($"{this} attacks {targetUnit}, does {damage} damage!");
        Debug.Log($"{targetUnit} remaining health: {targetUnit.Health}");
    }

    // ReduceHealth(): Reduces a unit's health.
    //
    // amount: Amount of health to take away.
    public void ReduceHealth(int amount)
    {
        if (amount < 0) return;

        this.health -= amount;

        if (this.health <= 0)
            Die();
    }

    // Die(): Destroys a unit.
    private void Die()
    {
        if (CurrentTile != null)
            CurrentTile.ClearOccupyingUnit();
        
        Destroy(gameObject);
    }
}
