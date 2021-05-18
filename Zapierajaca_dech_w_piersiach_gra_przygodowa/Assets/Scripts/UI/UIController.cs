using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;
using System;

public class UIController : MonoBehaviour
{

    [SerializeField] GameObject LeftPart;
    [SerializeField] GameObject RightPart;
    [SerializeField] GameObject DiedSprite;
    [SerializeField] GameObject Inventory;
    [SerializeField] GameObject Quests;

    [SerializeField] GameObject escapeMenu;
    [SerializeField] AudioSource mainSoundBackground;
    private float _mainSoundBGVolume = 0.05f;

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape) && !Inventory.active && !Quests.active)
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

    public void changeVolume(TMP_Text buttonText)
    {
        if (_mainSoundBGVolume <= 0)
        {
            _mainSoundBGVolume = 0.5f;
        }
        else
        {
            _mainSoundBGVolume -= 0.05f;
        }

        mainSoundBackground.volume = _mainSoundBGVolume;

        if (_mainSoundBGVolume <= 0)
            buttonText.text = "Muzyka Brak";
        else
            buttonText.text = "Muzyka " + Math.Round(_mainSoundBGVolume,2).ToString(); 
    }

}
