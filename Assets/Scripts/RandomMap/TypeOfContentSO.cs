using UnityEngine;
[CreateAssetMenu(fileName = "ContentsOfTypeTable", menuName = "ContentsOfType")]
public class ContentsOfType : ScriptableObject
{
    public ContentOfType[] contentOfType;
}

[System.Serializable]
public class ContentOfType : IItemOfMassive<Object>
{
    public Object Value => gameObject;
    public int Weight => weight;
    public int Max_amount => max_amount;
    public int Min_amount => min_amount;

    public string name;
    public Object gameObject;
    public int weight;
    public int max_amount;
    public int min_amount;
}