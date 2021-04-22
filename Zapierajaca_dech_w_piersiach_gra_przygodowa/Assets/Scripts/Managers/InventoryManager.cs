using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InventoryManager : MonoBehaviour, IGameManager
{
    public ManagerStatus status { get; private set; }
    public string equippedItem { get; private set; }
    public string equippedWeapon { get; private set; }
    public string equippedShield { get; private set; }
    public string equippedPotion { get; private set; }
    public string equippedSpecial { get; private set; }

    private Dictionary<Items, int> _items;

    public void Startup()
    {
        Debug.Log("Uruchomienie menadżera magazynu...");
        _items = new Dictionary<Items, int>();
        status = ManagerStatus.Started;
    }

    private void DisplayItems()
    {
        string itemDisplay = "Przedmioty: ";
        foreach (KeyValuePair<Items, int> item in _items)
            itemDisplay += item.Key.itemName + "(" + item.Value + ") ";
        Debug.Log(itemDisplay);
    }

    public List<Items> GetItemList()
    {
        List<Items> list = new List<Items>(_items.Keys);
        return list;
    }

    public int GetItemCount(Items searchedItem)
    {
        int suma = 0;
        foreach (Items item in _items.Keys)
        {
            if (item.itemName == searchedItem.itemName && item.type == searchedItem.type)
                suma++;
        }
        return suma;
    }

    public void AddItem(Items item)
    {
        if (_items.ContainsKey(item))
            _items[item] += 1;
        else
            _items[item] = 1;
        DisplayItems();
    }

    public bool EquipItem(Items item)
    {
        switch(item.type)
        {
            case Items.itemType.WEAPON:
                if (_items.ContainsKey(item) && equippedWeapon != item.itemName)
                {
                    equippedWeapon = item.itemName;
                    return true;
                }
                equippedWeapon = null;
                return false;
            case Items.itemType.SHIELD:
                if (_items.ContainsKey(item) && equippedShield != item.itemName)
                {
                    equippedShield = item.itemName;
                    return true;
                }
                equippedShield = null;
                return false;
            case Items.itemType.POTION:
                if (_items.ContainsKey(item) && equippedPotion != item.itemName)
                {
                    equippedPotion = item.itemName;
                    return true;
                }
                equippedPotion = null;
                return false;
            case Items.itemType.SPECIAL:
                if (_items.ContainsKey(item) && equippedSpecial != item.itemName)
                {
                    equippedSpecial = item.itemName;
                    return true;
                }
                equippedSpecial = null;
                return false;
        }
        return false;
        //if(_items.ContainsKey(item) && equippedItem != item.itemName)
        //{
        //    equippedItem = item.itemName;
        //    return true;
        //}
        //equippedItem = null;
        //return false;
    }

    public bool ConsumeItem(Items item)
    {
        if(_items.ContainsKey(item))
        {
            _items[item]--;
            if (_items[item] == 0)
                _items.Remove(item);
        }
        else
        {
            Debug.Log("Nie można użyć przedmiotu " + name);
            return false;
        }
        DisplayItems();
        return true;
    }

    public Items getItem(string name)
    {
        foreach(Items item in _items.Keys)
        {
            if (item.itemName == name)
                return item;
        }
        return null;
    }
}
