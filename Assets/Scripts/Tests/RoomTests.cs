using Unity.VisualScripting;
using UnityEngine;
using NUnit.Framework;
using System.Collections.Generic;

public class RoomTets
{
    [Test]
    public void JustChecker()
    {
        AdvancedRandomList<int> advancedRandomList = new();
        List<ItemOfMassive<int>> itemOfMassiveList = new();
        advancedRandomList.Amount = 5;
        for (int i = 0; i<5; i++)
        {
            ItemOfMassive<int> itemOfMassive = new ItemOfMassive<int>();
            itemOfMassive.Value = i;
            itemOfMassive.weight = 5;
            itemOfMassive.min_amount = 2;
            itemOfMassive.max_amount = 2;
        }
        Assert.AreEqual(true, advancedRandomList.CheckErrors(itemOfMassiveList));
    }
}
