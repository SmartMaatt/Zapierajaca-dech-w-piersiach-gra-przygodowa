using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIOperator : MonoBehaviour
{
    public GameObject InventoryPanel;
    public GameObject EscapePanel;
    public GameObject QuestPanel;

    public AudioSource open;
    public AudioSource close;

    bool _activInvetory = false;
    bool _activQuests = false;

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.I) && !Managers.Player.isDead && !EscapePanel.active && !QuestPanel.active)
        {
            _activInvetory = !_activInvetory;
            if (!_activInvetory)
            {
                open.Play();
                Time.timeScale = 1.0f;
                Cursor.visible = false;
                Cursor.lockState = Cursor.lockState = CursorLockMode.Locked;
            }
            else if(_activInvetory && !Cursor.visible)
            {
                close.Play();
                Time.timeScale = 0.0f;
                Cursor.visible = true;
                Cursor.lockState = Cursor.lockState = CursorLockMode.None;
            }

            InventoryPanel.SetActive(_activInvetory);
            ReloadInventoryCapacity();
        }

        if (Input.GetKeyDown(KeyCode.Q) && !Managers.Player.isDead && !EscapePanel.active && !InventoryPanel.active)
        {
            _activQuests = !_activQuests;
            if (!_activQuests)
            {
                open.Play();
                Time.timeScale = 1.0f;
                Cursor.visible = false;
                Cursor.lockState = Cursor.lockState = CursorLockMode.Locked;
            }
            else if (_activQuests && !Cursor.visible)
            {
                close.Play();
                Time.timeScale = 0.0f;
                Cursor.visible = true;
                Cursor.lockState = Cursor.lockState = CursorLockMode.None;
                Managers.Quest.ShowQuests();
            }

            QuestPanel.SetActive(_activQuests);
        }
    }

    public void ReloadInventoryCapacity()
    {
        GameObject capacity = InventoryPanel.transform.Find("PanelForMainText").transform.Find("Capacity").gameObject;
        if (capacity != null)
        {
            capacity.GetComponent<Text>().text = Managers.Inventory.GetCapacityInText();
        }
    }
}
