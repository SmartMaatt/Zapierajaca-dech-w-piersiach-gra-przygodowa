using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class QuestManager : MonoBehaviour, IGameManager
{
    public ManagerStatus status { get; private set; }

    public GameObject questArea = null;
    public GameObject questSlotPrefab = null;
    public List<GameObject> questSlots = null;

    public List<GetItemQuest> getItemQuests;
    public List<KillQuest> killQuests;
    public List<TalkToQuest> talkToQuests;
    public List<VisitQuest> visitQuests;
    public List<Quest> questToSet;
    public List<Quest> questsToReset;

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

    public bool DoesThisDialogueInQuestExist(Dialogue dialogue)
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

    public bool CanISetThisQuest(Dialogue dialogue)
    {
        foreach (TalkToQuest quest in questToSet)
        {
            if (dialogue == quest.questDialogue)
            {
                return true;
            }
        }
        return false;
    }

    public void SetQuest(Dialogue dialogue)
    {
        foreach (TalkToQuest quest in questToSet)
        {
            if (dialogue == quest.questDialogue)
            {
                Managers.Quest.talkToQuests.Add(quest);
            }
        }
    }

    public void ResetAllQuests()
    {
        foreach (Quest quest in questsToReset)
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

    public void ShowQuests()
    {
        int counter = questSlots.Count;
        foreach (GameObject slot in questSlots)
        {
            if (counter >= 7)
            {
                questArea.GetComponent<RectTransform>().offsetMin = new Vector2(questArea.GetComponent<RectTransform>().offsetMin.x, questArea.GetComponent<RectTransform>().offsetMin.y + 110);
            }
            Destroy(slot);
            counter--;
        }
        questSlots.Clear();

        foreach (TalkToQuest quest in talkToQuests)
        {
            AddSlot(quest);
        }

        foreach (KillQuest quest in killQuests)
        {
            AddSlot(quest);
        }

        foreach (GetItemQuest quest in getItemQuests)
        {
            AddSlot(quest);
        }
    }

    private void AddSlot(Quest quest)
    {
        GameObject slot = Instantiate(questSlotPrefab);
        GameObject text = slot.transform.GetChild(0).gameObject;
        text.GetComponent<UnityEngine.UI.Text>().text = quest.title;
        slot.transform.SetParent(questArea.transform);
        questSlots.Add(slot);
        if (questSlots.Count >= 7)
        {
            questArea.GetComponent<RectTransform>().offsetMin = new Vector2(questArea.GetComponent<RectTransform>().offsetMin.x, questArea.GetComponent<RectTransform>().offsetMin.y - 110);
        }
    }
}
