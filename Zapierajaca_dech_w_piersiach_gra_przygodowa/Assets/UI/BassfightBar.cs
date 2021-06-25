using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BassfightBar : MonoBehaviour
{
    Mage _bossObj;
    public UIBar healthBar;

    void Start()
    {
        if (FindObjectsOfType<Mage>().Length > 0)
        {
            _bossObj = FindObjectsOfType<Mage>()[0].transform.GetComponent<Mage>();
        }
        else
        {
            gameObject.SetActive(false);
        }
    }
}
