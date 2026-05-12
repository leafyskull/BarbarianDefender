using System.Diagnostics.Tracing;
using JetBrains.Annotations;
using UnityEngine;
using UnityEngine.EventSystems;

public class Unit : MonoBehaviour, IPointerDownHandler
{
    [SerializeField] private GameObject selectionVisual;

    public int x {get; private set;}
    public int y {get; private set;}
    
    private bool isSelected;



    // Init(): Creates a unit, sets it's position.
    //
    // x: x coordinate to spawn unit at.
    // y: y cooridnate to spawn unit at.
    public void Init(int x, int y)
    {
        this.x = x;
        this.y = y;
        transform.position = new Vector3(x, y, -1);
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
}
