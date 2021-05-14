using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EquipButtonClick : MonoBehaviour
{
    public int itemIndex;
    public GameObject icon;
    public GameObject player;

    public bool iconActiv = false;

    public void OnClick()
    {
        Managers.Inventory.ItemsPrefabs[itemIndex].GetComponent<Items>().equip();
        iconActiv = !iconActiv;
        icon.SetActive(iconActiv);
    }

    public void Drop()
    {
        Managers.Inventory.ItemsPrefabs[itemIndex].GetComponent<Items>().Drop(player.transform.position, true);

        Debug.Log(Managers.Inventory.GetItemCount(itemIndex));
        if (Managers.Inventory.GetItemCount(itemIndex) < 1)
        {
            if (iconActiv)
                Managers.Inventory.ItemsPrefabs[itemIndex].GetComponent<Items>().equip();

            this.transform.SetParent(null);
            Destroy(this.gameObject);
        }
    }
}
