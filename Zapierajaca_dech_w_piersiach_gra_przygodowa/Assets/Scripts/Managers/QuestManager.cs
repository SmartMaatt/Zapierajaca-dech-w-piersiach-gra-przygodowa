using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class QuestManager : MonoBehaviour, IGameManager
{
    public ManagerStatus status { get; private set; }

    public List<GetItemQuest> getItemQuests;
    public List<KillQuest> killQuests;
    public List<TalkToQuest> talkToQuests;
    public List<VisitQuest> visitQuests;
    public List<Quest> allQuests;

    public void Startup()
    {
        Debug.Log("Uruchomienie menadżera questów...");
        ResetAllQuests();
        status = ManagerStatus.Started;
    }

    public void CheckTalkToQuest(Dialogue dialogue)
    {
        foreach (TalkToQuest quest in talkToQuests)
        {
            if(quest.CheckQuest(dialogue))
            {
                break;
            }
        }
    }

    public bool DoesThisDialoguInQuestExist(Dialogue dialogue)
    {
        foreach (TalkToQuest quest in talkToQuests)
        {
            if(dialogue == quest.questDialogue)
            {
                return true;
            }
        }
        return false;
    }

    public void CheckKillQuest(string enemyName)
    {
        foreach (KillQuest quest in killQuests)
        {
            if (quest.CheckQuest(enemyName))
            {
                break;
            }
        }
    }

    public void CheckGetItemQuest(string itemName)
    {
        foreach (GetItemQuest quest in getItemQuests)
        {
            if (quest.CheckQuest(itemName))
            {
                break;
            }
        }
    }

    public void ResetAllQuests()
    {
        foreach (Quest quest in allQuests)
        {
            switch (quest.questType)
            {
                case Quest.QuestType.GETITEM:
                    GetItemQuest tmpGetItemQuest = (GetItemQuest)quest;
                    tmpGetItemQuest.questItemsCounter = 0;
                    break;
                case Quest.QuestType.KILL:
                    KillQuest tmpKillQuest = (KillQuest)quest;
                    tmpKillQuest.questKillsCounter = 0;
                    break;
                case Quest.QuestType.TALKTO:
                    break;
                case Quest.QuestType.VISIT:
                    break;
            }
        }
    }
}
