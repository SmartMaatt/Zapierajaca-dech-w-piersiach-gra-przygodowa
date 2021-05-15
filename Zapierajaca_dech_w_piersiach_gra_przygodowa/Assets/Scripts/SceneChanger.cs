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
        Debug.Log("CHUJ");
        Managers.Save.Save(teleportScene);
        SceneManager.LoadScene(teleportScene);
    }
}
