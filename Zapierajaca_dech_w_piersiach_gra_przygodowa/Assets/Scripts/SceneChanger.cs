using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneChanger : MonoBehaviour
{
    public int teleportScene;
    public Quest killBorysQuest;
    CapsuleCollider _collider;

    private void Start()
    {
        _collider = GetComponent<CapsuleCollider>();
    }

    public void ChangeScene()
    {
        foreach(KillQuest quest in Managers.Quest.killQuests)
        {
            if(quest == killBorysQuest)
            {
                StartCoroutine(ChangeSceneCor());
                break;
            }
        }
    }

    private IEnumerator ChangeSceneCor()
    {
        yield return new WaitForSeconds(0.5f);
        Managers.Save.Save(teleportScene);
        SceneManager.LoadScene(teleportScene);
    }
}
