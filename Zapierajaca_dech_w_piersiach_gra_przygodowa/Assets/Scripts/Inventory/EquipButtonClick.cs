using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EquipButtonClick : MonoBehaviour
{
    public Items item;
    public GameObject icon;
    public GameObject player;

    public bool iconActiv = false;
    public void OnClick()
    {
        item.equip();
        iconActiv = !iconActiv;
        icon.SetActive(iconActiv);
    }

    public void Drop()
    {
        if(iconActiv)
            item.equip();

        item.Drop(player.transform.position, true);
        this.transform.SetParent(null);
        Destroy(this.gameObject);
    }
}
