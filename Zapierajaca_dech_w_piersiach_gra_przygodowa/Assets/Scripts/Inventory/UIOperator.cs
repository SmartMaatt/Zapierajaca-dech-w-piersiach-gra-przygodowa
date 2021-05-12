using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIOperator : MonoBehaviour
{
    public GameObject InventoryPanel;
    public GameObject InventoryMainTitle;

    bool _activInvetory = false;

    void Start()
    {
        InventoryPanel.SetActive(_activInvetory);
        InventoryMainTitle.SetActive(_activInvetory);
    }
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.I))
        {
            _activInvetory = !_activInvetory;
            InventoryPanel.SetActive(_activInvetory);
            InventoryMainTitle.SetActive(_activInvetory);
            ReloadCapacity();
        }
    }

    public void ReloadCapacity()
    {
        GameObject capacity = InventoryMainTitle.transform.Find("Capacity").gameObject;
        if (capacity != null)
        {
            capacity.GetComponent<Text>().text = Managers.Inventory.GetCapacityInText();
        }
    }
}
