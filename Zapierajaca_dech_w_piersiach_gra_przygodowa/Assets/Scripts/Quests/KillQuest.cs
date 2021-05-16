using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Quest", menuName = "Own/Quest/KillQuest")]
public class KillQuest : Quest
{
    public string questEnemyName;
    public int questKillsNumber;
    public int questKillsCounter = 0;

    public bool CheckQuest(string enemyName)
    {
        if (enemyName == questEnemyName)
        {
            questKillsCounter++;
            if (questKillsCounter == questKillsNumber)
            {
                FinishQuest();
            }
            return true;
        }
        return false;
    }
}
