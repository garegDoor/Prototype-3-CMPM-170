using UnityEngine;

public class DraggableSimple : MonoBehaviour
{
    [Header("Pick Settings")]
    [Tooltip("Layers that can start a drag. Exclude the 'Connection' layer.")]
    public LayerMask pickMask;                 // Set in Inspector to Everything minus "Connection"

    [Header("Drag Settings")]
    [Tooltip("Drag on a plane whose normal is Up (horizontal plane).")]
    public Vector3 dragPlaneNormal = Vector3.up;
    [Tooltip("Higher = snappier follow.")]
    public float followLerp = 20f;

    [Header("Physics (optional)")]
    public Rigidbody rb;                       // Assign if present
    public bool kinematicWhileDragging = true; // Keeps physics from fighting while dragging

    bool dragging;
    Plane dragPlane;
    Vector3 grabOffset;

    void Reset()
    {
        // Auto-exclude "Connection" if the layer exists
        int conn = LayerMask.NameToLayer("Connection");
        pickMask = (conn >= 0) ? ~(1 << conn) : ~0;
    }

    void Awake()
    {
        if (!rb) rb = GetComponent<Rigidbody>();
    }

    void OnDisable()
    {
        dragging = false;
        if (rb && kinematicWhileDragging) rb.isKinematic = true;
    }

    void Update()
    {
        HandleStartStopDrag();
        HandleDragMove();
    }

    // --------- DRAG START/STOP ----------
    void HandleStartStopDrag()
    {
        if (Input.GetMouseButtonDown(0))
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            if (Physics.Raycast(ray, out var hit, 1000f, pickMask))
            {
                // Only begin drag if we hit this object (or one of its children),
                // and NOT a "Connection" (filtered by pickMask).
                if (hit.transform == transform || hit.transform.IsChildOf(transform))
                    StartDrag(hit.point);
            }
        }

        if (dragging && Input.GetMouseButtonUp(0))
            StopDrag();
    }

    void StartDrag(Vector3 worldHit)
    {
        dragging = true;

        // Create a plane through the current object position with the chosen normal
        dragPlane = new Plane(dragPlaneNormal, transform.position);

        // Maintain the offset from hit point to object pivot so dragging feels “grabbed”
        grabOffset = transform.position - worldHit;

        if (rb && kinematicWhileDragging) rb.isKinematic = true;
    }

    void StopDrag()
    {
        dragging = false;
        if (rb && kinematicWhileDragging) rb.isKinematic = true; // keep kinematic after drag
    }

    // --------- DRAG MOVE ----------
    void HandleDragMove()
    {
        if (!dragging) return;

        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        if (dragPlane.Raycast(ray, out float dist))
        {
            Vector3 target = ray.GetPoint(dist) + grabOffset;

            if (rb && kinematicWhileDragging)
                rb.MovePosition(Vector3.Lerp(transform.position, target, followLerp * Time.unscaledDeltaTime));
            else
                transform.position = Vector3.Lerp(transform.position, target, followLerp * Time.unscaledDeltaTime);
        }
    }
}
