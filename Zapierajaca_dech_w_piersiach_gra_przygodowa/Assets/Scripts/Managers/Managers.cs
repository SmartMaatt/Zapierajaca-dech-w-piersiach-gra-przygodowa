using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(InventoryManager))]
[RequireComponent(typeof(DialogueManager))]
[RequireComponent(typeof(QuestManager))]
[RequireComponent(typeof(CutscenesManager))]
[RequireComponent(typeof(SaveManager))]
public class Managers : MonoBehaviour
{
    public static PlayerManager Player { get; private set; }
    public static InventoryManager Inventory { get; private set; }
    public static DialogueManager Dialogue { get; private set; }
    public static QuestManager Quest { get; private set; }
    public static CutscenesManager Cutscene { get; private set; }
    public static SaveManager Save { get; private set; }
    public static bool allLoaded { get; private set; }

    private List<IGameManager> _startSequence;

    private void Awake()
    {
        allLoaded = false;
        Player = FindObjectsOfType<PlayerManager>()[0].transform.GetComponent<PlayerManager>();
        Inventory = GetComponent<InventoryManager>();
        Dialogue = GetComponent<DialogueManager>();
        Quest = GetComponent<QuestManager>();
        Cutscene = GetComponent<CutscenesManager>();
        Save = GetComponent<SaveManager>();


        _startSequence = new List<IGameManager>();
        
        _startSequence.Add(Inventory);
        _startSequence.Add(Dialogue);
        _startSequence.Add(Quest);
        _startSequence.Add(Player);
        _startSequence.Add(Cutscene);
        _startSequence.Add(Save);

        StartCoroutine(StartupManagers());
    }

    private IEnumerator StartupManagers()
    {
        foreach (IGameManager manager in _startSequence)
        {
            manager.Startup();
        }

        yield return null;

        int numModels = _startSequence.Count;
        int numReady = 0;

        while(numReady < numModels)
        {
            int lastReady = numReady;
            numReady = 0;

            foreach(IGameManager manager in _startSequence)
            {
                if (manager.status == ManagerStatus.Started)
                    numReady++;
            }

            if (numReady > lastReady)
                Debug.Log("Postęp: " + numReady + " / " + numModels);

            yield return null;
        }
        Debug.Log("Wszystkie menadżery uruchomione");
        allLoaded = true;
    }
}
