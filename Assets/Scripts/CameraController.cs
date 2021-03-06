using UnityEngine;

[RequireComponent(typeof(Camera))]
public class CameraController : MonoBehaviour
{
    [SerializeField] private DragReceiver dragReceiver;
    [SerializeField] private float zoomSensitivity;
    [SerializeField] private float minOrthographicSize;
    [SerializeField] private float maxOrthopraghicSize;
     
    private Camera cam;
    private Vector3 oldWorldPoint;
    private float normalizedZoom;

    private void Awake()
    {
        dragReceiver.Drag += Move;
    }

    private void Update()
    {
        Zoom();
    }
    
    private void Move(Vector2 screenDelta)
    {
        var worldPoint = cam.ScreenToWorldPoint(screenDelta);
        var worldDelta = -(worldPoint - oldWorldPoint);

        oldWorldPoint = cam.ScreenToWorldPoint(screenDelta);
        
        transform.position += worldDelta;
    }

    private void Zoom()
    {
        var scrollDelta = Input.mouseScrollDelta;
        normalizedZoom = Mathf.Clamp01(normalizedZoom + scrollDelta.magnitude);

        cam.orthographicSize = Mathf.Lerp(minOrthographicSize, maxOrthopraghicSize, normalizedZoom);
    }
}