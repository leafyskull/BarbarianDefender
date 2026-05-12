using UnityEngine;

public class UnitSelectionManager : MonoBehaviour
{
    public static UnitSelectionManager Instance { get; private set; }
    public Unit SelectedUnit { get; private set; }



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

        Debug.Log($"Selected unit at {unit.x}, {unit.y}");
    }

    // ClearSelection(): Clears selected unit.
    public void ClearSelection()
    {
        if (SelectedUnit != null)
            SelectedUnit.SetSelected(false);

        SelectedUnit = null;
    }
}
