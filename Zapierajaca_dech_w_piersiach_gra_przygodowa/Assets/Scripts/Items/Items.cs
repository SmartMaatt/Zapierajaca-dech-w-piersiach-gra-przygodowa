using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Items : MonoBehaviour, InteractOperator
{
    public int id;
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
        if(Managers.Inventory.AddItem(id))
            Destroy(this.gameObject);
    }

    public void Drop(Vector3 dropPosition, bool playerDrop)
    {
        _item = Instantiate(itemPrefab) as GameObject;
        Vector3 dropRange = new Vector3(Random.Range(-2f, 2f), 0.5f, Random.Range(-2f, 2f));
        _item.transform.position = dropPosition + dropRange;

        _item.GetComponent<Items>().itemName = itemName;
        _item.GetComponent<Items>().type = type;
        _item.GetComponent<Items>().itemPrefab = itemPrefab;

        if(playerDrop)
        {
            Managers.Inventory.PlayerDrop(id);
        }
    }

    public void equip()
    {
        Managers.Inventory.EquipItem(id);
    }
}
