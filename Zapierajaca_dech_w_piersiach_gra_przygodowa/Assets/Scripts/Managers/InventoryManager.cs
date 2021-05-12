using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class InventoryManager : MonoBehaviour, IGameManager
{
    public ManagerStatus status { get; private set; }
    public string equippedItem { get; private set; }
    public string equippedWeapon { get; private set; }
    public string equippedShield { get; private set; }
    public string equippedPotion { get; private set; }
    public string equippedSpecial { get; private set; }

    private Dictionary<Items, int> _items;

    public GameObject itemSlotPrefab;
    public GameObject InventoryView;
    public GameObject InventoryMainTitle;
    public GameObject player;

    GameObject itemSlotName;
    GameObject itemSlotCount;
    GameObject itemSlotIcon;
    GameObject itemSlotType;

    [SerializeField] private Text error;
    [SerializeField] private int inventorySize = 60;
    

    public void Startup()
    {
        Debug.Log("Uruchomienie menadżera magazynu...");
        _items = new Dictionary<Items, int>();
        error.enabled = false;
        status = ManagerStatus.Started;
        
    }

    public List<Items> GetItemList()
    {
        List<Items> list = new List<Items>(_items.Keys);
        return list;
    }

    public void ReloadCapacity()
    {
        GameObject capacity = InventoryMainTitle.transform.Find("Capacity").gameObject;
        if (capacity != null)
        {
            capacity.GetComponent<Text>().text = Managers.Inventory.GetCapacityInText();
        }
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

    public void PlayerDrop(Items item)
    {
        ConsumeItem(item);
        ReloadCapacity();
    }

    public string GetCapacityInText()
    {
        return _items.Count + " / " + inventorySize;
    }

    public bool AddItem(Items item)
    {
        if (_items.Count < inventorySize)
        {
            if (GetItemCount(item) >= 1)
            {
                string name = item.itemName + item.type + "_slot";
                GameObject ItemSlot = InventoryView.transform.Find(name).gameObject;
                if (ItemSlot != null)
                    addInventoryCount(ItemSlot, item);
                else
                {
                    adjustUI(true);
                    addInventorySlot(item);
                }
            }
            else
            {
                adjustUI(true);
                addInventorySlot(item);
            }

            if (_items.ContainsKey(item))
                _items[item] += 1;
            else
                _items[item] = 1;
            ReloadCapacity();
            return true;
        }
        else
        {
            StartCoroutine(errorPopup());
            return false;
        }
    }

    public bool EquipItem(Items item)
    {
        switch(item.type)
        {
            case Items.itemType.WEAPON:
                if (_items.ContainsKey(item) && equippedWeapon != item.itemName)
                {
                    changeEquippedItem(equippedWeapon, item);
                    equippedWeapon = item.itemName;
                    return true;
                }
                equippedWeapon = null;
                return false;
            case Items.itemType.SHIELD:
                if (_items.ContainsKey(item) && equippedShield != item.itemName)
                {
                    changeEquippedItem(equippedShield, item);
                    equippedShield = item.itemName;
                    return true;
                }
                equippedShield = null;
                return false;
            case Items.itemType.POTION:
                if (_items.ContainsKey(item) && equippedPotion != item.itemName)
                {
                    changeEquippedItem(equippedPotion, item);
                    equippedPotion = item.itemName;
                    return true;
                }
                equippedPotion = null;
                return false;
            case Items.itemType.SPECIAL:
                if (_items.ContainsKey(item) && equippedSpecial != item.itemName)
                {
                    changeEquippedItem(equippedSpecial, item);
                    equippedSpecial = item.itemName;
                    return true;
                }
                equippedSpecial = null;
                return false;
        }
        return false;
    }

    public bool ConsumeItem(Items item)
    {
        if(_items.ContainsKey(item))
        {
            _items[item]--;
            if (_items[item] == 0)
            {
                if(_items.Count > 7)
                {
                    adjustUI(false);
                }
                _items.Remove(item);
            }
        }
        else
        {
            Debug.Log("Nie można użyć przedmiotu " + name);
            return false;
        }
        return true;
    }

    public Items getItem(string name)
    {
        foreach(Items item in _items.Keys)
        {
            if (item.itemName + item.type == name)
                return item;
        }
        return null;
    }

    private IEnumerator errorPopup()
    {
        error.enabled = true;
        yield return new WaitForSeconds(2.5f);
        error.enabled = false;
    }

    private void changeEquippedItem(string itemType, Items item)
    {
        if (itemType != null)
        {
            GameObject prevEquip;
            prevEquip = InventoryView.transform.Find(itemType + item.type + "_slot").gameObject;
            bool activIcon = !prevEquip.GetComponent<EquipButtonClick>().iconActiv;
            prevEquip.GetComponent<EquipButtonClick>().iconActiv = activIcon;
            GameObject icon = prevEquip.transform.Find("Icon").gameObject;
            icon.SetActive(activIcon);
        }
    }

    private void addInventoryCount(GameObject ItemSlot, Items item)
    {
        itemSlotCount = ItemSlot.transform.Find("ItemCount").gameObject;
        if (itemSlotCount != null)
        {
            itemSlotCount.GetComponentInChildren<Text>().text = (GetItemCount(item) + 1).ToString();
        }
    }

    private void addInventorySlot(Items item)
    {
        GameObject newItemSlot;
        newItemSlot = Instantiate(itemSlotPrefab).gameObject;
        newItemSlot.GetComponent<EquipButtonClick>().item = item;
        newItemSlot.GetComponent<EquipButtonClick>().player = player;
        newItemSlot.name = item.itemName + item.type + "_slot";
        newItemSlot.transform.SetParent(InventoryView.transform);

        itemSlotName = newItemSlot.transform.Find("ItemName").gameObject;
        if (itemSlotName != null)
        {
            itemSlotName.GetComponentInChildren<Text>().text = item.itemName;
        }
        itemSlotCount = newItemSlot.transform.Find("ItemCount").gameObject;
        if (itemSlotCount != null)
        {
            itemSlotCount.GetComponentInChildren<Text>().text = "1";
        }
        itemSlotIcon = newItemSlot.transform.Find("Icon").gameObject;
        if (itemSlotIcon != null)
        {
            itemSlotIcon.SetActive(false);
            newItemSlot.GetComponent<EquipButtonClick>().icon = itemSlotIcon;
        }
        itemSlotType = newItemSlot.transform.Find("ItemType").gameObject;
        if (itemSlotType != null)
        {
            itemSlotType.GetComponentInChildren<Text>().text = item.type.ToString();
        }
    }

    private void adjustUI(bool direction)
    {
        if (direction)
        {
            if (_items.Count >= 7)
            {
                InventoryView.GetComponent<RectTransform>().offsetMin = new Vector2(InventoryView.GetComponent<RectTransform>().offsetMin.x, InventoryView.GetComponent<RectTransform>().offsetMin.y - 110);
                Debug.Log("Ustawiam bottom na: " + InventoryView.GetComponent<RectTransform>().offsetMin.y);
            }
        }
        else
        {
            InventoryView.GetComponent<RectTransform>().offsetMin = new Vector2(InventoryView.GetComponent<RectTransform>().offsetMin.x, InventoryView.GetComponent<RectTransform>().offsetMin.y + 110);
            Debug.Log("Ustawiam bottom na: " + InventoryView.GetComponent<RectTransform>().offsetMin.y);
        }
    }
}
