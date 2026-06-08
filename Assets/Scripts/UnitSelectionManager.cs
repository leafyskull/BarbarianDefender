using System.Collections.Generic;
using System.Security.Cryptography;
using TMPro;
using UnityEditor.Rendering;
using UnityEngine;
using UnityEngine.UI;

public enum UnitAction
{
    None,
    Move,
    Attack
}

public class UnitSelectionManager : MonoBehaviour
{
    public static UnitSelectionManager Instance { get; private set; }
    public Unit SelectedUnit { get; private set; }
    public UnitAction SelectedUnitAction { get; private set; }
    private readonly List<GameTile> highlightedTiles = new List<GameTile>();


    [Header("UI elements")]
    [SerializeField] private GameObject selectedUnitPanel;
    [SerializeField] private TMP_Text selectedUnitTypeText;
    [SerializeField] private TMP_Text selectedUnitmovePointsText;


    // Awake(): Sets instance reference
    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
    }

    // SelectUnit(): Selects a given unit.
    //
    // unit: The unit to be selected.
    public void SelectUnit(Unit unit)
    {
        // Unselect currently selected unit
        if (SelectedUnit != null)
        {
            SelectedUnit.SetSelected(false);
        }

        // Select new unit
        SelectedUnit = unit;
        SelectedUnit.SetSelected(true);

        UpdateSelectedUnitPanel();        

        Debug.Log($"Selected unit at {unit.x}, {unit.y}");
    }

    // UpdateSelectedUnitPanel(): Enables and updates info on selected unit panel.
    private void UpdateSelectedUnitPanel()
    {
        selectedUnitPanel.SetActive(true);
        selectedUnitmovePointsText.text = SelectedUnit.MovePoints.ToString();
        selectedUnitTypeText.text = SelectedUnit.ToString();
    }

    // ClearSelection(): Clears selected unit.
    public void ClearSelection()
    {
        ClearHighlights();

        if (SelectedUnit != null)
            SelectedUnit.SetSelected(false);

        SelectedUnit = null;

        selectedUnitPanel.SetActive(false);
        SelectedUnitAction = UnitAction.None;
    }

    // OnTileClicked(): Handles unit selection once a tile is clicked.
    //
    // tile: The tile being clicked on.
    public void OnTileClicked(GameTile tile)
    {
        if (SelectedUnit == null)
        {
            Debug.Log($"Clicked tile: {tile.x},{tile.y}");
            return;
        }

        // If you currently have a unit selected, and click on a tile that's occupied.
        if (tile.IsOccupied)
        {
            Debug.Log("Tile is already occupied!");
            return;
        }

        // If you have unit selceted, and click on unoccupied tile
        // TODO: Attack and move choice
        
        // ATTACK

        // MOVE


        SelectedUnit.MoveTo(tile);
        Debug.Log($"Moved unit to {tile.x},{tile.y}");
        ClearSelection();
    }

    // HighlightMoveRange(): Highlights tiles within unit's move range.
    //
    // unit: The unit being referenced.
    //
    // Thought: For now, there's just move points, and each tile is 1 move point.
    // This makes movability, etc. easy to calculate.
    // A future idea is to take the unit's current location as a start point, the destination
    // as the end point, and find a series of tile movements that gets there in the least number
    // of points.
    public void HighlightMoveRange(Unit unit)
    {
        highlightedTiles.Clear();

        List<GameTile> validMoveTiles = GridManager.Instance.GetTilesInMoveRange(unit);

        foreach (GameTile tile in validMoveTiles){
            tile.SetHighlight(true);
            highlightedTiles.Add(tile);
        }
    }

    // HighlightAttackRange(): Highlights all tiles within a unit's attack range.
    //
    // unit: The unit being referenced.
    //
    // Similar future thinking to "HighlightMoveRange" above ^, particularly
    // for handling terrain (hills, forests, etc).
    public void HighlightAttackRange(Unit unit)
    {
        highlightedTiles.Clear();

        List<GameTile> validAttackTiles = GridManager.Instance.GetTilesInAttackRange(unit);

        foreach (GameTile tile in validAttackTiles)
        {
            tile.SetHighlight(true);
            highlightedTiles.Add(tile);
        }
    }

    private void ClearHighlights()
    {
        foreach (GameTile tile in highlightedTiles)
        {
            tile.SetHighlight(false);
        }

        highlightedTiles.Clear();
    }

    // ********** UI interaction **********

    // AttackButtonClicked(): Called when the attack button is clicked.
    //
    // What should happen when attack button is clicked:
    // - Any previous action ("move", etc) is disabled.
    // - "Attack" action is enabled
    // - All tiles in attack range are highlighted.
    // - If tile containing enemy is then clicked, enemy is attacked.
    // - If anything else is clicked, nothing happens.
    public void AttackButtonClicked()
    {
        SelectedUnitAction = UnitAction.Attack;
        HighlightAttackRange(SelectedUnit);
    }

    // MoveButtonClicked(): Called when the move button is clicked.
    //
    // What should happen when move button is clicked:
    // - Any previous action ("attack", etc.) is disabled.
    // - "Move" action is enabled.
    // - All tiles in move range are highlighted.
    // - If free tile is clicked, unit moves to tile and move points are consumed.
    public void MoveButtonClicked()
    {
        SelectedUnitAction = UnitAction.Move;
        HighlightMoveRange(SelectedUnit);
    }


}
