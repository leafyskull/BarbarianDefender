using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;

public class GameTile : MonoBehaviour, IPointerDownHandler
{
    public int x;
    public int y;

    public Unit OccupyingUnit { get; private set; }
    public bool IsOccupied => OccupyingUnit != null;
    public bool IsHighlighted { get; private set; }
    public bool IsVisible { get; private set; }

    [Header("Visuals")]
    [SerializeField] private SpriteRenderer highlightRenderer;
    [SerializeField] private SpriteRenderer hiddenRenderer;

    public void Init(int tileX, int tileY)
    {
        x = tileX;
        y = tileY;
        SetHighlight(false);
        SetVisible(false);
    }

    // OnPointerDown(): Handles clicking on tiles.
    //
    // The current idea is this: When a tile is clicked, it passes itself to the UnitSelectionManager
    // to handle what happens.
    public void OnPointerDown(PointerEventData eventData)
    {
        UnitSelectionManager.Instance.OnTileClicked(this);
    }

    public void SetOccupyingUnit(Unit unit)
    {
        this.OccupyingUnit = unit;   
    }

    public void ClearOccupyingUnit()
    {
        this.OccupyingUnit = null;
    }

    public void SetHighlight(bool isHighlighted)
    {
        this.IsHighlighted = isHighlighted;

        if (highlightRenderer != null)
            highlightRenderer.enabled = isHighlighted;
    }

    public void SetHighlightColor(Color color)
    {
        this.highlightRenderer.color = color;
    }

    public void SetVisible(bool isVisible)
    {
        this.IsVisible = isVisible;

        hiddenRenderer.enabled = isVisible;
    }

}