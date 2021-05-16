using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneChanger : MonoBehaviour
{
    public int teleportScene;
    CapsuleCollider _collider;

    private void Start()
    {
        _collider = GetComponent<CapsuleCollider>();
    }

    public void ChangeScene()
    {
        StartCoroutine(ChangeSceneCor());
    }

    private IEnumerator ChangeSceneCor()
    {
        yield return new WaitForSeconds(0.5f);
        Managers.Save.Save(teleportScene);
        SceneManager.LoadScene(teleportScene);
    }
}
