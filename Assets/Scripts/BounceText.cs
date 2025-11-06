using UnityEngine;

[RequireComponent(typeof(RectTransform))]
public class Bouncer : MonoBehaviour
{
    public Vector2 velocity = new Vector2(120f, 140f);
    public float lifetime = 8f;

    RectTransform rt;
    RectTransform bounds;
    Vector2 halfSize;
    float age;

    public void Init(RectTransform bounds, Vector2 startVelocity)
    {
        this.bounds = bounds;
        rt = GetComponent<RectTransform>();
        velocity = startVelocity;
        UpdateHalfSize();
    }

    void UpdateHalfSize()
    {
        halfSize = rt.rect.size * 0.5f;
    }

    void Update()
    {
        age += Time.unscaledDeltaTime;
        if (lifetime > 0 && age >= lifetime) { Destroy(gameObject); return; }

        var pos = rt.anchoredPosition + velocity * Time.unscaledDeltaTime;

        var min = bounds.rect.min + halfSize;
        var max = bounds.rect.max - halfSize;

        if (pos.x < min.x) { pos.x = min.x; velocity.x *= -1f; }
        else if (pos.x > max.x) { pos.x = max.x; velocity.x *= -1f; }

        if (pos.y < min.y) { pos.y = min.y; velocity.y *= -1f; }
        else if (pos.y > max.y) { pos.y = max.y; velocity.y *= -1f; }

        rt.anchoredPosition = pos;
    }

    void OnRectTransformDimensionsChange()
    {
        if (rt != null) UpdateHalfSize();
    }
}
