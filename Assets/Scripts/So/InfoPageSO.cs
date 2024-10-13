using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;

[CreateAssetMenu(menuName = "InfoPageSO")]
public class InfoPageSO : ScriptableObject
{
    
    public List<InfoBlock> InfoBlockL;

    [Serializable]
    public struct InfoBlock {
        public string titleText;
        public Sprite img;

    }

}
