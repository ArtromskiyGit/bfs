using UnityEngine;
using System;

public class DragReceiver : MonoBehaviour, IDragHandler
{
    public event Action<Vector2> OnDrag;

    private void Update()
    {

    }

    public void OnDrag(PointerEventData eventData)
    {
        OnDrag?.Invoke(eventData.delta);
    }
}