using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class UIOperator : MonoBehaviour
{
    public GameObject InventoryPanel;
    public GameObject EscapePanel;
    public GameObject QuestPanel;

    bool _activInvetory = false;
    bool _activQuests = false;

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.I) && !Managers.Player.isDead && !EscapePanel.active && !QuestPanel.active && !Managers.Dialogue.isTalking)
        {
            _activInvetory = !_activInvetory;
            if (!_activInvetory)
            {
                Managers.Inventory.GetAudioManager().UnmuteAllManagers();
                Managers.Inventory.GetAudioManager().Play("Close");
                Time.timeScale = 1.0f;
                Cursor.visible = false;
                Cursor.lockState = Cursor.lockState = CursorLockMode.Locked;
                Managers.Player.gameObject.GetComponent<MakeDamage>().enabled = true;
            }
            else if(_activInvetory && !Cursor.visible)
            {
                Managers.Inventory.GetAudioManager().MuteSoundsWithoutThis();
                Managers.Inventory.GetAudioManager().Play("Open");
                Time.timeScale = 0.0f;
                Cursor.visible = true;
                Cursor.lockState = Cursor.lockState = CursorLockMode.None;
                Managers.Player.gameObject.GetComponent<MakeDamage>().enabled = false;
            }

            InventoryPanel.SetActive(_activInvetory);
            ReloadInventoryCapacity();
        }

        if (Input.GetKeyDown(KeyCode.Q) && !Managers.Player.isDead && !EscapePanel.active && !InventoryPanel.active && !Managers.Dialogue.isTalking)
        {
            if (SceneManager.GetActiveScene().buildIndex == 1)
            {
                _activQuests = !_activQuests;
                if (!_activQuests)
                {
                    Managers.Inventory.GetAudioManager().UnmuteAllManagers();
                    Managers.Inventory.GetAudioManager().Play("Close");
                    Time.timeScale = 1.0f;
                    Cursor.visible = false;
                    Cursor.lockState = Cursor.lockState = CursorLockMode.Locked;
                    Managers.Player.gameObject.GetComponent<MakeDamage>().enabled = true;
                }
                else if (_activQuests && !Cursor.visible)
                {
                    Managers.Inventory.GetAudioManager().MuteSoundsWithoutThis();
                    Managers.Inventory.GetAudioManager().Play("Open");
                    Time.timeScale = 0.0f;
                    Cursor.visible = true;
                    Cursor.lockState = Cursor.lockState = CursorLockMode.None;
                    Managers.Quest.ShowQuests();
                    Managers.Player.gameObject.GetComponent<MakeDamage>().enabled = false;
                }

                QuestPanel.SetActive(_activQuests);
            }
            else
            {
                Managers.Quest.popUp.RunFace(3f, 0f);
                Managers.Quest.popUp.gameObject.transform.GetChild(0).gameObject.GetComponent<Text>().text = "Nie ma czasu na questy!\nTrzeba walczyć!";
            }
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
