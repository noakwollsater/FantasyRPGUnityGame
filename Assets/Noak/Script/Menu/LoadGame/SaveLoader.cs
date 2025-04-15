using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class SaveLoader : MonoBehaviour
{
    public string saveFilePrefix = "GameSave_";
    public List<GameSaveData> LoadAllSaves()
    {
        List<GameSaveData> saves = new List<GameSaveData>();
        string savePath = Application.persistentDataPath;

        foreach (string file in Directory.GetFiles(savePath, $"{saveFilePrefix}*.es3"))
        {
            var settings = new ES3Settings(file)
            {
                encryptionType = ES3.EncryptionType.AES,
                encryptionPassword = "MySuperSecretPassword123!" // ❗ Samma som du använde vid sparning!
            };

            if (ES3.KeyExists("GameSave", settings))
            {
                GameSaveData save = ES3.Load<GameSaveData>("GameSave", settings);
                saves.Add(save);
            }
        }

        // Sortera t.ex. efter senaste sparning
        saves.Sort((a, b) => b.saveDateTime.CompareTo(a.saveDateTime));

        return saves;
    }
}
