using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Quest : ScriptableObject
{
    public bool active = true;
    public string title;
    public string description;
    public QuestType questType;
    public int goldReward;
    public int experienceReward;
    public List<int> itemsRewards;
    public Quest nextQuest;

    public void FinishQuest()
    {
        if(nextQuest)
        {
            switch(nextQuest.questType)
            {
                case QuestType.GETITEM:
                    Managers.Quest.getItemQuests.Add((GetItemQuest)nextQuest);
                    break;
                case QuestType.KILL:
                    Managers.Quest.killQuests.Add((KillQuest)nextQuest);
                    break;
                case QuestType.TALKTO:
                    Managers.Quest.talkToQuests.Add((TalkToQuest)nextQuest);
                    break;
                case QuestType.VISIT: 
                    Managers.Quest.visitQuests.Add((VisitQuest)nextQuest);
                    break;
            }
        }
        else
        {
            Managers.Quest.QuestEnd(goldReward, experienceReward);
        }

        Managers.Player.changeMoney(goldReward);
        Managers.Player.changeExp(experienceReward);

        foreach (int itemID in itemsRewards)
        {
            Managers.Inventory.AddItem(itemID);
        }
        switch (questType)
        {
            case QuestType.GETITEM:
                Managers.Quest.getItemQuests.Remove((GetItemQuest)this);
                break;
            case QuestType.KILL:
                Managers.Quest.killQuests.Remove((KillQuest)this);
                break;
            case QuestType.TALKTO:
                Managers.Quest.talkToQuests.Remove((TalkToQuest)this);
                break;
            case QuestType.VISIT:
                Managers.Quest.visitQuests.Remove((VisitQuest)this);
                break;
        }
    }

    public enum QuestType
    {
        GETITEM = 0,
        KILL = 1,
        TALKTO = 2,
        VISIT = 3
    }
}
