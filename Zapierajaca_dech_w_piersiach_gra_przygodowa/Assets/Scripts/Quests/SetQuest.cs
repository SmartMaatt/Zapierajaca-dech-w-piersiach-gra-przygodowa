using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Quest", menuName = "Own/Quest/SetQuest")]
public class SetQuest : ScriptableObject
{
    public Dialogue dialogue;
    public Quest quest;
}
