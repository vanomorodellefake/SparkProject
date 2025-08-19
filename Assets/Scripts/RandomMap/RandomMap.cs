using NUnit.Framework;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using static TMPro.SpriteAssetUtilities.TexturePacker_JsonArray;

public class CreateFloor : MonoBehaviour
{
    [SerializeField] private GameObject wall;
    [SerializeField] private GameObject wallWithHole;
    [SerializeField] private int x;
    [SerializeField] private int y;
    [SerializeField] private int rooms;
    [SerializeField] private Details details;
    [SerializeField] private TypesOfRoom typesOfRoom;
    [SerializeField] private bool EnableStartEndRooms;

    private Dictionary<int, int> NumOfSquaresInRooms = new();
    private (int, int, int) dataLongestShotWay;
    private Dictionary<int, Dictionary<int, int>> numberOfPossibleDoors = new();
    private Dictionary<int, Dictionary<int, int>> numberOfActualDoors = new();
    private Dictionary<int, List<(int, int, int, int, bool)[]>> coordsOfPossibleDoors = new();

    private void Start()
    {
        Debug.Log("Приступаю к выполнению");
        //CreationOfRooms();
        int[,] map = GenerateMap(rooms);
        DebugMap(map);
        GenerateOfPaths(map, rooms);
        GenerateRooms(map);
        GenerateWalls(map);
        int k2 = 0;
        foreach (var qwe in NumOfSquaresInRooms)
        {
            k2+= qwe.Value;
            Debug.Log($"{qwe.Key} - {qwe.Value}");
        }
        Debug.Log($"Кол-во квадратов: {k2}");
    }

    private int[,] GenerateMap(int rooms) // 8 5 5
    {
        List<int[]> RoomsCoords = new List<int[]>();
        int[,] map = new int[y + 15, x];

        // Генерим и распределяем румы

        AdvancedRandomList<Row[]> advancedRandomList = new();
        advancedRandomList.SetAsMassive(details.details);
        List<int[,]> NEWDETAILS_LIST = Defiler(advancedRandomList.ReturnRandomListOfItems(rooms));

        for (int i = 1; i <= rooms; i++)
        {
            bool placed = false;          
            int[,] room = GenerateRandomDetail(i, NEWDETAILS_LIST);
            int roomWidth = room.GetLength(0);
            int roomHeight = room.GetLength(1);
            Debug.Log($"{roomWidth}, {roomHeight}");

            int startX = Random.Range(0, x - roomWidth+1);
            int startY = 0;

            RoomsCoords.Clear();
            for (int j = 0; j < roomWidth; j++)
                for (int l = 0; l < roomHeight; l++)
                {
                    //Debug.Log($"{x}, {initial_pos}, {j}, {y}, {l}");
                    //Debug.Log(room[j, l]);
                    if (room[j,l] != 0)
                    {
                        Debug.Log($"{startY}, {l}, {startX}, {j}");
                        //map[startY + l, startX + j] = room[j, l];
                        RoomsCoords.Add(new int[2] { startY + l, startX + j });
                    }
                }
            while(!placed)
            {
                //Debug.Log("Координаты комнаты");
                //foreach (var l in RoomsCoords)
                //{
                //    Debug.Log($"{l[0]}, {l[1]}");
                //}
                placed = true;
                foreach (var l in RoomsCoords)
                {
                    //Debug.Log($"На координате: { map[l[0], l[1]]}");
                    if (map[l[0],l[1]] != 0)
                    {
                        placed = false;
                        break;
                    }
                }
                if (placed)
                {
                    foreach (var l in RoomsCoords)
                    {
                        map[l[0], l[1]] = i;
                    }
                }
                else
                {
                    for (int c = 0; c < RoomsCoords.Count; c++)
                    {
                        RoomsCoords[c][0]++;
                    }
                }
            }
            //DebugMap(map);
            RoomPlacing(map, RoomsCoords, i);
        }

        return map;
    }

    private List<int[,]> Defiler(List<Row[]> InputMassive)
    {
        int k = 0;
        List<int[,]> OutputMassive = new List<int[,]>();
        foreach (var column in InputMassive)
        {
            k++;
            int k1 = 0;
            int x = column.Length;
            int y = column[0].cells.Length;

            //Debug.Log($"{x}, {y}");
            int[,] oneDetail = new int[x, y];

            for (int i = 0; i < x; i++)
                for (int j = 0; j < y; j++)
                {
                    //Debug.Log($"{ch}, {i}, {j}");
                    if (column[i].cells[j] == true)
                    {
                        oneDetail[i, j] = -1;
                        k1++;
                    }  
                }
            NumOfSquaresInRooms.Add(k, k1);
            OutputMassive.Add(oneDetail);
        }
        return OutputMassive; 
    }

    private int[,] RoomPlacing(int[,] map, List<int[]> RoomsCoords, int i)
    {
        bool spawned = false;
        bool zenok = false;
        bool CanGoLeft = true;
        bool CanGoRight = true;
        int WhereAreWeGoing = 0;

        while (!zenok)
        {
            Debug.Log("Карта!");
            DebugMap(map);

            if (i == 1)
            {
                zenok = true;
            }

            var shag = FallsShags(RoomsCoords, map);
            Debug.Log($"Был найден шаг вниз, равный: {shag}");
            if (shag>0)
            {
                GoDown(map, RoomsCoords, shag, i);
                WhereAreWeGoing = 0;
            }

            CanGoLeft = CheckCanGoLeft(RoomsCoords, map);
            CanGoRight = CheckCanGoRight(RoomsCoords, map);

            Debug.Log(CanGoLeft);
            Debug.Log(CanGoRight);

            if (WhereAreWeGoing == 0)
            {
                if (CanGoRight && CanGoLeft)
                {
                    WhereAreWeGoing = Random.Range(1, 3);
                }
                else if (CanGoLeft && !spawned)
                {
                    WhereAreWeGoing = 1;
                }
                else if (CanGoRight && !spawned)
                {
                    WhereAreWeGoing = 2;
                }
                else
                {
                    zenok = true;
                }
            }
            
            if (WhereAreWeGoing == 1)
            {
                if (CanGoLeft)
                    GoLeft(map, RoomsCoords, i);
                else if (!CheckRoomsAround(RoomsCoords, map))
                {
                    WhereAreWeGoing = 2;
                }
                else zenok = true;
            }
            else if (WhereAreWeGoing == 2)
            {
                if (CanGoRight)
                    GoRight(map, RoomsCoords, i);
                else if (!CheckRoomsAround(RoomsCoords, map))
                {
                    WhereAreWeGoing = 1;
                }
                else zenok = true;
            }

            spawned = true;
        }
        //Debug.LogError(CheckRoomsAround(RoomsCoords, map));
        return map;
    }
    public bool CheckRoomsAround(List<int[]> RoomCoords, int[,] map)
    {
        int roomId = map[RoomCoords[0][0], RoomCoords[0][1]]; // [0][0] y [0][1] x
        int[] zapretMassive = new int[2] { 0, roomId };
        int y = map.GetLength(0)-1;
        int x = map.GetLength(1)-1;

        //Debug.LogError("Размер мапы");
        //Debug.LogError($"x: {x} y: {y}, roomId: {roomId}, RoomCoords[0][0]: {RoomCoords[0][0]}, RoomCoords[0][1]: {RoomCoords[0][1]}, RoomCoords[1][0]: {RoomCoords[1][0]}, RoomCoords[1][1]: {RoomCoords[1][1]}");
        //Debug.LogError($"{zapretMassive[0]}, {zapretMassive[1]}");

        foreach (int[] CoordOfRoom in RoomCoords)
        {
            Debug.LogWarning($"{CoordOfRoom[0]} {CoordOfRoom[1]}");
            if (CoordOfRoom[0] != 0) 
            {
                if (!zapretMassive.Contains(map[CoordOfRoom[0] - 1, CoordOfRoom[1]]))
                {
                    //Debug.LogWarning(map[CoordOfRoom[0] - 1, CoordOfRoom[1]]);
                    return true;
                }
            }
            if (CoordOfRoom[0] != y)
            {
                if (!zapretMassive.Contains(map[CoordOfRoom[0] + 1, CoordOfRoom[1]]))
                {
                    //Debug.LogWarning(map[CoordOfRoom[0] + 1, CoordOfRoom[1]]);
                    return true;
                }
            }
            if (CoordOfRoom[1] != 0)
            {
                if (!zapretMassive.Contains(map[CoordOfRoom[0], CoordOfRoom[1] - 1]))
                {
                    //Debug.LogWarning(map[CoordOfRoom[0], CoordOfRoom[1] - 1]);
                    return true;
                }
            }
            if (CoordOfRoom[1] != x)
            {
                if (!zapretMassive.Contains(map[CoordOfRoom[0], CoordOfRoom[1] + 1]))
                {
                    //Debug.LogWarning(map[CoordOfRoom[0], CoordOfRoom[1] + 1]);
                    return true;
                }
            }         
        }
        return false;
    }
    private bool CheckCanGoLeft(List<int[]> RoomCoords, int[,] map)
    {
        Dictionary<int, int> leftface = new Dictionary<int, int>();
        foreach (int[] z in RoomCoords)
        {
            if (!leftface.ContainsKey(z[0]) || leftface[z[0]] > z[1]) leftface[z[0]] = z[1];
        }

        //Debug.Log("leftface");
        //foreach (var x in leftface)
        //{
        //    Debug.Log($"{x.Key}, {x.Value}");
        //}

        foreach (var z in leftface)
        {
            //Debug.Log($"Ищем ошибку в LeftFace {z.Key}, {z.Value}");
            if (z.Value == 0)
            {
                return false;
            }
            else if (map[z.Key, z.Value - 1] != 0)
                return false;
        }

        return true;
    }

    private bool CheckCanGoRight(List<int[]> RoomCoords, int[,] map)
    {
        Dictionary<int, int> rightface = new Dictionary<int, int>();
        foreach (int[] z in RoomCoords)
        {
            if (!rightface.ContainsKey(z[0]) || rightface[z[0]] < z[1]) rightface[z[0]] = z[1];
        }

        //Debug.Log("rightface");
        //foreach (var x in rightface)
        //{
        //    Debug.Log($"{x.Key}, {x.Value}");
        //}

        foreach (var z in rightface)
        {
            //Debug.Log($"Ищем ошибку в RightFace {z.Key}, {z.Value}, {x}");
            if (z.Value == x-1)
            {
                return false;
            }
            else if (map[z.Key, z.Value + 1] != 0)
                return false;
        }

        return true;
    }

    private int FallsShags(List<int[]> RoomCoords, int[,] map)
    {
        Dictionary<int, int> pol = new Dictionary<int, int>();
        foreach (int[] z in RoomCoords)
        {
            if (!pol.ContainsKey(z[1]) || pol[z[1]] > z[0]) pol[z[1]] = z[0];
        }

        Debug.Log("pol");
        foreach (var x in pol)
        {
            Debug.Log($"{x.Key}, {x.Value}");
        }

        int minShags = int.MaxValue;
        foreach (var j in pol)
        {
            int steps = 0;
            int y = j.Value;

            while (y > 0 && map[y - 1, j.Key] == 0)
            {
                steps++;
                y--;
            }

            if (steps < minShags)
            {
                minShags = steps;
            }
        }
        return minShags;
    }
    private void GoDown(int[,] map, List<int[]> coords, int shag, int roomNumber)
    {
        foreach (var coord in coords)
        {
            map[coord[0], coord[1]] = 0;
        }

        for (int i = 0; i < coords.Count; i++)
        {
            int newY = coords[i][0] - shag;
            map[newY, coords[i][1]] = roomNumber;   
            coords[i][0] = newY;
        }
    }
    private void GoLeft(int[,] map, List<int[]> coords, int roomNumber)
    {
        foreach (var coord in coords)
        {
            map[coord[0], coord[1]] = 0;
        }

        for (int i = 0; i < coords.Count; i++)
        {
            int newX = coords[i][1] - 1;
            map[coords[i][0], newX] = roomNumber;
            coords[i][1] = newX;
        }
    }
    private void GoRight(int[,] map, List<int[]> coords, int roomNumber)
    {
        foreach (var coord in coords)
        {
            map[coord[0], coord[1]] = 0;
        }

        for (int i = 0; i < coords.Count; i++)
        {
            int newX = coords[i][1] + 1;
            map[coords[i][0], newX] = roomNumber;
            coords[i][1] = newX;
        }
    }

    private int[,] GenerateRandomDetail(int k, List<int[,]> NEWDETAILS_LIST)
    {
        var newdetail = NEWDETAILS_LIST[k-1];

        var rotation = Random.Range(0, 4);

        switch (rotation)
        {
            case 0:
                for (int i = 0; i < newdetail.GetLength(0); i++)
                    for (int j = 0; j < newdetail.GetLength(1); j++)
                        if (newdetail[i, j] == -1) newdetail[i, j] = k;
                break;
            case 1:
                int[,] rotated90 = new int[newdetail.GetLength(1), newdetail.GetLength(0)];
                for (int i = 0; i < newdetail.GetLength(0); i++)
                    for (int j = 0; j < newdetail.GetLength(1); j++)
                        rotated90[j, newdetail.GetLength(0) - i - 1] =
                            newdetail[i, j] == -1 ? k : newdetail[i, j];
                newdetail = rotated90;
                break;
            case 2:
                int[,] rotated180 = new int[newdetail.GetLength(0), newdetail.GetLength(1)];
                for (int i = 0; i < newdetail.GetLength(0); i++)
                    for (int j = 0; j < newdetail.GetLength(1); j++)
                        rotated180[newdetail.GetLength(0) - i - 1, newdetail.GetLength(1) - j - 1] =
                            newdetail[i, j] == -1 ? k : newdetail[i, j];
                newdetail = rotated180;
                break;
            case 3:
                int[,] rotated270 = new int[newdetail.GetLength(1), newdetail.GetLength(0)];
                for (int i = 0; i < newdetail.GetLength(0); i++)
                    for (int j = 0; j < newdetail.GetLength(1); j++)
                        rotated270[newdetail.GetLength(1) - j - 1, i] =
                            newdetail[i, j] == -1 ? k : newdetail[i, j];
                newdetail = rotated270;
                break;
            default:
                for (int i = 0; i < newdetail.GetLength(0); i++)
                    for (int j = 0; j < newdetail.GetLength(1); j++)
                        if (newdetail[i, j] == -1) newdetail[i, j] = k;
                break;
        }
        for (int i = 0; i < newdetail.GetLength(0); i++)
            for (int j = 0; j < newdetail.GetLength(1); j++)
                if (newdetail[i, j] == -1) newdetail[i, j] = 0;
        DebugDetail(newdetail);
        return newdetail;
    }

    private void DebugMap(int[,] map)
    {
        for (int i = map.GetLength(0) - 1; i >= 0; i--)
        {
            string row = "";
            for (int j = 0; j < map.GetLength(1); j++)
            {
                row += map[i, j] + " ";
            }
            Debug.Log(row);
        }
    }

    private void DebugDetail(int[,] detail)
    {
        for (int i = 0; i < detail.GetLength(0); i++)
        {
            string row = "";
            for (int j = 0; j < detail.GetLength(1); j++)
            {
                row += detail[i, j] + " ";
            }
            Debug.Log(row);
        }
    }

    private void GenerateRooms(int[,] map)
    {
        Dictionary<int, TypeOfRoom> DistributionDictionary = DistributionOfRooms(rooms);

        List<List<GameObject>> ContentPlacingList = CreateContentOfRooms(rooms, DistributionDictionary);
        List<List<GameObject>> floorsPlacingList = CreateFloorsContentOfRooms(rooms, DistributionDictionary);
        List<List<GameObject>> wallsPlacingList = CreateWallsContentOfRooms(rooms, DistributionDictionary);

        int[] counterContent = new int[ContentPlacingList.Count];
        int[] counterFloors = new int[floorsPlacingList.Count];
        int[] counterWalls = new int[wallsPlacingList.Count];

        foreach (var content in ContentPlacingList)
        {
            string text = "";
            for (int i=0; i<content.Count;i++)
            {
                text += i;
                text += " ";
                text += content[i].name;
                text += " ";
            }

            Debug.Log(text);
        }

        int rows = map.GetLength(0); // y
        int cols = map.GetLength(1); // x

        for (int i = rows - 1; i >= 0; i--)
        {
            for (int j = 0;j < cols;j++)
            {
                int currentPlace = map[i, j];

                if (currentPlace != 0)
                {
                    var txt = floorsPlacingList[currentPlace-1][counterFloors[currentPlace-1]].GetComponentInChildren<TMP_Text>();
                    txt.text = currentPlace.ToString();
                    Instantiate(floorsPlacingList[currentPlace - 1][counterFloors[currentPlace - 1]], new Vector3(j * 5, 0, i * 5), Quaternion.Euler(0, 0, 0), transform);
                    counterFloors[currentPlace - 1]++;

                    for (int x=-1; x<2; x++)
                    {
                        for (int y=-1; y<2;y++)
                        {
                            Instantiate(ContentPlacingList[currentPlace - 1][counterContent[currentPlace - 1]], new Vector3(j * 5 + x*1.5f, -10, i * 5 + y*1.5f), Quaternion.Euler(0, 0, 0), transform);
                            counterContent[currentPlace - 1]++;
                        }
                    }
                    
                    //var txt = floor.GetComponentInChildren<TMP_Text>();
                    //txt.text = currentPlace.ToString();
                    //Instantiate(floor, new Vector3(j * 5, 0, i * 5), Quaternion.Euler(0,0,0), transform);
                }
            }
        }
    }

    private Dictionary<int, TypeOfRoom> DistributionOfRooms(int rooms)
    {
        Dictionary<int, TypeOfRoom> DistributionDictionary = new();
        AdvancedRandomList<TypeOfRoom> advancedRandomList = new();
        advancedRandomList.SetAsMassive(typesOfRoom.typeOfRoomInfs);
        if (!EnableStartEndRooms)
        {
            List<TypeOfRoom> types = advancedRandomList.ReturnRandomListOfItems(rooms);
            for (int i = 1; i <= rooms; i++)
            {
                DistributionDictionary.Add(i, types[i - 1]);
            }
        }
        else
        {
            DistributionDictionary.Add(dataLongestShotWay.Item2, typesOfRoom.startRoom);
            DistributionDictionary.Add(dataLongestShotWay.Item3, typesOfRoom.endRoom);
            List<TypeOfRoom> types = advancedRandomList.ReturnRandomListOfItems(rooms-2);
            int i= 1;
            int i2 = 0;
            while (i<=rooms)
            {
                if (!DistributionDictionary.ContainsKey(i))
                {
                    DistributionDictionary.Add(i, types[i2]);
                    i++;
                    i2++;
                }
                else
                {
                    i++;
                }
            }
        }

        return DistributionDictionary;
    }
    private List<List<GameObject>> CreateContentOfRooms(int rooms, Dictionary<int, TypeOfRoom> DistributionDictionary)
    {
        var allRoomsContent = new List<List<GameObject>>();

        for (int roomNumber = 1; roomNumber <= rooms; roomNumber++)
        {
            var currentRoomType = DistributionDictionary[roomNumber];
            int currentSquaresCount = NumOfSquaresInRooms[roomNumber];
            int currentPositionsCount = currentSquaresCount * 9;

            // CONTENT

            List<ContentsOfType> contentTypesDistribution = GenerateContentTypesDistribution(currentRoomType.contentOfRoom, currentPositionsCount);
            List<GameObject> roomContent = GenerateContentObjects(contentTypesDistribution);

            allRoomsContent.Add(roomContent);
        }

        return allRoomsContent;
    }

    private List<List<GameObject>> CreateFloorsContentOfRooms(int rooms, Dictionary<int, TypeOfRoom> DistributionDictionary)
    {
        var floorsContent = new List<List<GameObject>>();

        for (int roomNumber = 1; roomNumber <= rooms; roomNumber++)
        {
            var currentRoomType = DistributionDictionary[roomNumber];
            int currentSquaresCount = NumOfSquaresInRooms[roomNumber];

            List<GameObject> contentFloors = CreateContentPool(currentRoomType.floorsOfRoom, currentSquaresCount);

            floorsContent.Add(contentFloors);
        }

        return floorsContent;
    }

    private List<List<GameObject>> CreateWallsContentOfRooms(int rooms, Dictionary<int, TypeOfRoom> DistributionDictionary)
    {
        var wallsContent = new List<List<GameObject>>();

        for (int roomNumber = 1; roomNumber <= rooms; roomNumber++)
        {
            var currentRoomType = DistributionDictionary[roomNumber];
            int currentSquaresCount = NumOfSquaresInRooms[roomNumber];

            List<GameObject> contentWalls = CreateContentPool(currentRoomType.wallsOfRoom, currentSquaresCount);

            wallsContent.Add(contentWalls);
        }

        return wallsContent;
    }

    private List<ContentsOfType> GenerateContentTypesDistribution(ItemOfMassive<ContentsOfType>[] contentOfRoom, int currentPositionsCount)
    {
        var randomizer = new AdvancedRandomList<ContentsOfType>();
        randomizer.SetAsMassive(contentOfRoom);
        return randomizer.ReturnRandomListOfItems(currentPositionsCount);
    }
    private List<GameObject> GenerateContentObjects(List<ContentsOfType> contentTypesDistribution)
    {
        var diffTypesCount = contentTypesDistribution.GroupBy(type => type).ToDictionary(group => group.Key, group => group.Count());
        var contentGroups = contentTypesDistribution.GroupBy(type => type).ToDictionary(group => group.Key, group => new { Count = group.Count(), ContentPool = CreateContentPoolWithDynamicAmount(group.Key, diffTypesCount) });
        var result = new List<GameObject>();
        foreach (var contentType in contentTypesDistribution)
        {
            var pool = contentGroups[contentType].ContentPool;
            result.Add(pool[0]);
            pool.RemoveAt(0);
        }
        return result;
    }
    private List<GameObject> CreateContentPoolWithDynamicAmount(ContentsOfType contentsOfType, Dictionary<ContentsOfType, int> diffTypesCount)
    {
        var randomizer = new AdvancedRandomList<GameObject>();
        randomizer.SetAsMassive(contentsOfType.contentOfType);
        return randomizer.ReturnRandomListOfItems(diffTypesCount[contentsOfType]);
    }
    private List<GameObject> CreateContentPool(ContentsOfType contentsOfType, int amount)
    {
        var randomizer = new AdvancedRandomList<GameObject>();
        randomizer.SetAsMassive(contentsOfType.contentOfType);
        return randomizer.ReturnRandomListOfItems(amount);
    }
    private List<List<GameObject>> CreateContentOfRooms2(int rooms, Dictionary<int, TypeOfRoom> DistributionDictionary)
    {
        List<List<GameObject>> ContentPlacingList = new();
        List<List<GameObject>> ContentFloots = new();
        
        for (int i = 1; i <= rooms; i++)
        {
            List<List<GameObject>> TimedContentPlacingList = new();
            List<GameObject> TimedContentPlacingList2 = new();
            Dictionary<ContentsOfType, int> DictToCheck = new();

            int CountOfFloors = NumOfSquaresInRooms[i];
            int CountOfPlaces = NumOfSquaresInRooms[i]*9;

            AdvancedRandomList<ContentsOfType> advancedRandomList1 = new();
            advancedRandomList1.SetAsMassive(DistributionDictionary[i].contentOfRoom);
            List<ContentsOfType> ContentTypeList = advancedRandomList1.ReturnRandomListOfItems(CountOfPlaces);

            Dictionary<ContentsOfType, int> DiffTypesCount = new();
            foreach (var qwe in ContentTypeList)
            {
                if (!DiffTypesCount.ContainsKey(qwe))
                {
                    DiffTypesCount.Add(qwe, 0);
                }
                DiffTypesCount[qwe]++;
            }
            int k3 = 0;
            foreach (var qwe in DiffTypesCount)
            {
                AdvancedRandomList<GameObject> advancedRandomList2 = new();
                advancedRandomList2.SetAsMassive(qwe.Key.contentOfType);
                List<GameObject> ContentList = advancedRandomList2.ReturnRandomListOfItems(qwe.Value);
                TimedContentPlacingList.Add(ContentList);
                DictToCheck.Add(qwe.Key, k3);
                k3++;
            }
            foreach (var qwe in ContentTypeList)
            {
                TimedContentPlacingList2.Add(TimedContentPlacingList[DictToCheck[qwe]][0]);
                TimedContentPlacingList[DictToCheck[qwe]].RemoveAt(0);
            }
            ContentPlacingList.Add(TimedContentPlacingList2);
        }

        return ContentPlacingList;
    }

    private void GenerateWalls(int[,] metrs)
    {
        int rows = metrs.GetLength(0); // y
        int cols = metrs.GetLength(1); // x

        for (int i = 0; i < cols; i++)
        {
            for (int j = 0; j < rows; j++)
            {
                int currentRoom = metrs[j, i];

                // Генерация стен вокруг
                /*
                if (i == 0)
                {
                    Instantiate(wall, new Vector3(i*5 - 0.5f, 0, j*5), Quaternion.identity, transform);
                }
                if (i == x-1)
                {
                    Instantiate(wall, new Vector3(i * 5 + 0.5f, 0, j * 5), Quaternion.identity, transform);
                }
                if (j == 0)
                {
                    Instantiate(wall, new Vector3(i * 5, 0, j*5 - 0.5f), Quaternion.Euler(0, 90, 0), transform);
                }
                if (j == y-1)
                {
                    Instantiate(wall, new Vector3(i * 5, 0, j * 5 + 0.5f), Quaternion.Euler(0, 90, 0), transform);
                }
                */
                // Генерация стен между комнат
                if (currentRoom != 0)
                {
                    if ( ( i < x - 1 && metrs[j, i + 1] == 0 ) || i == x - 1)
                    {
                        Instantiate(wall, new Vector3(i * 5 + 2.25f, 2.75f, j * 5), Quaternion.identity, transform);
                    }
                    else if (metrs[j, i + 1] != currentRoom)
                    {
                        Instantiate(wallWithHole, new Vector3(i * 5 + 2.25f, 2.75f, j * 5), Quaternion.identity, transform);
                    }
                    if ((i > 0 && metrs[j, i - 1] == 0) || i == 0)
                    {
                        Instantiate(wall, new Vector3(i * 5 - 2.25f, 2.75f, j * 5), Quaternion.identity, transform);
                    }
                    else if (metrs[j, i - 1] != currentRoom)
                    {
                        Instantiate(wallWithHole, new Vector3(i * 5 - 2.25f, 2.55f, j * 5), Quaternion.identity, transform);
                    }
                    if (j < y - 1 && metrs[j + 1, i] == 0 || j == y - 1)
                    {
                        Instantiate(wall, new Vector3(i*5, 2.75f, j*5 + 2.25f), Quaternion.Euler(0, 90, 0), transform);
                    }
                    else if (metrs[j + 1, i] != currentRoom)
                    {
                        Instantiate(wallWithHole, new Vector3(i * 5, 2.75f, j * 5 + 2.25f), Quaternion.Euler(0, 90, 0), transform);
                    }
                    if (j > 0 && metrs[j - 1, i] == 0 || j == 0)
                    {
                        Instantiate(wall, new Vector3(i * 5, 2.75f, j * 5 - 2.25f), Quaternion.Euler(0, 90, 0), transform);
                    }
                    else if (metrs[j - 1, i] != currentRoom)
                    {
                        Instantiate(wallWithHole, new Vector3(i * 5, 2.75f, j * 5 - 2.25f), Quaternion.Euler(0, 90, 0), transform);
                    }
                }
                
            }
        }
    }

    public void GenerateOfPaths(int[,] map, int amountOfRooms)
    {
        Floid floid = new Floid();
        Graph graph = new Graph();

        int rows = map.GetLength(1); // x
        int cols = map.GetLength(0); // y

        for (int roomNumber = 1; roomNumber <= amountOfRooms; roomNumber++)
        {
            graph.AddNode(roomNumber.ToString());
        }

        for (int i = 0; i < cols - 1; i++)
        {
            for (int j = 0; j < rows - 1; j++)
            {
                Debug.Log($"i: {i}, j: {j}");
                int currentRoom = map[i, j];
                if (currentRoom != 0)
                {
                    Node currentNode = graph.Search(currentRoom.ToString());
                    if (map[i + 1, j] != 0 && map[i + 1, j] != currentRoom)
                    {
                        int newRoom = map[i + 1, j];

                        UpdateRoomsInNumberOfDoors(currentRoom, newRoom);

                        numberOfPossibleDoors[currentRoom][newRoom]++;
                        numberOfPossibleDoors[newRoom][currentRoom]++;
                        Node newNode = graph.Search(newRoom.ToString());
                        if (!currentNode.Links.ContainsKey(newNode))
                        {
                            currentNode.Links.Add(newNode, 1);
                            newNode.Links.Add(currentNode, 1);
                        }
                    }
                    if (map[i, j + 1] != 0 && map[i, j + 1] != currentRoom)
                    {
                        int newRoom = map[i, j + 1];

                        UpdateRoomsInNumberOfDoors(currentRoom, newRoom);

                        numberOfPossibleDoors[currentRoom][newRoom]++;
                        numberOfPossibleDoors[newRoom][currentRoom]++;
                        Node newNode = graph.Search(newRoom.ToString());
                        if (!currentNode.Links.ContainsKey(newNode))
                        {
                            currentNode.Links.Add(newNode, 1);
                            newNode.Links.Add(currentNode, 1);
                        }
                    }
                }
            }
        }

        GetNumberOfActualDoors();
        dataLongestShotWay = floid.floid(graph);

        foreach (var qwe in graph)
        {
            var qwe1 = graph.ReturnRoads(qwe.Name);
            Debug.Log(qwe.Name);
            foreach (var qwe2 in qwe1)
            {
                Debug.Log($"{qwe2.Key.Name} - {qwe2.Value}");
            }
        }
        foreach (var qwe in numberOfPossibleDoors)
        {
            foreach (var qwe2 in qwe.Value)
            {
                Debug.Log($"Коилчество возможных дверей: {qwe.Key} - {qwe2.Key} - {qwe2.Value}");
            }
        }
        foreach (var qwe in numberOfActualDoors)
        {
            foreach (var qwe2 in qwe.Value)
            {
                Debug.Log($"Коилчество актуальных дверей: {qwe.Key} - {qwe2.Key} - {qwe2.Value}");
            }
        }
        Debug.Log($"{dataLongestShotWay.Item1} {dataLongestShotWay.Item2} {dataLongestShotWay.Item3}");
        //Debug.Log($"rows: {rows}, cols: {cols}");
        /*
        var dictOfPaths = new Dictionary<int, List<int>>();
        
        for (int i = 0; i < cols-1; i++)
        {
            for (int j = 0; j < rows-1; j++)
            {
                Debug.Log($"i: {i}, j: {j}");
                int currentRoom = map[i,j];
                if (currentRoom != 0)
                {
                    if (!dictOfPaths.ContainsKey(currentRoom))
                    {
                        dictOfPaths.Add(currentRoom, new List<int>());
                    }

                    if (map[i + 1,j] != 0 && map[i + 1,j] != currentRoom)
                    {
                        int newRoom = map[i + 1,j];
                        if (!dictOfPaths[currentRoom].Contains(newRoom))
                        {
                            dictOfPaths[currentRoom].Add(newRoom);
                            if (!dictOfPaths.ContainsKey(newRoom))
                            {
                                dictOfPaths.Add(newRoom, new List<int>());
                            }
                            dictOfPaths[newRoom].Add(currentRoom);
                        }
                    }
                    if (map[i, j + 1] != 0 && map[i, j + 1] != currentRoom)
                    {
                        int newRoom = map[i, j + 1];
                        if (!dictOfPaths[currentRoom].Contains(newRoom))
                        {
                            dictOfPaths[currentRoom].Add(newRoom);
                            if (!dictOfPaths.ContainsKey(newRoom))
                            {
                                dictOfPaths.Add(newRoom, new List<int>());
                            }
                            dictOfPaths[newRoom].Add(currentRoom);
                        }
                    }
                }
            }
        }

        Debug.Log("PATHS!");

        foreach (var qwe in dictOfPaths)
        {
            string text = $"{qwe.Key}: ";
            for (int i = 0; i < qwe.Value.Count; i++)
            {
                //text += i;
                //text += " ";
                text += qwe.Value[i];
                text += " ";
            }

            Debug.Log(text);
        }    
        int amountOfDoorsPerRoom = 1;

        var dictOfPathsWithValues = new Dictionary<int, List<int[]>>();

        foreach (var qwe in dictOfPaths)
        {
            int currentRoom = qwe.Key;
            dictOfPathsWithValues.Add(currentRoom, new List<int[]>());

            foreach (int i in qwe.Value)
            {
                dictOfPathsWithValues[currentRoom].Add(new int[2] { i, 1 });
            }

        }
        */
    }

    private void UpdateRoomsInNumberOfDoors(int currentRoom, int newRoom)
    {
        if (!coordsOfPossibleDoors.ContainsKey(currentRoom))
            coordsOfPossibleDoors.Add(currentRoom, new List<(int, int, int, int, bool)[]>());
        if (!numberOfPossibleDoors.ContainsKey(currentRoom))
            numberOfPossibleDoors.Add(currentRoom, new Dictionary<int, int>());
        if (!numberOfPossibleDoors.ContainsKey(newRoom))
            numberOfPossibleDoors.Add(newRoom, new Dictionary<int, int>());
        if (!numberOfPossibleDoors[currentRoom].ContainsKey(newRoom))
            numberOfPossibleDoors[currentRoom].Add(newRoom, 0);
        if (!numberOfPossibleDoors[newRoom].ContainsKey(currentRoom))
            numberOfPossibleDoors[newRoom].Add(currentRoom, 0);
    }

    private void GetNumberOfActualDoors()
    {
        foreach (var qwe in numberOfPossibleDoors)
        {
            if (!numberOfActualDoors.ContainsKey(qwe.Key))
                numberOfActualDoors.Add(qwe.Key, new Dictionary<int, int>());
            foreach (var qwe2 in qwe.Value)
            {
                if (numberOfActualDoors.ContainsKey(qwe2.Key))
                    continue;

                numberOfActualDoors.Add(qwe2.Key, new Dictionary<int, int>());
                int randNumber = Random.Range(1, qwe2.Value+1);
                numberOfActualDoors[qwe.Key].Add(qwe2.Key, randNumber);
                numberOfActualDoors[qwe2.Key].Add(qwe.Key, randNumber);
            }
        }
    }
}