using UnityEngine;
[CreateAssetMenu(fileName = "ContentsOfTypeTable", menuName = "ContentsOfType")]
public class ContentsOfType : ScriptableObject
{
    public ItemOfMassive<GameObject>[] contentOfType;
}