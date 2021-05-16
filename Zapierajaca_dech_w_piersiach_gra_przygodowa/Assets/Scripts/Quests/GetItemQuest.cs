using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Quest", menuName = "Own/Quest/GetItemQuest")]
public class GetItemQuest : Quest
{
    public string questItemName;
    public int questItemsNumber;
    public int questItemsCounter = 0;

    public bool CheckQuest(string itemName)
    {
        if (itemName == questItemName)
        {
            questItemsCounter++;
            if (questItemsCounter == questItemsNumber)
            {
                FinishQuest();
            }
            return true;
        }
        return false;
    }
}
