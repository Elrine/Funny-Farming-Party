using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;

public static class SaveSystem
{
    public static readonly string SAVE_FOLDER = Application.dataPath + "/Saves/";

    public static void Init() {
        if (!Directory.Exists(SAVE_FOLDER))
            Directory.CreateDirectory(SAVE_FOLDER);
    }

    public static void Save(string worldName, string saveData) {
        File.WriteAllText(SAVE_FOLDER + worldName + ".json", saveData);
    }

    public static string Load(string worldName) {
        if (File.Exists(SAVE_FOLDER + worldName + ".json")) {
            string saveString = File.ReadAllText(SAVE_FOLDER + worldName + ".json");
            return saveString;
        }
        return null;
    }
}
