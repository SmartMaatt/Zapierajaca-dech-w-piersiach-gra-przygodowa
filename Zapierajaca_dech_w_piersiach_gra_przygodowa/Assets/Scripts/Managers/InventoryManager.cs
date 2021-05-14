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
    public Dictionary<int, int> _items { get; private set; }

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
    public GameObject[] ItemsPrefabs = new GameObject[2];
    

    public void Startup()
    {
        Debug.Log("Uruchomienie menadżera magazynu...");
        _items = new Dictionary<int, int>();
        status = ManagerStatus.Started;
        if (error)
            error.enabled = false;
    }

    public List<int> GetItemList()
    {
        List<int> list = new List<int>(_items.Keys);
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

    public int GetItemCount(int searchedItem)
    {
        int suma = 0;
        foreach (int item in _items.Keys)
        {
            if (item == searchedItem)
                suma++;
        }
        return suma;
    }

    public void PlayerDrop(int item)
    {
        ConsumeItem(item);
        ReloadCapacity();

        Items targetItem = ItemsPrefabs[item].GetComponent<Items>();
        GameObject prevEquip = InventoryView.transform.Find(targetItem.itemName + targetItem.type + "_slot").gameObject;
        addInventoryCount(prevEquip, item, 0);
    }

    public string GetCapacityInText()
    {
        return _items.Count + " / " + inventorySize;
    }

    public bool AddItem(int item)
    {
        Debug.Log("Add item: " + item);
        if (_items.Count < inventorySize)
        {
            if (GetItemCount(item) >= 1)
            {
                Items thisItem = ItemsPrefabs[item].GetComponent<Items>();
                string name = thisItem.itemName + thisItem.type + "_slot";
                GameObject ItemSlot = InventoryView.transform.Find(name).gameObject;

                if (ItemSlot != null)
                    addInventoryCount(ItemSlot, item, 1);
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

    public bool EquipItem(int item)
    {
        Items thisItem = ItemsPrefabs[item].GetComponent<Items>();
        switch (thisItem.type)
        {
            case Items.itemType.WEAPON:
                if (_items.ContainsKey(item) && equippedWeapon != thisItem.itemName)
                {
                    changeEquippedItem(equippedWeapon, item);
                    equippedWeapon = thisItem.itemName;
                    return true;
                }
                equippedWeapon = null;
                return false;
            case Items.itemType.SHIELD:
                if (_items.ContainsKey(item) && equippedShield != thisItem.itemName)
                {
                    changeEquippedItem(equippedShield, item);
                    equippedShield = thisItem.itemName;
                    return true;
                }
                equippedShield = null;
                return false;
            case Items.itemType.POTION:
                if (_items.ContainsKey(item) && equippedPotion != thisItem.itemName)
                {
                    changeEquippedItem(equippedPotion, item);
                    equippedPotion = thisItem.itemName;
                    return true;
                }
                equippedPotion = null;
                return false;
            case Items.itemType.SPECIAL:
                if (_items.ContainsKey(item) && equippedSpecial != thisItem.itemName)
                {
                    changeEquippedItem(equippedSpecial, item);
                    equippedSpecial = thisItem.itemName;
                    return true;
                }
                equippedSpecial = null;
                return false;
        }
        return false;
    }

    public bool ConsumeItem(int item)
    {
        if (_items.ContainsKey(item))
        {
            _items[item]--;
            if (_items[item] == 0)
            {
                if (_items.Count > 7)
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
        for(int i=0; i<ItemsPrefabs.Length; i++)
        {
            if (ItemsPrefabs[i].GetComponent<Items>().itemName + ItemsPrefabs[i].GetComponent<Items>().type == name)
                return ItemsPrefabs[i].GetComponent<Items>();
        }
        return null;
    }

    private IEnumerator errorPopup()
    {
        error.enabled = true;
        yield return new WaitForSeconds(2.5f);
        error.enabled = false;
    }

    private void changeEquippedItem(string itemType, int item)
    {
        if (itemType != null)
        {
            Items thisItem = ItemsPrefabs[item].GetComponent<Items>();
            GameObject prevEquip;
            prevEquip = InventoryView.transform.Find(itemType + thisItem.type + "_slot").gameObject;
            bool activIcon = !prevEquip.GetComponent<EquipButtonClick>().iconActiv;
            prevEquip.GetComponent<EquipButtonClick>().iconActiv = activIcon;
            GameObject icon = prevEquip.transform.Find("Icon").gameObject;
            icon.SetActive(activIcon);
        }
    }

    private void addInventoryCount(GameObject ItemSlot, int item, int amount)
    {
        itemSlotCount = ItemSlot.transform.Find("ItemCount").gameObject;
        if (itemSlotCount != null)
        {
            itemSlotCount.GetComponentInChildren<Text>().text = (GetItemCount(item) + amount).ToString();
        }
    }

    private void addInventorySlot(int item)
    {
        GameObject newItemSlot;
        newItemSlot = Instantiate(itemSlotPrefab).gameObject;
        newItemSlot.GetComponent<EquipButtonClick>().itemIndex = item;
        newItemSlot.GetComponent<EquipButtonClick>().player = player;

        Items thisItem = ItemsPrefabs[item].GetComponent<Items>();
        newItemSlot.name = thisItem.itemName + thisItem.type + "_slot";
        newItemSlot.transform.SetParent(InventoryView.transform);

        itemSlotName = newItemSlot.transform.Find("ItemName").gameObject;
        if (itemSlotName != null)
        {
            itemSlotName.GetComponentInChildren<Text>().text = thisItem.itemName;
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
            itemSlotType.GetComponentInChildren<Text>().text = thisItem.type.ToString();
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

    public void clearInventory()
    {
        int children = InventoryView.transform.childCount;

        for (int i = children - 1; i >= 0; i--)
        {
            GameObject.DestroyImmediate(InventoryView.transform.GetChild(i).gameObject);
        }

        GameObject capacity = InventoryMainTitle.transform.Find("Capacity").gameObject;
        if (capacity != null)
        {
            capacity.GetComponent<Text>().text = "0 / " + inventorySize.ToString();
        }

        _items.Clear();
    }
}
