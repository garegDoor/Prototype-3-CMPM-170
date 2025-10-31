using UnityEngine;

[System.Serializable]
public class List
{
    [TextArea(1, 3)] public string text;
    public bool isTruth = true;
    public string actionTag;
    public string[] prereqTags;
    public string[] decoyTags;
}

[CreateAssetMenu(fileName = "List", menuName = "Prototype/List")]
public class ListInstructions : ScriptableObject
{
    public List[] clauses;
}
