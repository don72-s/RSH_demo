using System.Collections;
using System.Collections.Generic;
using UnityEngine;



public enum BGM_TYPE { STAGE1 };
public enum SE_TYPYE { DEFAULT_UPPER, DEFAULT_LOWER };


[CreateAssetMenu(menuName = "Dictonarys/AduioDictionary")]
public class AudioDictonary : ScriptableObject
{
    [System.Serializable]
    struct bgmData {

        [SerializeField]
        public BGM_TYPE bgmType;
        [SerializeField]
        public AudioClip bgmClip;

    }

    [System.Serializable]
    struct seData {

        [SerializeField]
        public SE_TYPYE seType;
        [SerializeField]
        public AudioClip seClip;

    }

    [SerializeField]
    List<bgmData> bgmList;
    [SerializeField]
    List<seData> seList;

    Dictionary<BGM_TYPE, AudioClip> bgmDic = null;
    Dictionary<SE_TYPYE, AudioClip> seDic = null;


    public AudioClip GetBGMClip(BGM_TYPE _bgmType) {

        if (bgmDic == null) { 
        
            bgmDic = new Dictionary<BGM_TYPE, AudioClip>();

            foreach (bgmData _data in bgmList) { 
                bgmDic.Add(_data.bgmType, _data.bgmClip);
            }

        }

        if (bgmDic.ContainsKey(_bgmType))
        {
            return bgmDic[_bgmType];
        }
        else {
            return null;
        }

    }

    public AudioClip GetSEClip(SE_TYPYE _seType)
    {

        if (seDic == null)
        {

            seDic = new Dictionary<SE_TYPYE, AudioClip>();

            foreach (seData _data in seList)
            {
                seDic.Add(_data.seType, _data.seClip);
            }

        }

        if (seDic.ContainsKey(_seType))
        {
            return seDic[_seType];
        }
        else
        {
            return null;
        }

    }


}
