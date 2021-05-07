using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EquipButtonClick : MonoBehaviour
{
    public Items item;
    public GameObject icon;

    public bool iconActiv = false;
    public void OnClick()
    {
        item.equip();
        iconActiv = !iconActiv;
        icon.SetActive(iconActiv);
    }
}
