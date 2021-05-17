using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;

public class SaveManager : MonoBehaviour, IGameManager
{
    public ManagerStatus status { get; private set; }
    public string SavePath;
    public string SaveScenePath;
    
    public void Startup()
    {
        Debug.Log("Uruchomienie menadżera zapisu i odczytu...");
        Load();
        status = ManagerStatus.Started;
    }

    public void Save(int sceneToSave)
    {
        int _sceneToSave = 0;
        if (sceneToSave < 0)
            _sceneToSave = SceneManager.GetActiveScene().buildIndex + 1;
        else
            _sceneToSave = sceneToSave;

        Debug.Log("Save");
        int _exp = Managers.Player.exp;
        int _money = Managers.Player.money;
        int _level = Managers.Player.level;
        int[] _keysInventory = (new List<int>(Managers.Inventory._items.Keys)).ToArray();
        int[] _valuesInventory = (new List<int>(Managers.Inventory._items.Values)).ToArray();

        SaveFileTemplate currentSave = new SaveFileTemplate(_exp, _money, _level, _keysInventory, _valuesInventory);
        SaveStageState currentStageSave = new SaveStageState(_sceneToSave);

        string saveData = JsonUtility.ToJson(currentSave, true);
        string saveStageData = JsonUtility.ToJson(currentStageSave, true);

        BinaryFormatter bf = new BinaryFormatter();
        FileStream file = File.Create(string.Concat(Application.persistentDataPath, SavePath));
        bf.Serialize(file, saveData);
        file.Close();

        BinaryFormatter bf2 = new BinaryFormatter();
        FileStream fileScene = File.Create(string.Concat(Application.persistentDataPath, SaveScenePath));
        bf2.Serialize(fileScene, saveStageData);
        fileScene.Close();
    }

    public void Load()
    {
        if(File.Exists(string.Concat(Application.persistentDataPath, SavePath)) && SceneManager.GetActiveScene().buildIndex != 0 && SceneManager.GetActiveScene().buildIndex != 4)
        {
            Debug.Log("Load");
            SaveFileTemplate currentLoad = new SaveFileTemplate();

            BinaryFormatter bf = new BinaryFormatter();
            FileStream file = File.Open(string.Concat(Application.persistentDataPath, SavePath), FileMode.Open);
            JsonUtility.FromJsonOverwrite(bf.Deserialize(file).ToString(), currentLoad);
            file.Close();

            Managers.Player.exp = currentLoad._exp;
            Managers.Player.level = currentLoad._level;
            Managers.Player.money = currentLoad._money;
            
            for (int i = 0; i < currentLoad._keysInventory.Length; i++)
            {
                for(int j = 0; j < currentLoad._valuesInventory[i]; j++)
                {
                    Managers.Inventory.AddItem(currentLoad._keysInventory[i]);
                }
            }
        }
        else
        {
            Debug.Log("Menu główne, wczytanie odrzucone");
        }
    }

    private class SaveFileTemplate
    {
        public int _exp;
        public int _money;
        public int _level;
        public int[] _keysInventory;
        public int[] _valuesInventory;

        public SaveFileTemplate()
        {
            _exp = 0;
            _money = 0;
            _level = 0;
            _keysInventory = null;
            _valuesInventory = null;
        }

        public SaveFileTemplate(int exp, int money, int level, int[] keysInventory, int[] valuesInventory)
        {
            _exp = exp;
            _money = money;
            _level = level;
            _keysInventory = keysInventory;
            _valuesInventory = valuesInventory;
        }
    }

    private class SaveStageState
    {
        public int _sceneID;

        public SaveStageState()
        {
            _sceneID = 0;
        }
        public SaveStageState(int sceneID)
        {
            _sceneID = sceneID;
        }
    }
}
