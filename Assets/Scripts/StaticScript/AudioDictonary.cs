using System.Collections;
using System.Collections.Generic;
using UnityEngine;



public enum BGM_TYPE { STAGE1, STAGE2, STAGE3 };
public enum SE_TYPE { DEF_UPPER, DEF_LOWER };


[CreateAssetMenu(menuName = "Dictonarys/AduioDictionary")]
public class AudioDictonary : ScriptableObject
{
    [System.Serializable]
    struct bgmData {

        [SerializeField]
        public BGM_TYPE bgmType;
        [SerializeField]
        public AudioClip bgmClip;

        [SerializeField]
        public float offsetSecond;
        [SerializeField]
        public int bpm;
    }

    [System.Serializable]
    struct seData {

        [SerializeField]
        public SE_TYPE seType;
        [SerializeField]
        public AudioClip seClip;

    }

    [SerializeField]
    List<bgmData> bgmList;
    [SerializeField]
    List<seData> seList;

    Dictionary<BGM_TYPE, bgmData> bgmDic = null;
    Dictionary<SE_TYPE, seData> seDic = null;


    public AudioClip GetBGMClip(BGM_TYPE _bgmType) {

        CheckBGMDic();

        if (bgmDic.ContainsKey(_bgmType))
        {
            return bgmDic[_bgmType].bgmClip;
        }
        else {
            return null;
        }

    }

    public float GetBGMOffset(BGM_TYPE _bgmType) {

        CheckBGMDic();

        if (bgmDic.ContainsKey(_bgmType))
        {
            return bgmDic[_bgmType].offsetSecond;
        }
        else
        {
            return float.MinValue;
        }

    }

    public float GetBGM_BPM(BGM_TYPE _bgmType)
    {

        CheckBGMDic();

        if (bgmDic.ContainsKey(_bgmType))
        {
            return bgmDic[_bgmType].bpm;
        }
        else
        {
            return float.MinValue;
        }

    }

    private void CheckBGMDic() {

        if (bgmDic == null)
        {

            bgmDic = new Dictionary<BGM_TYPE, bgmData>();

            foreach (bgmData _data in bgmList)
            {
                bgmDic.Add(_data.bgmType, _data);
            }

        }

    }





    public AudioClip GetSEClip(SE_TYPE _seType)
    {

        if (seDic == null)
        {

            seDic = new Dictionary<SE_TYPE, seData>();

            foreach (seData _data in seList)
            {
                seDic.Add(_data.seType, _data);
            }

        }

        if (seDic.ContainsKey(_seType))
        {
            return seDic[_seType].seClip;
        }
        else
        {
            return null;
        }

    }


}
