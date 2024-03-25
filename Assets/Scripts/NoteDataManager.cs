
using UnityEngine;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using System;

[System.Serializable]
public class NoteData
{

    public NoteData(int[] _arr) { 
        noteArray = _arr;
    }

    public int[] noteArray;

}



public enum NoteType { NONE, DOWN_NOTE, UPPER_NOTE, INVERSE_DOWN_NOTE, INVERSE_UPPER_NOTE };

[System.Serializable]
public class StageInfo {


    public readonly float offsetSecond;
    public readonly int bpm;
    public readonly int bpmMultiplier;
    public readonly int scoreUnit;

    public readonly BGM_TYPE bgmType;
    public readonly SE_TYPYE upperSeType;
    public readonly SE_TYPYE lowerSeType;

    public readonly NoteInfo[] noteArray;

    public StageInfo(NoteInfo[] _noteArr, float _offsetSecond, int _bpm, int _bpmMultiplier, int _scoreUnit, BGM_TYPE _bgm, SE_TYPYE _upper, SE_TYPYE _lower) { 
    
        noteArray= _noteArr;

        offsetSecond= _offsetSecond;
        bpm= _bpm;
        bpmMultiplier= _bpmMultiplier;
        scoreUnit = _scoreUnit;

        bgmType= _bgm;
        upperSeType= _upper;
        lowerSeType= _lower;

    }

}

[System.Serializable]
public class NoteInfo
{

    public NoteType noteType = NoteType.NONE;
    public int waitingUnit = 0;
    public int effectTimeUnit = 0;


}

public static class NoteDataManager
{


    public static void SaveData(NoteInfo[] _noteArr, float _offsetSecond, int _bpm, int _bpmMultiplier, int _scoreUnit, BGM_TYPE _bgm, SE_TYPYE _upperSE, SE_TYPYE _lowerSE, string _fileName = "noteData.dat") {

        StageInfo tmp = new StageInfo(_noteArr, _offsetSecond, _bpm, _bpmMultiplier, _scoreUnit, _bgm, _upperSE, _lowerSE);
        SaveData(tmp, _fileName);

    }

    public static void SaveData(StageInfo _stageInfo, string _fileName = "noteData.dat") {

        BinaryFormatter formatter = new BinaryFormatter();
        FileStream fileStream = new FileStream(Path.Combine(System.Environment.GetFolderPath(System.Environment.SpecialFolder.Desktop), _fileName), FileMode.Create);
        formatter.Serialize(fileStream, _stageInfo);
        fileStream.Close();

    }

    public static StageInfo LoadData(string _fileName) {//바탕화면 기준

        if (File.Exists(Path.Combine(System.Environment.GetFolderPath(System.Environment.SpecialFolder.Desktop), _fileName)))
        {
            StageInfo tmp;
            BinaryFormatter formatter = new BinaryFormatter();
            FileStream fileStream = new FileStream(Path.Combine(System.Environment.GetFolderPath(System.Environment.SpecialFolder.Desktop), _fileName), FileMode.Open);
            tmp = (StageInfo)formatter.Deserialize(fileStream);
            fileStream.Close();
            return tmp;
        }
        else
        {
            Debug.LogWarning("파일을 찾을수가 없습니다. - 바탕화면에 파일을 올려놓고 진행하세요.");
            return null;
        }

    }



    public static void AndroidSaveData(NoteInfo[] _noteArr, float _offsetSecond, int _bpm, int _bpmMultiplier, int _scoreUnit, BGM_TYPE _bgm, SE_TYPYE _upperSE, SE_TYPYE _lowerSE, string _fileName = "noteData.dat")
    {

        StageInfo tmp = new StageInfo(_noteArr, _offsetSecond, _bpm, _bpmMultiplier, _scoreUnit, _bgm, _upperSE, _lowerSE);
        AndroidSaveData(tmp, _fileName);

    }

    public static void AndroidSaveData(StageInfo _stageInfo, string _fileName = "noteData.dat")
    {

        BinaryFormatter formatter = new BinaryFormatter();
        FileStream fileStream = new FileStream(Path.Combine(Application.persistentDataPath, _fileName), FileMode.Create);
        formatter.Serialize(fileStream, _stageInfo);
        fileStream.Close();

    }

    public static StageInfo AndroidLoadData(string _fileName) {

        if (File.Exists(Path.Combine(Application.persistentDataPath, _fileName)))
        {
            StageInfo tmp;
            BinaryFormatter formatter = new BinaryFormatter();
            FileStream fileStream = new FileStream(Path.Combine(Application.persistentDataPath, _fileName), FileMode.Open);
            tmp = (StageInfo)formatter.Deserialize(fileStream);
            fileStream.Close();
            return tmp;
        }
        else
        {
            return null;
        }

    }


}
