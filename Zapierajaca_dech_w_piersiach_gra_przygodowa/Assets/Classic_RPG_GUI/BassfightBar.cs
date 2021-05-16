using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BassfightBar : MonoBehaviour
{
    Mage _bossObj;
    public UIBar healthBar;

    void Start()
    {
        _bossObj = FindObjectsOfType<Mage>()[0].transform.GetComponent<Mage>();

        if (_bossObj == null)
            gameObject.SetActive(false);
    }
}
