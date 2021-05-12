using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIController : MonoBehaviour
{
    [SerializeField] GameObject LeftPart;
    [SerializeField] GameObject RightPart;
    [SerializeField] GameObject DiedSprite;

   public void playerDiedScene()
    {
        LeftPart.SetActive(false);
        RightPart.SetActive(false);
        DiedSprite.SetActive(true);
    }
}
