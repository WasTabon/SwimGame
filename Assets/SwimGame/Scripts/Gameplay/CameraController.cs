using DG.Tweening;
using UnityEngine;

[RequireComponent(typeof(Camera))]
public class CameraController : MonoBehaviour
{
    private const float MaxOrthoSize = 7.5f;
    private const float FitMargin = 1.2f;
    private const float ClampMargin = 1f;

    private Camera cam;
    private Bounds fieldBounds;
    private bool scrollEnabled;
    private Vector3 dragOriginWorld;
    private Vector3 lastDragDelta;
    private Tween moveTween;

    private void Awake()
    {
        cam = GetComponent<Camera>();
    }

    public void Setup(GridManager grid, Vector3 focusPos)
    {
        float w = grid.Width * GridManager.CellSize;
        float h = grid.Height * GridManager.CellSize;
        Vector3 center = new Vector3(
            (grid.Width - 1) * 0.5f * GridManager.CellSize,
            (grid.Height - 1) * 0.5f * GridManager.CellSize,
            0f);
        fieldBounds = new Bounds(center, new Vector3(w, h, 0f));

        float neededHalfHeight = h * 0.5f + FitMargin;
        float neededHalfHeightFromWidth = (w * 0.5f + FitMargin) / cam.aspect;
        float neededSize = Mathf.Max(neededHalfHeight, neededHalfHeightFromWidth);

        moveTween?.Kill();
        transform.DOKill();

        if (neededSize <= MaxOrthoSize)
        {
            scrollEnabled = false;
            cam.orthographicSize = neededSize;
            transform.position = new Vector3(center.x, center.y + neededSize * 0.06f, -10f);
        }
        else
        {
            scrollEnabled = true;
            cam.orthographicSize = MaxOrthoSize;
            transform.position = ClampPosition(new Vector3(focusPos.x, focusPos.y, -10f));
        }
    }

    public void Shake()
    {
        moveTween?.Kill();
        Vector3 original = transform.position;
        transform.DOShakePosition(0.45f, 0.3f, 25, 90f, false, true)
            .OnComplete(() => transform.position = original);
    }

    public void BeginDrag(Vector3 screenPos)
    {
        if (!scrollEnabled) return;
        moveTween?.Kill();
        dragOriginWorld = cam.ScreenToWorldPoint(screenPos);
        lastDragDelta = Vector3.zero;
    }

    public void Drag(Vector3 screenPos)
    {
        if (!scrollEnabled) return;
        Vector3 current = cam.ScreenToWorldPoint(screenPos);
        Vector3 delta = dragOriginWorld - current;
        delta.z = 0f;
        Vector3 newPos = ClampPosition(transform.position + delta);
        lastDragDelta = newPos - transform.position;
        transform.position = newPos;
    }

    public void EndDrag()
    {
        if (!scrollEnabled) return;
        if (lastDragDelta.magnitude < 0.01f) return;
        Vector3 target = ClampPosition(transform.position + lastDragDelta * 8f);
        moveTween?.Kill();
        moveTween = transform.DOMove(target, 0.45f).SetEase(Ease.OutQuad);
    }

    public void EnsureVisible(Vector3 worldPos)
    {
        if (!scrollEnabled) return;
        float halfH = cam.orthographicSize * 0.6f;
        float halfW = cam.orthographicSize * cam.aspect * 0.6f;
        Vector3 pos = transform.position;
        Vector3 delta = Vector3.zero;

        if (worldPos.x > pos.x + halfW) delta.x = worldPos.x - (pos.x + halfW);
        if (worldPos.x < pos.x - halfW) delta.x = worldPos.x - (pos.x - halfW);
        if (worldPos.y > pos.y + halfH) delta.y = worldPos.y - (pos.y + halfH);
        if (worldPos.y < pos.y - halfH) delta.y = worldPos.y - (pos.y - halfH);

        if (delta == Vector3.zero) return;

        moveTween?.Kill();
        moveTween = transform.DOMove(ClampPosition(pos + delta), 0.35f).SetEase(Ease.OutQuad);
    }

    private Vector3 ClampPosition(Vector3 pos)
    {
        float halfH = cam.orthographicSize;
        float halfW = halfH * cam.aspect;

        float minX = fieldBounds.min.x + halfW - ClampMargin;
        float maxX = fieldBounds.max.x - halfW + ClampMargin;
        float minY = fieldBounds.min.y + halfH - ClampMargin;
        float maxY = fieldBounds.max.y - halfH + ClampMargin;

        pos.x = minX > maxX ? fieldBounds.center.x : Mathf.Clamp(pos.x, minX, maxX);
        pos.y = minY > maxY ? fieldBounds.center.y : Mathf.Clamp(pos.y, minY, maxY);
        pos.z = -10f;
        return pos;
    }
}
