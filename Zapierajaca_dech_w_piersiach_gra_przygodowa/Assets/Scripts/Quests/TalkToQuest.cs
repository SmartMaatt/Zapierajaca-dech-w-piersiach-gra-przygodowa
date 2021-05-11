using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Quest", menuName = "Own/Quest/TalkToQuest")]
public class TalkToQuest : Quest
{
    public Dialogue questDialogue;

    public bool CheckQuest(Dialogue dialogue)
    {
        if (dialogue == questDialogue)
        {
            FinishQuest();
            return true;
        }
        return false;
    }
}
