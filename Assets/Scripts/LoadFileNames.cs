using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Dictonarys/DefaultNames")]
public class LoadFileNames : ScriptableObject
{
    public List<string> fileNames = new List<string>();
}
