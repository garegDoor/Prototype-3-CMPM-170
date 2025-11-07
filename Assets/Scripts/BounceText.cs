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

    // small nudge to avoid retriggering boundary tests
    const float EPS = 0.5f; // pixels

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
        if (halfSize.x < 0f) halfSize.x = 0f;
        if (halfSize.y < 0f) halfSize.y = 0f;
    }

    void Update()
    {
        age += Time.unscaledDeltaTime;
        if (lifetime > 0 && age >= lifetime) { Destroy(gameObject); return; }

        // if layout changed (rare but can happen), keep size fresh
        // (optional: you can move this to OnRectTransformDimensionsChange if you prefer)
        UpdateHalfSize();

        Vector2 pos = rt.anchoredPosition;
        Vector2 delta = velocity * Time.unscaledDeltaTime;

        // proposed new position
        pos += delta;

        // bounds in anchored (local) space
        var rect = bounds.rect;
        float minX = rect.xMin + halfSize.x;
        float maxX = rect.xMax - halfSize.x;
        float minY = rect.yMin + halfSize.y;
        float maxY = rect.yMax - halfSize.y;

        // --- reflect on X ---
        if (pos.x < minX)
        {
            // reflect overshoot back inside
            float over = minX - pos.x;
            pos.x = minX + over + EPS;
            velocity.x = -velocity.x;
        }
        else if (pos.x > maxX)
        {
            float over = pos.x - maxX;
            pos.x = maxX - over - EPS;
            velocity.x = -velocity.x;
        }

        // --- reflect on Y ---
        if (pos.y < minY)
        {
            float over = minY - pos.y;
            pos.y = minY + over + EPS;
            velocity.y = -velocity.y;
        }
        else if (pos.y > maxY)
        {
            float over = pos.y - maxY;
            pos.y = maxY - over - EPS;
            velocity.y = -velocity.y;
        }

        rt.anchoredPosition = pos;
    }

    void OnRectTransformDimensionsChange()
    {
        if (rt != null) UpdateHalfSize();
    }
}
