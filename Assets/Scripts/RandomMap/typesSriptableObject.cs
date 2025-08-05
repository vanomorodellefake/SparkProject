using UnityEngine;
[CreateAssetMenu(fileName = "TypeOfRoomsTable", menuName = "TypeOfRoom")]
public class TypesOfRoom : ScriptableObject
{
    public ItemOfMassive<TypeOfRoom>[] typeOfRoomInfs;
}
[System.Serializable]
public class TypeOfRoom
{
    public string name;
    public GameObject floor;
    public ItemOfMassive<ContentsOfType>[] contentOfRoom;
}