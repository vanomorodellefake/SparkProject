using UnityEngine;
[CreateAssetMenu(fileName = "TypeOfRoomsTable", menuName = "TypeOfRoom")]
public class TypesOfRoom : ScriptableObject
{
    public ItemOfMassive<TypeOfRoom>[] typeOfRoomInfs;
}
[System.Serializable]
public class TypeOfRoom
{
    //public GameObject floor;
    public ContentsOfType floorsOfRoom;
    public ContentsOfType wallsOfRoom;
    public ItemOfMassive<ContentsOfType>[] contentOfRoom;
}