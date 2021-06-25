using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class CutscenesManager : MonoBehaviour, IGameManager
{
    public ManagerStatus status { get; private set; }

    public void Startup()
    {
        status = ManagerStatus.Started;
    }

    public IEnumerator playCutscene(GameObject Scene, GameObject Cutscene)
    {
        Managers.Inventory.GetAudioBackground().Stop();
        Managers.Inventory.GetAudioManager().Play("EpicSaxGuy");
        Managers.Player.transform.gameObject.SetActive(false);
        yield return new WaitForSeconds(1f);
        Cutscene.SetActive(true);
        Scene.SetActive(false);
        yield return new WaitForSeconds(13f);
        SceneManager.LoadScene(4);
        Cursor.visible = true;
        Cursor.lockState = Cursor.lockState = CursorLockMode.None;
    }
}
