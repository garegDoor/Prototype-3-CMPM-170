using UnityEngine;
using TMPro;
using System.Collections.Generic;

public class TextSpawner : MonoBehaviour
{
    [Header("References")]
    public RectTransform PlayArea;              // assign the PlayArea panel
    public TextMeshProUGUI textPrefab;          

    [Header("Messages")]
    [TextArea] public List<string> messages = new List<string>()
    {
        "hey!", "look over here", "pls focus", "ping!", "because 67",
        ":P", "stop ignoring me", "you need to install wheels too", "did you turn in your prototype?", "lol",
        "hello?", "are you still there?", "the fitness gram pacer test", "is a multistage aerobic capacity test",
        "why was 6 afraid of 7?", "did I lock the door?"
    };

    [Header("Spawn")]
    public float spawnInterval = 0.6f;
    public int   maxActive = 50;
    public float lifetime = 6f;

    [Header("Motion")]
    public float minSpeed = 100f;
    public float maxSpeed = 220f;

    int cursor = 0;
    readonly List<GameObject> actives = new();

    void OnEnable()  => StartCoroutine(SpawnLoop());
    void OnDisable() => StopAllCoroutines();

    System.Collections.IEnumerator SpawnLoop()
    {
        var wait = new WaitForSecondsRealtime(spawnInterval);
        while (enabled)
        {
            if (textPrefab && PlayArea && messages.Count > 0)
                SpawnOne();
            yield return wait;
        }
    }

    void SpawnOne()
    {
        // Cull if over limit
        while (actives.Count >= maxActive)
        {
            var oldest = actives[0];
            actives.RemoveAt(0);
            if (oldest) Destroy(oldest);
        }

        var tmp = Instantiate(textPrefab, PlayArea);
        tmp.text = messages[cursor];
        cursor = (cursor + 1) % messages.Count;

        tmp.color = new Color(Random.value, Random.value, Random.value);
        tmp.fontSize = Random.Range(18f, 36f);

        // Ensure correct size then place randomly inside bounds
        tmp.ForceMeshUpdate();
        var rt = tmp.rectTransform;
        rt.sizeDelta = new Vector2(tmp.preferredWidth, tmp.preferredHeight);

        var area = PlayArea.rect;
        var half = rt.sizeDelta * 0.5f;
        var min = area.min + half;
        var max = area.max - half;

        var start = new Vector2(Random.Range(min.x, max.x), Random.Range(min.y, max.y));
        rt.anchoredPosition = start;

        // Random direction & speed
        var dir = Random.insideUnitCircle.normalized;
        if (dir == Vector2.zero) dir = Vector2.right;
        var speed = Random.Range(minSpeed, maxSpeed);

        var b = tmp.gameObject.AddComponent<Bouncer>();
        b.Init(PlayArea, dir * speed);
        b.lifetime = lifetime;

        actives.Add(tmp.gameObject);
    }
}
