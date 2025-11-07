using UnityEngine;

public class DraggableSimple : MonoBehaviour
{
    [Header("Pick Settings")]
    public LayerMask pickMask;                 // Set to Everything minus "Connection"

    [Header("Drag Settings")]
    public Vector3 dragPlaneNormal = Vector3.up;

    public float startDragPixelThreshold = 6f;

    public float followLerp = 20f;

    // --- state ---
    bool mouseDownOverThis;    // we pressed LMB over this object (but not yet dragging)
    bool dragging;
    Plane dragPlane;
    Vector3 grabOffset;        // pivot - hitPoint
    Vector3 mouseDownPos;      // in pixels
    Vector3 startHitPoint;     // world hit point at mouse down

    void Reset()
    {
        // Auto-exclude "Connection" if the layer exists
        int conn = LayerMask.NameToLayer("Connection");
        pickMask = (conn >= 0) ? ~(1 << conn) : ~0;
    }
    void OnDisable()
    {
        mouseDownOverThis = false;
        dragging = false;
    }

    void Update()
    {
        HandleMouseDown();
        HandleMaybeStartDrag();
        HandleDragMove();
        HandleMouseUp();
    }

    // LMB down: capture if we clicked THIS object (not on Connection layer)
    void HandleMouseDown()
    {
        if (!Input.GetMouseButtonDown(0)) return;

        var ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        if (Physics.Raycast(ray, out var hit, 1000f, pickMask))
        {
            if (hit.transform == transform || hit.transform.IsChildOf(transform))
            {
                mouseDownOverThis = true;
                mouseDownPos = Input.mousePosition;

                // Build the plane THROUGH THE HIT POINT (not the object position)
                // so the first projection matches exactly where we clicked.
                startHitPoint = hit.point;
                dragPlane = new Plane(dragPlaneNormal, startHitPoint);

                // Keep the exact offset from hit to pivot to avoid any initial pop.
                grabOffset = transform.position - startHitPoint;
            }
        }
    }

    // Don’t start dragging until the mouse has moved a few pixels (debounce)
    void HandleMaybeStartDrag()
    {
        if (dragging || !mouseDownOverThis) return;

        var delta = (Vector2)(Input.mousePosition - mouseDownPos);
        if (delta.sqrMagnitude >= startDragPixelThreshold * startDragPixelThreshold)
        {
            dragging = true; // now we start moving it
        }
    }

    void HandleDragMove()
    {
        if (!dragging) return;

        var ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        if (dragPlane.Raycast(ray, out float dist))
        {
            // Project current mouse to plane, then re-apply the saved offset
            Vector3 target = ray.GetPoint(dist) + grabOffset;

            transform.position = Vector3.Lerp(transform.position, target, followLerp * Time.unscaledDeltaTime);
        }
    }

    void HandleMouseUp()
    {
        if (!Input.GetMouseButtonUp(0)) return;

        // If we never crossed the pixel threshold, we treat it as a click only — no movement occurred.
        mouseDownOverThis = false;
        dragging = false;
    }
}
