using UnityEngine;
using UnityEngine.EventSystems;

public class GameTile : MonoBehaviour, IPointerDownHandler
{
    public int x;
    public int y;

    public void Init(int tileX, int tileY)
    {
        x = tileX;
        y = tileY;
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        Debug.Log($"Clicked tile: {x}, {y}");
    }
}