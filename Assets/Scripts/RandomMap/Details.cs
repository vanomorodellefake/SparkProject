using UnityEngine;
[CreateAssetMenu(fileName = "DetailsTable", menuName = "Detail")]
public class Details : ScriptableObject
{
    public Column[] details;


    /*
    public List<int[,]> ReturnRandomDetailList(int rooms_amount)
    {
        List<Column> max_amounts_details = new();
        List<Column> min_amount_details = new();
        int bug_check_min = 0;
        bool only_min_remain = false;
        (Column detail, int Times, int Value) olddetail = new();
        //Column[] details_copy = (Column[])details.Clone(); 

        Column[] details_copy = new Column[details.Length];
        for (int INIT=0; INIT<details.Length;INIT++)
        {
            details_copy[INIT] = new Column();
            details_copy[INIT].columns = details[INIT].columns;
            details_copy[INIT].weight = details[INIT].weight;
            details_copy[INIT].min_amount = details[INIT].min_amount;
            details_copy[INIT].max_amount = details[INIT].max_amount;
        }

        ///////////////////////////////////////////////////////////////////////

        int bug_check_max = 0;
        int details_with_max = 0;
            
        foreach (var det in details_copy) // заранее собираем детали с минимальным/максимальным количеством
        {
            if (det.max_amount > 0)
            {
                max_amounts_details.Add(det);
                bug_check_max++;
                details_with_max++;
            }
            if (det.min_amount > 0)
            {
                min_amount_details.Add(det);
                bug_check_min += det.min_amount;
            }
        }
        if (bug_check_min > rooms_amount)
        {
            //throw new System.Exception("Min amount of rooms more than count of rooms!");
            Debug.Log("Cam't create more rooms as they are too limited by max amount!");
            Debug.LogError("Cam't create more rooms as they are too limited by max amount!");
            return null;
        }
        else if (details_with_max == rooms_amount)
        {
            if (bug_check_max < rooms_amount)
            {
                //throw new System.Exception("Cam't create more rooms as they are too limited by max amount!");
                Debug.Log("Cam't create more rooms as they are too limited by max amount!");
                Debug.LogError("Cam't create more rooms as they are too limited by max amount!");
                return null;
            }
        }    
        else
        {
            int checker = 1;
            List<int[,]> ITOG_LIST_DETAILS = new List<int[,]>();
            while (checker <= rooms_amount)
            {
                Debug.Log("Начинаю попытку получения комнаты");
                Column detail = null;
                bool we_getted_room = false;
                while (!we_getted_room)
                {
                    we_getted_room = true;
                    detail = GetRandomDetail(details_copy);
                    if (max_amounts_details.Contains(detail))
                    {
                        if (detail.max_amount == 0)
                        {
                            we_getted_room = false;
                            continue;
                        }
                    }
                    if (!min_amount_details.Contains(detail))
                    {
                        if ((rooms_amount - checker + 1) == bug_check_min)
                        {
                            only_min_remain = true;
                            foreach (var det in min_amount_details)
                            {
                                det.weight = 2000000;
                            }
                            we_getted_room = false;
                            continue;
                        }
                    }
                }
                ///////////////////////////////////////////////////////////////////////

                if (detail == null)
                    Debug.Log("ПУСТАЯ ДЕТАЛЬ");
                else
                    Debug.Log($"Получена деталь с весом {detail.weight}");

                ///////////////////////////////////////////////////////////////////////

                if (detail != null)
                {

                    ///////////////////////////////////////////////////////////////////////

                    if (max_amounts_details.Contains(detail)) detail.max_amount--;
                    if (min_amount_details.Contains(detail))
                    {
                        detail.min_amount--;
                        bug_check_min--;
                        if (detail.min_amount == 0) min_amount_details.Remove(detail);
                    }

                    ///////////////////////////////////////////////////////////////////////

                    if (!only_min_remain)
                    {
                        if (detail.Equals(olddetail.detail))
                        {
                            detail.weight = olddetail.Value;
                            UpdateCapacity(detail, olddetail);
                        }
                        else
                        {
                            if (olddetail.detail != null)
                                olddetail.detail.weight = olddetail.Value;
                            olddetail.detail = detail;
                            olddetail.Times = 0;
                            olddetail.Value = detail.weight;
                            UpdateCapacity(detail, olddetail);
                        }
                    }



                    ///////////////////////////////////////////////////////////////////////

                    int x = detail.columns.Length;
                    int y = detail.columns[0].cells.Length;

                    //Debug.Log($"{x}, {y}");
                    int[,] ITOG_MASSIVE_DETAIL = new int[x, y];

                    for (int i = 0; i < x; i++)
                        for (int j = 0; j < y; j++)
                        {
                            //Debug.Log($"{ch}, {i}, {j}");
                            if (detail.columns[i].cells[j] == true)
                                ITOG_MASSIVE_DETAIL[i, j] = -1;
                        }

                    checker++;
                    ITOG_LIST_DETAILS.Add(ITOG_MASSIVE_DETAIL);
                }
            }

            return ITOG_LIST_DETAILS;

                ///////////////////////////////////////////////////////////////////////
        }
        return null;


    }

    public Column GetRandomDetail(Column[] details_copy)
    {
        if (details_copy == null || details_copy.Length == 0) return null;

        int sumWeight = details_copy.Sum(x => x.weight);
        if (sumWeight == 0) return null;
        int randomValue = UnityEngine.Random.Range(1, sumWeight + 1);

        int currentWeight = 0;
        foreach (var detail in details_copy)
        {
            currentWeight += detail.weight;
            if (randomValue < currentWeight)
            {
                return detail;
            }
        }

        return null;
    }

    private void UpdateCapacity(Column item, (Column detail, int Times, int Value) olddetail)
    {
        olddetail.Times++;
        
        int totalCapacity = details.Sum(x => x.weight);
        int capacityWithoutItem = totalCapacity - olddetail.Value;

        float f = MathF.Pow(olddetail.Value, olddetail.Times - 1) * capacityWithoutItem;
        float checker = MathF.Pow(totalCapacity, olddetail.Times) - MathF.Pow(olddetail.Value, olddetail.Times);
        f /= checker;
        item.weight = Convert.ToInt32(item.weight * f);
    }

    */
}



[System.Serializable]
public class Column : IItemOfMassive<Row[]>
{
    public Row[] Value => columns;
    public int Weight => weight;
    public int Max_amount => max_amount;
    public int Min_amount => min_amount;

    public Row[] columns;
    public int weight;
    public int max_amount;
    public int min_amount;
}

[System.Serializable]
public class Row
{
    public bool[] cells;
}