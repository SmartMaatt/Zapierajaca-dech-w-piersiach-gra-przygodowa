using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class QuestManager : MonoBehaviour, IGameManager
{
    public ManagerStatus status { get; private set; }

    public GameObject questArea = null;
    public GameObject questSlotPrefab = null;
    public GameObject descriptionPopup = null;
    public TMP_Text descriptionText = null;
    public List<GameObject> questSlots = null;

    [Header("UI elements")]
    public ElementFade questStarted = null;
    public ElementFade questEnd = null;
    public ElementFade popUp = null;

    public List<GetItemQuest> getItemQuests;
    public List<KillQuest> killQuests;
    public List<TalkToQuest> talkToQuests;
    public List<VisitQuest> visitQuests;
    public List<SetQuest> questsToSet;
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
        foreach (SetQuest setQuest in questsToSet)
        {
            if (dialogue == setQuest.dialogue)
            {
                return true;
            }
        }
        return false;
    }

    public void SetQuest(Dialogue dialogue)
    {
        foreach (SetQuest setQuest in questsToSet)
        {
            if (dialogue == setQuest.dialogue)
            {
                switch (setQuest.quest.questType)
                {
                    case Quest.QuestType.GETITEM:
                        GetItemQuest tmpGetItemQuest = (GetItemQuest)setQuest.quest;
                        Managers.Quest.getItemQuests.Add(tmpGetItemQuest);
                        break;
                    case Quest.QuestType.KILL:
                        KillQuest tmpKillQuest = (KillQuest)setQuest.quest;
                        Managers.Quest.killQuests.Add(tmpKillQuest);
                        break;
                    case Quest.QuestType.TALKTO:
                        TalkToQuest tmpTalkToQuest = (TalkToQuest)setQuest.quest;
                        Managers.Quest.talkToQuests.Add(tmpTalkToQuest);
                        break;
                    case Quest.QuestType.VISIT:
                        break;
                }
                Managers.Quest.QuestStart();
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
        slot.GetComponent<Button>().onClick.AddListener(delegate { ShowDescription(quest); });
        slot.transform.SetParent(questArea.transform);
        questSlots.Add(slot);
        if (questSlots.Count >= 7)
        {
            questArea.GetComponent<RectTransform>().offsetMin = new Vector2(questArea.GetComponent<RectTransform>().offsetMin.x, questArea.GetComponent<RectTransform>().offsetMin.y - 110);
        }
    }

    public void ShowDescription(Quest quest)
    {
        descriptionPopup.SetActive(true);
        descriptionText.text = quest.description;
        switch (quest.questType)
        {
            case Quest.QuestType.GETITEM:
                GetItemQuest tmpGetItemQuest = (GetItemQuest)quest;
                descriptionText.text += "<br>Postęp: " + tmpGetItemQuest.questItemsCounter + "/" + tmpGetItemQuest.questItemsNumber;
                break;
            case Quest.QuestType.KILL:
                KillQuest tmpKillQuest = (KillQuest)quest;
                descriptionText.text += "<br>Postęp: " + tmpKillQuest.questKillsCounter + "/" + tmpKillQuest.questKillsNumber;
                break;
            case Quest.QuestType.TALKTO:
                break;
            case Quest.QuestType.VISIT:
                break;
        }
    }

    public void HideDescription()
    {
        descriptionPopup.SetActive(false);
    }

    public void QuestStart()
    {
        questStarted.RunFace(3f, 1f);
        Managers.Player.getAudioManager().Play("Heal");
    }

    public void QuestEnd(int gold, int exp)
    {
        questEnd.RunFace(3f, 1f);
        popUp.gameObject.transform.GetChild(0).gameObject.GetComponent<Text>().text = "Złoto: " + gold + "\nExp: " + exp;
        popUp.RunFace(3f, 1f);
        Managers.Player.getAudioManager().Play("Heal");
    }
}
