using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class ManualPageController : MonoBehaviour
{
    [Header("Data")]
    public ListInstructions clauseList;

    [Header("UI")]
    public RectTransform clauseListRoot;   // Parent with VerticalLayoutGroup
    public Button clauseButtonPrefab;      // Prefab we made earlier

    [Header("Options")]
    public bool startAllIgnored = true;    // Start with paper 'zoned out'

    // runtime state
    private HashSet<int> kept = new();                 // indices of kept clauses
    private readonly Dictionary<int, TextMeshProUGUI> labels = new();
    private readonly Dictionary<int, string> originalText = new();

    void Start()
    {
        BuildUI();
    }

    // Build the button list from the data
    void BuildUI()
    {
        // Clear any existing children (editor play/reload safety)
        for (int i = clauseListRoot.childCount - 1; i >= 0; i--)
        {
            Destroy(clauseListRoot.GetChild(i).gameObject);
        }

        labels.Clear();
        originalText.Clear();
        kept.Clear();

        for (int i = 0; i < clauseList.clauses.Length; i++)
        {
            var clause = clauseList.clauses[i];
            var btn = Instantiate(clauseButtonPrefab, clauseListRoot);
            var label = btn.GetComponentInChildren<TextMeshProUGUI>(true);

            // Save original text so we can swap back/forth
            originalText[i] = clause.text;

            // Default state
            bool initiallyKept = !startAllIgnored;
            if (initiallyKept) kept.Add(i);

            ApplyStyle(label, initiallyKept, clause.text);
            labels[i] = label;

            int idx = i; // capture loop variable
            btn.onClick.AddListener(() => ToggleClause(idx));
        }
    }

    public void ToggleClause(int idx)
    {
        if (kept.Contains(idx)) kept.Remove(idx);
        else kept.Add(idx);

        var label = labels[idx];
        var text = originalText[idx];
        ApplyStyle(label, kept.Contains(idx), text);

        // For future: emit an event so world can react
        // OnClauseToggled?.Invoke(clauseList.clauses[idx], kept.Contains(idx));
    }

    void ApplyStyle(TextMeshProUGUI label, bool isKept, string original)
    {
        if (isKept)
        {
            label.text = original;
            label.fontStyle = FontStyles.Bold;
            label.color = Color.white;
        }
        else
        {
            label.text = MakeBlah(original.Length);
            label.fontStyle = FontStyles.Strikethrough;
            label.color = new Color(1, 1, 1, 0.6f);
        }
    }

    string MakeBlah(int targetLen)
    {
        const string token = "blah ";
        string s = "";
        while (s.Length < targetLen) s += token;
        return s.Substring(0, targetLen);
    }

    // Optional helpers you might use later:
    public bool IsKept(int idx) => kept.Contains(idx);
    public void KeepAll(bool keep)
    {
        kept.Clear();
        if (keep) for (int i = 0; i < clauseList.clauses.Length; i++) kept.Add(i);
        // refresh labels
        for (int i = 0; i < clauseList.clauses.Length; i++)
        {
            ApplyStyle(labels[i], kept.Contains(i), originalText[i]);
        }
    }
}
