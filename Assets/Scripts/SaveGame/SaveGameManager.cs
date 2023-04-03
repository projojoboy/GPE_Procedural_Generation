using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using UnityEngine;

static class SaveGameManager
{
    static string fileLocation = "C:/SavegamesOwnProject/WorldGen.sav";

    public static void SaveGame(SaveFile save)
    {
        BinaryFormatter bf = new BinaryFormatter();
        FileStream file = File.Create(fileLocation);
        bf.Serialize(file, save);
        file.Close();
        Debug.Log("Saved Game");
    }

    public static SaveFile LoadGame()
    {
        if (File.Exists(fileLocation))
        {
            BinaryFormatter bf = new BinaryFormatter();
            FileStream file = File.Open(fileLocation, FileMode.Open);
            SaveFile save = (SaveFile)bf.Deserialize(file);
            file.Close();
            return save;
        }
        return null;
    }
}
