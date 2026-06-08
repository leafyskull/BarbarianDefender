using System.Diagnostics.Tracing;
using JetBrains.Annotations;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Tilemaps;

public class Unit : MonoBehaviour, IPointerDownHandler
{
    // The highlight around the unit when they are selected
    [SerializeField] private GameObject selectionVisual;
    public GameTile CurrentTile { get; private set; }

    public int x => CurrentTile.x;
    public int y => CurrentTile.y;
    
    private bool isSelected;
    private int movePoints = 2;
    private int attackRange = 1;
    public int MovePoints => movePoints;
    public int AttackRange => attackRange;
    private int attackStrength = 1;
    private int defendStrength = 1;
    public int AttackStrenght => attackStrength;
    public int DefendStrength => defendStrength;

    private int health = 10;
    public int Health => health;



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

    // OnPointerDown(): Handles clicking.
    public void OnPointerDown(PointerEventData eventData)
    {
        // If currently selected, unselect
        if (isSelected) UnitSelectionManager.Instance.ClearSelection();
        // If not currently selected, select
        else UnitSelectionManager.Instance.SelectUnit(this);
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
    public void AttackUnit(Unit enemyUnit)
    {
        int damage = this.attackStrength - enemyUnit.DefendStrength;
        enemyUnit.ReduceHealth(damage);
    }

    // ReduceHealth(): Reduces a unit's health.
    //
    // amount: Amount of health to take away.
    public void ReduceHealth(int amount)
    {
        if (amount < 0) return;
        this.health -= amount;
    }
}
