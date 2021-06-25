using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;

public class CreditsMenu : MonoBehaviour
{
    public void GoToMain()
    {
        Managers.Inventory.GetAudioManager().Play("Click");
        SceneManager.LoadScene(0);
    }

    public void QuitGame()
    {
        Managers.Inventory.GetAudioManager().Play("Click");
        Application.Quit();
    }
}
