using UnityEngine;
using UnityEngine.EventSystems;

public class InputController : MonoBehaviour
{
    [SerializeField] private Camera gameCamera;
    [SerializeField] private GridManager gridManager;
    [SerializeField] private CameraController cameraController;

    public event System.Action<Vector2Int> OnCellTapped;

    private bool isPressed;
    private bool isDragging;
    private Vector3 pressScreenPos;

    private float DragThresholdPixels => Screen.dpi > 0f ? Screen.dpi * 0.12f : 30f;

    private void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            if (IsPointerOverUI()) return;
            isPressed = true;
            isDragging = false;
            pressScreenPos = Input.mousePosition;
            cameraController.BeginDrag(Input.mousePosition);
        }

        if (!isPressed) return;

        if (Input.GetMouseButton(0))
        {
            if (!isDragging && (Input.mousePosition - pressScreenPos).magnitude > DragThresholdPixels)
            {
                isDragging = true;
            }
            if (isDragging)
            {
                cameraController.Drag(Input.mousePosition);
            }
        }

        if (Input.GetMouseButtonUp(0))
        {
            isPressed = false;
            cameraController.EndDrag();
            if (!isDragging)
            {
                Vector3 world = gameCamera.ScreenToWorldPoint(Input.mousePosition);
                OnCellTapped?.Invoke(gridManager.WorldToGrid(world));
            }
        }
    }

    private bool IsPointerOverUI()
    {
        if (EventSystem.current == null) return false;

        if (EventSystem.current.IsPointerOverGameObject())
            return true;

        if (Input.touchCount > 0)
        {
            return EventSystem.current.IsPointerOverGameObject(Input.GetTouch(0).fingerId);
        }

        return false;
    }
}
