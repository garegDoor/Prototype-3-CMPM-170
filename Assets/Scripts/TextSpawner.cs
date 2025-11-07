using UnityEngine;
using TMPro;
using System.Collections.Generic;

public class TextSpawner : MonoBehaviour
{
    [Header("References")]
    public RectTransform PlayArea;              // assign the PlayArea panel
    public TextMeshProUGUI textPrefab;

    [Header("Messages")]
    [TextArea]
    public List<string> messages = new List<string>()
    {
        "hey!", "look over here", "pls focus", "ping!", "because 67",
        ":P", "stop ignoring me", "you need to install wheels too", "did you turn in your prototype?", "lol",
        "hello?", "are you still there?", "the fitness gram pacer test", "is a multistage aerobic capacity test",
        "why was 6 afraid of 7?", "did I lock the door?"
    };

    [Header("Spawn")]
    public float spawnInterval = 0.6f;
    public int maxActive = 50;
    public float lifetime = 6f;

    [Header("Motion")]
    public float minSpeed = 100f;
    public float maxSpeed = 220f;

    [Header("Appearance")]
    public float minFontSize = 24f;
    public float maxFontSize = 96f;   // <- make it bigger as you like
    public bool vividColors = true;  // use HSV for punchy colors


    int cursor = 0;
    readonly List<GameObject> actives = new();

    void OnEnable() => StartCoroutine(SpawnLoop());
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

        tmp.enableVertexGradient = false;
        tmp.colorGradientPreset = null;
        tmp.overrideColorTags = true;

        var c = Color.HSVToRGB(Random.value, 0.9f, 1f);
        tmp.color = c;

        var instMat = new Material(tmp.fontSharedMaterial);
        instMat.SetColor(TMPro.ShaderUtilities.ID_FaceColor, c);

        if (instMat.HasProperty(TMPro.ShaderUtilities.ID_OutlineColor))
            instMat.SetColor(TMPro.ShaderUtilities.ID_OutlineColor, c);

        tmp.fontMaterial = instMat;

        tmp.enableAutoSizing = false;
        tmp.fontSize = Random.Range(minFontSize, maxFontSize);


        // Distinct color per spawn
        if (vividColors)
        {
            tmp.color = Color.HSVToRGB(Random.value, 0.85f, 1f);
        }
        else
        {
            tmp.color = new Color(Random.value, Random.value, Random.value, 1f);
        }

        // Bigger randomized font size
        tmp.enableAutoSizing = false;
        tmp.fontSize = Random.Range(minFontSize, maxFontSize);

        // Set message
        tmp.text = messages[cursor];
        cursor = (cursor + 1) % messages.Count;

        // Update layout AFTER size/text changes
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
