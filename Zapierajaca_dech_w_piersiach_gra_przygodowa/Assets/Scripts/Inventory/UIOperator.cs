using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIOperator : MonoBehaviour
{
    public GameObject InventoryPanel;

    bool _activInvetory = false;

    void Start()
    {
        InventoryPanel.SetActive(_activInvetory);
    }
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.I))
        {
            _activInvetory = !_activInvetory;
            if (!_activInvetory)
            {
                Time.timeScale = 1.0f;
                Cursor.visible = false;
                Cursor.lockState = Cursor.lockState = CursorLockMode.Locked;
            }
            else
            {
                Time.timeScale = 0.0f;
                Cursor.visible = true;
                Cursor.lockState = Cursor.lockState = CursorLockMode.None;
            }

            InventoryPanel.SetActive(_activInvetory);
            ReloadCapacity();
        }
    }

    public void ReloadCapacity()
    {
        GameObject capacity = InventoryPanel.transform.Find("PanelForMainText").transform.Find("Capacity").gameObject;
        if (capacity != null)
        {
            capacity.GetComponent<Text>().text = Managers.Inventory.GetCapacityInText();
        }
    }
}
