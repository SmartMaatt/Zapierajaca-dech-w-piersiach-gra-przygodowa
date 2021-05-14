﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;

public class MainMenu : MonoBehaviour
{
    [SerializeField] string SaveStagePath;

   public void NewGame()
   {
        Managers.Save.Save(1);
        SceneManager.LoadScene(1);
   }

    public void LoadGame()
    {
        if (File.Exists(string.Concat(Application.persistentDataPath, SaveStagePath)))
        {
            SaveStageState currentStageSave = new SaveStageState();
            BinaryFormatter bf = new BinaryFormatter();
            FileStream file = File.Open(string.Concat(Application.persistentDataPath, SaveStagePath), FileMode.Open);
            JsonUtility.FromJsonOverwrite(bf.Deserialize(file).ToString(), currentStageSave);
            file.Close();

            SceneManager.LoadScene(currentStageSave._sceneID);
        }
        else
        {
            //KOMUNIKAT
            Debug.Log(string.Concat(Application.persistentDataPath, SaveStagePath));
        }
    }

    public void QuitGame()
    {
        Application.Quit();
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
