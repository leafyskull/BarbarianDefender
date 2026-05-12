using System.Diagnostics.Tracing;
using JetBrains.Annotations;
using UnityEngine;
using UnityEngine.EventSystems;

public class Unit : MonoBehaviour, IPointerDownHandler
{
    [SerializeField] private GameObject selectionVisual;

    public int x {get; private set;}
    public int y {get; private set;}

    public void Init(int x, int y)
    {
        this.x = x;
        this.y = y;
        transform.position = new Vector3(x, y, -1);
        SetSelected(false);
    }
    
    public void SetSelected(bool isSelected)
    {
        if (selectionVisual != null)
            selectionVisual.SetActive(isSelected);
    }


    public void OnPointerDown(PointerEventData eventData)
    {
        
    }
}
