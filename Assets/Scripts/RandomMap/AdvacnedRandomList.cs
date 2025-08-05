using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using static UnityEditor.Progress;

public class AdvancedRandomList<T>
{
    private (ItemOfMassive<T> detail, int Times, int Value) olddetail = new();
    private ItemOfMassive<T>[] MassiveOfObjects;
    private List<ItemOfMassive<T>> max_amounts_details = new();
    private List<ItemOfMassive<T>> min_amount_details = new();
    private int bug_check_min = 0;
    private int bug_check_max = 0;
    private int details_with_max = 0;
    public int Amount;
    private bool only_min_remain;

    public void SetAsMassive(ItemOfMassive<T>[] NewMassive )
    {
        MassiveOfObjects = NewMassive;
    }
    /*
    public void LoadFromInterface<U>(U[] items) where U : IItemOfMassive<T>
    {
        MassiveOfObjects = items
            .Select(item => new ItemOfMassive<T>
            {
                Value = item.Value,
                weight = item.Weight,
                max_amount = item.Max_amount,
                min_amount = item.Min_amount
            })
            .ToArray();
    }
    */
    public List<T> ReturnRandomListOfItems(int enter_amount)
    {
        Amount = enter_amount;
        only_min_remain = false;
        
        //Column[] details_copy = (Column[])details.Clone(); 

        /*
        ItemOfMassive<T>[] details_copy = new ItemOfMassive<T>[MassiveOfObjects.Length]; // Создание копии
        for (int INIT = 0; INIT < MassiveOfObjects.Length; INIT++)
        {
            details_copy[INIT] = new ItemOfMassive<T>();
            details_copy[INIT].Value = MassiveOfObjects[INIT].Value;
            details_copy[INIT].weight = MassiveOfObjects[INIT].weight;
            details_copy[INIT].min_amount = MassiveOfObjects[INIT].min_amount;
            details_copy[INIT].max_amount = MassiveOfObjects[INIT].max_amount;
        }
        */

        List<ItemOfMassive<T>> details_copy = new();
        foreach (var _item in MassiveOfObjects)
        {
            details_copy.Add(new ItemOfMassive<T> { Value = _item.Value, weight = _item.weight, min_amount = _item.min_amount, max_amount = _item.max_amount });
        }
        ///////////////////////////////////////////////////////////////////////

        
        if (CheckErrors(details_copy))
        {
            int checker = 1;
            List<T> ITOG_LIST_DETAILS = new List<T>();
            while (checker <= Amount)
            {
                Debug.Log("Начинаю попытку получения комнаты");
                ItemOfMassive<T> detail = TryGetRoom(details_copy, checker);
                
                
                ///////////////////////////////////////////////////////////////////////

                if (detail == null)
                    Debug.Log("ПУСТАЯ ДЕТАЛЬ");
                else
                    Debug.Log($"Получена деталь с весом {detail.weight}");

                ///////////////////////////////////////////////////////////////////////

                if (detail != null)
                {

                    ///////////////////////////////////////////////////////////////////////

                    //if (!only_min_remain && details_copy.Count>1)
                    //{
                        
                    //}

                    checker++;
                    ITOG_LIST_DETAILS.Add(detail.Value);
                }
            }

            return ITOG_LIST_DETAILS;

            ///////////////////////////////////////////////////////////////////////
        }
        return null;
    }
    public void UpdateOldDetail(ItemOfMassive<T> item)
    {
        if (item.Equals(olddetail.detail))
        {
            item.weight = olddetail.Value;
            UpdateCapacity(item, olddetail);
        }
        else
        {
            if (olddetail.detail != null)
                olddetail.detail.weight = olddetail.Value;
            olddetail.detail = item;
            olddetail.Times = 0;
            olddetail.Value = item.weight;
            UpdateCapacity(item, olddetail);
        }
    }
    public ItemOfMassive<T> TryGetRoom(List<ItemOfMassive<T>> MassiveToGet, int checker)
    {
        bool we_getted_room = false;
        ItemOfMassive<T> item = null;
        while (!we_getted_room)
        {
            we_getted_room = true;
            if (!only_min_remain)
            {
                if (MassiveToGet.Count == 0) break;
                item = GetRandomDetail(MassiveToGet);
            }
            else
            {
                if (min_amount_details.Count == 0) break;
                item = GetRandomDetail(min_amount_details);
            }

            if (item == null)
            {
                Debug.LogError("Произошёл пиздос");
            }

            /*
            if (max_amounts_details.Contains(item))
            {
                if (item.max_amount == 0)
                {
                    we_getted_room = false;
                    continue;
                }
            }
            */

            if (!min_amount_details.Contains(item))
            {
                if ((Amount - checker + 1) == bug_check_min)
                {
                    only_min_remain = true;
                    //foreach (var det in min_amount_details)
                    //{
                    //    det.weight = 2000000;
                    //}
                    we_getted_room = false;
                    continue;
                }
            }
        }

        if (max_amounts_details.Contains(item))
        {
            item.max_amount--;
            if (item.max_amount == 0)
            {
                MassiveToGet.Remove(item);
            }
        }
            
        if (min_amount_details.Contains(item))
        {
            item.min_amount--;
            bug_check_min--;
            if (item.min_amount == 0) min_amount_details.Remove(item);
        }
        if (MassiveToGet.Count > 1)
            UpdateOldDetail(item);
        return item;
    }
    public bool CheckErrors(List<ItemOfMassive<T>> MassiveToCheck)
    {
        foreach (var det in MassiveToCheck) // заранее собираем детали с минимальным/максимальным количеством
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
        if (bug_check_min > Amount)
        {
            //throw new System.Exception("Min amount of rooms more than count of rooms!");
            Debug.Log("Can't create more rooms as they are too limited by max amount!");
            Debug.LogError("Can't create more rooms as they are too limited by max amount!");
            return false;
        }
        else if (details_with_max == Amount)
        {
            if (bug_check_max < Amount)
            {
                //throw new System.Exception("Cam't create more rooms as they are too limited by max amount!");
                Debug.Log("Can't create more rooms as they are too limited by max amount!");
                Debug.LogError("Can't create more rooms as they are too limited by max amount!");
                return false;
            }
        }
        return true;
    }

    public ItemOfMassive<T> GetRandomDetail(List<ItemOfMassive<T>> details_copy)
    {
        if (details_copy == null || details_copy.Count == 0) return null;
        if (details_copy.Count == 1) return details_copy[0];

        int sumWeight = details_copy.Sum(x => x.weight);
        if (sumWeight == 0) return null;
        int randomValue = UnityEngine.Random.Range(1, sumWeight + 1);

        int currentWeight = 0;
        foreach (var detail in details_copy)
        {
            currentWeight += detail.weight;
            if (randomValue <= currentWeight)
            {
                return detail;
            }
        }
        Debug.Log(currentWeight);
        Debug.Log(sumWeight);
        Debug.Log(randomValue);
        return null;
    }

    private void UpdateCapacity(ItemOfMassive<T> item, (ItemOfMassive<T> detail, int Times, int Value) olddetail)
    {
        olddetail.Times++;

        int totalCapacity = MassiveOfObjects.Sum(x => x.weight);
        int capacityWithoutItem = totalCapacity - olddetail.Value;

        float f = MathF.Pow(olddetail.Value, olddetail.Times - 1) * capacityWithoutItem;
        float checker = MathF.Pow(totalCapacity, olddetail.Times) - MathF.Pow(olddetail.Value, olddetail.Times);
        f /= checker;
        item.weight = Convert.ToInt32(item.weight * f);
        if (item.weight == 0) item.weight = 1;
    }
}

[System.Serializable]
public class ItemOfMassive<T>
{
    public string name;
    public T Value;
    public int weight;
    public int max_amount;
    public int min_amount;
}