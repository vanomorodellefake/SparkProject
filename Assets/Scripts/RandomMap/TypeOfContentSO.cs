using UnityEngine;
[CreateAssetMenu(fileName = "ContentsOfTypeTable", menuName = "ContentsOfType")]
public class ContentsOfType : ScriptableObject
{
    public ItemOfMassive<Object>[] contentOfType;
}