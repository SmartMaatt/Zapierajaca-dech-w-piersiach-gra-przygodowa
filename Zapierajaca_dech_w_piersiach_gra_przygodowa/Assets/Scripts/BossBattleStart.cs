using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class BossBattleStart : MonoBehaviour
{
    public Vector3 BossSpawnPosition;
    public GameObject BossHealthBarObj;
    public UIBar BossHealthBarScript;
    public GameObject BossPrefab;
    public GameObject Player;
    [Space]
    public AudioClip battleMusic;
    public GameObject Enemies;
    public GameObject Cutscene;
    public TMP_Text buttonText;
    public UIController soundController;
    public bool isStarted = false;

    private void Update()
    {
        if (!isStarted)
        {
            Vector3 distance = transform.position - Player.transform.position;
            if (distance.magnitude < 15f)
            {
                StartBossfight();
            }
        }
    }

    private void StartBossfight()
    {
        isStarted = true;
        Managers.Inventory.GetAudioBackground().clip = battleMusic;
        soundController.setVolumeToMax(buttonText);
        Managers.Inventory.GetAudioBackground().Play();

        GameObject Boss = Instantiate(BossPrefab, BossSpawnPosition, Quaternion.identity);
        Boss.GetComponent<Mage>().Scene = Enemies;
        Boss.GetComponent<Mage>().Cutscene = Cutscene;
        BossHealthBarObj.SetActive(true);
        Boss.GetComponent<Mage>().SetUpHealthBar(BossHealthBarScript);
    }
}
