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

    public float startMusicVolume;
    private float _mainSoundBGVolume;

    private void Start()
    {
        _mainSoundBGVolume = startMusicVolume;
        changeVolume(null);
    }

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
            Managers.Inventory.GetAudioManager().UnmuteAllManagers();
            Time.timeScale = 1.0f;
            escapeMenu.SetActive(false);
            Cursor.visible = false;
            Cursor.lockState = Cursor.lockState = CursorLockMode.Locked;
        }
        else
        {
            Time.timeScale = 0.0f;
            Managers.Inventory.GetAudioManager().MuteSoundsWithoutThis();
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
        if (buttonText != null)
        {
            if (_mainSoundBGVolume <= 0)
            {
                _mainSoundBGVolume = 0.2f;
            }
            else
            {
                _mainSoundBGVolume -= 0.01f;
            }
        }

        Managers.Inventory.GetAudioBackground().volume = _mainSoundBGVolume;

        if (buttonText != null)
        {
            if (_mainSoundBGVolume <= 0)
                buttonText.text = "Muzyka Brak";
            else
                buttonText.text = "Muzyka " + Math.Round((100 * Math.Round(_mainSoundBGVolume, 3) / 0.2f), 0).ToString() + "%";
        }
    }

    public float getCurrentVolume()
    {
        return _mainSoundBGVolume;
    }
}
