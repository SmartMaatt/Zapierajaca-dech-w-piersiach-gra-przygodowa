using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InventoryManager : MonoBehaviour, IGameManager
{
    public ManagerStatus status { get; private set; }
    public string equippedItem { get; private set; }

    private Dictionary<string, int> _items;

    public void Startup()
    {
        Debug.Log("Uruchomienie menadżera magazynu...");
        _items = new Dictionary<string, int>();
        status = ManagerStatus.Started;
    }

    private void DisplayItems()
    {
        string itemDisplay = "Przedmioty: ";
        foreach (KeyValuePair<string, int> item in _items)
            itemDisplay += item.Key + "(" + item.Value + ") ";
        Debug.Log(itemDisplay);
    }

    public List<string> GetItemList()
    {
        List<string> list = new List<string>(_items.Keys);
        return list;
    }

    public int GetItemCount(string name)
    {
        if (_items.ContainsKey(name))
            return _items[name];
        return 0;
    }

    public void AddItem(string name)
    {
        if (_items.ContainsKey(name))
            _items[name] += 1;
        else
            _items[name] = 1;
        DisplayItems();
    }

    public bool EquipItem(string name)
    {
        if(_items.ContainsKey(name) && equippedItem != name)
        {
            equippedItem = name;
            return true;
        }
        equippedItem = null;
        return false;
    }

    public bool ConsumeItem(string name)
    {
        if(_items.ContainsKey(name))
        {
            _items[name]--;
            if (_items[name] == 0)
                _items.Remove(name);
        }
        else
        {
            Debug.Log("Nie można użyć przedmiotu " + name);
            return false;
        }
        DisplayItems();
        return true;
    }
}
