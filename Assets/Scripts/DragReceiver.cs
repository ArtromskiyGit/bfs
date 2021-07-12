using UnityEngine;
using System;
using UnityEngine.EventSystems;

public class DragReceiver : MonoBehaviour, IDragHandler
{
    public event Action<Vector2> Drag;

    private void Update()
    {

    }

    public void OnDrag(PointerEventData eventData)
    {
        Drag?.Invoke(eventData.delta);
    }
}