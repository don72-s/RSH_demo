
using UnityEngine;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;

[System.Serializable]
public class NoteData
{

    public NoteData(int[] _arr) { 
        noteArray = _arr;
    }

    public int[] noteArray;
}

public static class NoteDataManager
{

    public static void SaveData(int[] _noteArray)
    {
        BinaryFormatter formatter = new BinaryFormatter();
        FileStream fileStream = new FileStream(Path.Combine(System.Environment.GetFolderPath(System.Environment.SpecialFolder.Desktop), "noteData.dat"), FileMode.Create);
        formatter.Serialize(fileStream, new NoteData(_noteArray));
        fileStream.Close();
    }

    public static void SaveData(List<int> _noteArray)
    {
        SaveData(_noteArray.ToArray());
    }

    public static int[] LoadData()
    {
        if (File.Exists(Path.Combine(System.Environment.GetFolderPath(System.Environment.SpecialFolder.Desktop), "noteData.dat")))
        {
            NoteData tmp;
            BinaryFormatter formatter = new BinaryFormatter();
            FileStream fileStream = new FileStream(Path.Combine(System.Environment.GetFolderPath(System.Environment.SpecialFolder.Desktop), "noteData.dat"), FileMode.Open);
            tmp = (NoteData)formatter.Deserialize(fileStream);
            fileStream.Close();
            return tmp.noteArray;
        }
        else
        {
            Debug.LogWarning("Save file not found.");
            return null;
        }
    }

    public static int[] AndroidMapLoadData() {

        if (File.Exists(Path.Combine(Application.persistentDataPath, "noteData.dat")))
        {
            NoteData tmp;
            BinaryFormatter formatter = new BinaryFormatter();
            FileStream fileStream = new FileStream(Path.Combine(Application.persistentDataPath, "noteData.dat"), FileMode.Open);
            tmp = (NoteData)formatter.Deserialize(fileStream);
            fileStream.Close();
            return tmp.noteArray;
        }
        else
        {
            Debug.LogWarning("Save file not found.");
            return null;
        }

    }

}
