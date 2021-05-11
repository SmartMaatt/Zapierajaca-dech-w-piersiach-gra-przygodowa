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

    public void Startup()
    {
        Debug.Log("Uruchomienie menadżera questów...");
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
}
