using UnityEngine;
[CreateAssetMenu(fileName = "TypeOfRoomsTable", menuName = "TypeOfRoom")]
public class TypesOfRoom : ScriptableObject
{
    public TypeOfRoomInf[] typeOfRoomInfs;
}

[System.Serializable]
public class TypeOfRoomInf : IItemOfMassive<TypeOfRoom>
{
    public TypeOfRoom Value => typeOfRoom;
    public int Weight => weight;
    public int Max_amount => max_amount;
    public int Min_amount => min_amount;

    public TypeOfRoom typeOfRoom;
    public int weight;
    public int max_amount;
    public int min_amount;
}
[System.Serializable]
public class TypeOfRoom
{
    public string name;
    public GameObject floor;
    public ContentOfRoom[] contentOfRoom;
}

[System.Serializable]
public class ContentOfRoom : IItemOfMassive<ContentsOfType>
{
    public ContentsOfType Value => content;
    public int Weight => weight;
    public int Max_amount => max_amount;
    public int Min_amount => min_amount;

    public string name;

    public ContentsOfType content;
    public int weight;
    public int max_amount;
    public int min_amount;
}
/*
[System.Serializable]
public class TypeOfRoom
{
    public string name;
    public GameObject floor;
    public Decoration[] decoration;
    public Usefulitems[] usefulitems;
    public Enemies[] enemies;
}
[System.Serializable]
public class Decoration : IItemOfMassive<GameObject>
{
    public GameObject Value => decoration;
    public int Weight => weight;
    public int Max_amount => max_amount;
    public int Min_amount => min_amount;

    public GameObject decoration;
    public int weight;
    public int max_amount;
    public int min_amount;
}
[System.Serializable]
public class Usefulitems : IItemOfMassive<GameObject>
{
    public GameObject Value => usefulitem;
    public int Weight => weight;
    public int Max_amount => max_amount;
    public int Min_amount => min_amount;

    public GameObject usefulitem;
    public int weight;
    public int max_amount;
    public int min_amount;
}
[System.Serializable]
public class Enemies : IItemOfMassive<GameObject>
{
    public GameObject Value => enemy;
    public int Weight => weight;
    public int Max_amount => max_amount;
    public int Min_amount => min_amount;

    public GameObject enemy;
    public int weight;
    public int max_amount;
    public int min_amount;
}
*/