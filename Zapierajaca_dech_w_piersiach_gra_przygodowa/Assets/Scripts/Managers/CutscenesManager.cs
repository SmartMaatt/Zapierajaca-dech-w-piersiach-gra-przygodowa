using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CutscenesManager : MonoBehaviour, IGameManager
{
    public ManagerStatus status { get; private set; }

    public void Startup()
    {
        status = ManagerStatus.Started;
    }

    public IEnumerator playCutscene(GameObject Scene, GameObject Cutscene)
    {
        yield return new WaitForSeconds(3);
        Cutscene.SetActive(true);
        Scene.SetActive(false);
        yield return new WaitForSeconds(13f);
        Scene.SetActive(true);
        Cutscene.SetActive(false);
    }
}
