using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Dictonarys/DefaultNames")]
public class LoadFileNames : ScriptableObject
{
    public List<FileInfo> fileInfoL = new List<FileInfo>();

    public List<string> GetFileNames() { 
    
        List<string> result = new List<string>();

        foreach (FileInfo file in fileInfoL) { 
            result.Add(file.fileName);
        }

        return result;

    }

    [System.Serializable]
    public struct FileInfo {
        public string fileName;
        public string sha256Hash;
    }
}
