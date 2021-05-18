using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Dialogue", menuName = "Own/Dialogue")]
public class Dialogue : ScriptableObject
{
    [TextArea(3, 10)]
    public string sentence;
    public List<Response> responses;
    public bool canExit;

    [Serializable]
    public class Response
    {
        [TextArea(3, 10)]
        public string sentence;
        public Talkable talkable;
        public Dialogue nextDialogue;
    }

    public enum Talkable
    {
        INFINITE = 0,
        ONCE = 1,
        QUEST = 2,
        DONE = 3,
        SETQUEST = 4
    }
}


