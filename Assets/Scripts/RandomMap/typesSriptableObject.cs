using UnityEngine;
[CreateAssetMenu(fileName = "TypeOfRoomsTable", menuName = "TypeOfRoom")]
public class TypesOfRoom : ScriptableObject
{
    public ItemOfMassive<TypeOfRoom>[] typeOfRoomInfs;
    public TypeOfRoom startRoom;
    public TypeOfRoom endRoom;
}
[System.Serializable]
public class TypeOfRoom
{
    //public GameObject floor;
    public ContentsOfType floorsOfRoom;
    public ContentsOfType wallsOfRoom;
    public ItemOfMassive<ContentsOfType>[] contentOfRoom;
}