using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class SaveLoader : MonoBehaviour
{
    public string saveFilePrefix = "GameSave_";
    private string encryptionPassword = "K00a03j23s50a25"; // Or load from a secure place

    public List<GameSaveData> LoadAllSaves()
    {
        List<GameSaveData> saves = new List<GameSaveData>();
        string savePath = Application.persistentDataPath;

        // Get all files in subdirectories that match the save file pattern
        string[] saveFiles = Directory.GetFiles(savePath, $"{saveFilePrefix}*.es3", SearchOption.AllDirectories);

        foreach (string file in saveFiles)
        {
            var settings = new ES3Settings(file)
            {
                encryptionType = ES3.EncryptionType.AES,
                encryptionPassword = encryptionPassword
            };

            if (ES3.KeyExists("GameSave", settings))
            {
                try
                {
                    GameSaveData save = ES3.Load<GameSaveData>("GameSave", settings);
                    saves.Add(save);
                }
                catch (Exception e)
                {
                    Debug.LogWarning($"⚠️ Could not load save from file: {file}\n{e}");
                }
            }
        }

        // Sort by most recent save
        saves.Sort((a, b) => b.saveDateTime.CompareTo(a.saveDateTime));

        return saves;
    }
}
