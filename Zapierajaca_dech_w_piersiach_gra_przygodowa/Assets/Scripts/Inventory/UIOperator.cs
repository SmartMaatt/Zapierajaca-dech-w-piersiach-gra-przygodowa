using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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
        }
    }
}
