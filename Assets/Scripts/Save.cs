using System;
using System.IO;
using UnityEngine;

[Serializable]
public class Save
{
    public int level;
    public int levelToReplay;

    private const string pathEnd = "/save.json";
    public Save() 
    {
        
    }

    public Save(int level, int levelToReplay)
    {
        this.level = level;
        this.levelToReplay = levelToReplay;
    }

    public static Save GetSave()
    {
        string path = Application.persistentDataPath + pathEnd;
        
        // if file doesnt exist create save with level 1
        if (!File.Exists(path))
        {
            SetSave(1, 0);
            return GetSave();
        }

        string data = File.ReadAllText(path);
        Save save = JsonUtility.FromJson<Save>(data);
        return save;
    }

    public static void SetSave(int level, int levelToReplay)
    {
        Save save = new(level, levelToReplay);
        string saveData = JsonUtility.ToJson(save);
        string path = Application.persistentDataPath + pathEnd;
        File.WriteAllText(path, saveData);
    }
}

