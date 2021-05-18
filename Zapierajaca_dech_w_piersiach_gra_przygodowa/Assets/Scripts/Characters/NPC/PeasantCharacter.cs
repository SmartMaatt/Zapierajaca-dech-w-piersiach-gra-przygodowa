using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PeasantCharacter : MonoBehaviour
{
    public string peasantName;

    public Dialogue dialogue;

    public GameObject exclamationMark = null;

    public void HideExclamationMark()
    {
        if (exclamationMark)
        {
            exclamationMark.SetActive(false);
        }
    }
}
