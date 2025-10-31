using UnityEngine;

public class Draggable : MonoBehaviour
{
    Camera cam;
    bool dragging;
    Vector3 offset;

    void Awake() { cam = Camera.main; }

    void OnMouseDown()
    {
        dragging = true;
        offset = transform.position - GetMouseOnPlane();
    }

    void OnMouseUp() { dragging = false; }

    void Update()
    {
        if (!dragging) return;
        transform.position = GetMouseOnPlane() + offset;
    }

    Vector3 GetMouseOnPlane()
    {
        var plane = new Plane(Vector3.up, Vector3.zero);
        var ray = cam.ScreenPointToRay(Input.mousePosition);
        plane.Raycast(ray, out float d);
        return ray.GetPoint(d);
    }
}
