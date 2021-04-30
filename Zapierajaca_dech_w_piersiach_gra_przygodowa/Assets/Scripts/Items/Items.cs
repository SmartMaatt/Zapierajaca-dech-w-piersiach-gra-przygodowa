using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Items : MonoBehaviour, InteractOperator
{
    public string itemName;
    public itemType type;
    public GameObject itemPrefab;
    public enum itemType
    {
        WEAPON = 0,
        SHIELD = 1,
        POTION = 2,
        SPECIAL = 3
    }
    private GameObject _item;
    
    public void Operate()
    {
        if(Managers.Inventory.AddItem(this))
            Destroy(this.gameObject);
    }

    public void Drop(Vector3 dropPosition)
    {
        _item = Instantiate(itemPrefab) as GameObject;
        _item.transform.position = dropPosition;

        _item.GetComponent<Items>().itemName = itemName;
        _item.GetComponent<Items>().type = type;
        _item.GetComponent<Items>().itemPrefab = itemPrefab;
    }

    public void equip()
    {
        Managers.Inventory.EquipItem(this);
    }
}
