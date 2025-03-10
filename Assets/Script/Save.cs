using UnityEngine;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using UnityEngine.Audio;

public static class Save
{
    static readonly string path = Application.persistentDataPath + "/LushData.bin";

    public static void SaveData(Player player, AudioMixer audioMixer, bool widescreen) 
    {
        BinaryFormatter formatter = new();

        FileStream stream = new(path, FileMode.Create);

        GameData gameData = new(player, audioMixer, widescreen);

        formatter.Serialize(stream, gameData);
        stream.Close();
    }

    public static GameData LoadData() 
    {
        if (File.Exists(path))
        {
            BinaryFormatter formatter = new();
            FileStream stream = new(path, FileMode.Open);

            GameData gameData = formatter.Deserialize(stream) as GameData;
            stream.Close();

            return gameData;
        }
        else
        {
            Debug.LogWarning("Save file not found in " + path);

            return null;
        }
    }
}
