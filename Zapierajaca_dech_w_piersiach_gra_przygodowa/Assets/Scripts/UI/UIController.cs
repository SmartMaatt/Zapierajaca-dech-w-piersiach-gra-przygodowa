using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class UIController : MonoBehaviour
{
    [SerializeField] GameObject LeftPart;
    [SerializeField] GameObject RightPart;
    [SerializeField] GameObject DiedSprite;
    [SerializeField] GameObject Inventory;
    [SerializeField] GameObject Quests;

    [SerializeField] GameObject escapeMenu;

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape) && !Inventory.active)
        {
            escapeMenuToggle();
        }
    }

    public void leaveToMainMenu()
    {
        SceneManager.LoadScene(0);
    }

    public void escapeMenuToggle()
    {
        if (escapeMenu.active)
        {
            Time.timeScale = 1.0f;
            escapeMenu.SetActive(false);
            Cursor.visible = false;
            Cursor.lockState = Cursor.lockState = CursorLockMode.Locked;
        }
        else
        {
            Time.timeScale = 0.0f;
            escapeMenu.SetActive(true);
            Cursor.visible = true;
            Cursor.lockState = Cursor.lockState = CursorLockMode.None;
        }
    }

    public void playerDiedScene()
    {
        LeftPart.SetActive(false);
        RightPart.SetActive(false);
        Inventory.SetActive(false);
        Quests.SetActive(false);
        DiedSprite.SetActive(true);
    }
}
