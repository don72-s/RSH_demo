
using UnityEngine;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using System;
using System.Security.Cryptography;

public enum NoteType { NONE, DOWN_NOTE, UPPER_NOTE, INVERSE_DOWN_NOTE, INVERSE_UPPER_NOTE };

[System.Serializable]
public class StageInfo {


    public readonly float offsetSecond;
    public readonly int bpm;
    public readonly int bpmMultiplier;
    public readonly int scoreUnit;

    public readonly BGM_TYPE bgmType;
    public readonly SE_TYPE upperSeType;
    public readonly SE_TYPE lowerSeType;

    public readonly NoteInfo[] noteArray;

    public StageInfo(NoteInfo[] _noteArr, float _offsetSecond, int _bpm, int _bpmMultiplier, int _scoreUnit, BGM_TYPE _bgm, SE_TYPE _upper, SE_TYPE _lower) { 
    
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
    public int waitScoreCount = 0;
}

public static class NoteDataManager
{
    //todo : #if로 구분하기.

    public static void SaveData(NoteInfo[] _noteArr, float _offsetSecond, int _bpm, int _bpmMultiplier, int _scoreUnit, BGM_TYPE _bgm, SE_TYPE _upperSE, SE_TYPE _lowerSE, string _fileName = "noteData.dat") {

        StageInfo tmp = new StageInfo(_noteArr, _offsetSecond, _bpm, _bpmMultiplier, _scoreUnit, _bgm, _upperSE, _lowerSE);
        SaveData(tmp, _fileName);

    }

    public static void SaveData(StageInfo _stageInfo, string _fileName = "noteData.dat") {

        BinaryFormatter formatter = new BinaryFormatter();
        FileStream fileStream = new FileStream(Path.Combine(System.Environment.GetFolderPath(System.Environment.SpecialFolder.Desktop), _fileName), FileMode.Create);
        formatter.Serialize(fileStream, _stageInfo);
        fileStream.Close();

    }

    public static void SaveData<T>(T _data, string _fileName)
    {

        BinaryFormatter formatter = new BinaryFormatter();
        FileStream fileStream = new FileStream(Path.Combine(System.Environment.GetFolderPath(System.Environment.SpecialFolder.Desktop), _fileName), FileMode.Create);
        formatter.Serialize(fileStream, _data);
        fileStream.Close();

    }

    /// <summary>
    /// 전달된 이름의 파일을 읽어옴 [ .dat 포함된 파일 이름 전달 ], 바탕화면 기준.
    /// </summary>
    /// <param name="_fileName">파일 이름 [ .dat 포함된 이름으로 전달 ]</param>
    /// <returns></returns>
    public static StageInfo LoadData(string _fileName) {

        if (CheckFileExist(_fileName))
        {
            StageInfo tmp;
            BinaryFormatter formatter = new BinaryFormatter();
            FileStream fileStream = new FileStream(Path.Combine(Application.streamingAssetsPath, _fileName), FileMode.Open);
            tmp = (StageInfo)formatter.Deserialize(fileStream);
            fileStream.Close();
            return tmp;
        }
        else
        {
            Debug.LogWarning("파일을 찾을수가 없습니다. - 바탕화면에 파일을 올려놓고 진행하세요._loadData");
            return null;
        }

    }

    public static UserData LoadUserData() {

        if (CheckFileExist("userData.sav"))
        {
            UserData tmp;
            BinaryFormatter formatter = new BinaryFormatter();
            FileStream fileStream = new FileStream(Path.Combine(Application.streamingAssetsPath, "userData.sav"), FileMode.Open);
            tmp = (UserData)formatter.Deserialize(fileStream);
            fileStream.Close();
            return tmp;
        }
        else
        {
            Debug.LogWarning("파일을 찾을수가 없습니다. - 바탕화면에 파일을 올려놓고 진행하세요._LoadUserData");
            return null;
        }

    }

    /// <summary>
    /// 파일의 존재 확인.
    /// </summary>
    /// <param name="_fileName">[.dat]이 포함된 파일 이름</param>
    /// <returns>파일 존재 여부</returns>
    public static bool CheckFileExist(string _fileName)
    {
        return File.Exists(Path.Combine(Application.streamingAssetsPath, _fileName));
    }




    public static void AndroidSaveData(NoteInfo[] _noteArr, float _offsetSecond, int _bpm, int _bpmMultiplier, int _scoreUnit, BGM_TYPE _bgm, SE_TYPE _upperSE, SE_TYPE _lowerSE, string _fileName = "noteData.dat")
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


    public static void AndroidSaveData<T>(T _data, string _fileName)
    {

        BinaryFormatter formatter = new BinaryFormatter();
        FileStream fileStream = new FileStream(Path.Combine(Application.persistentDataPath, _fileName), FileMode.Create);
        formatter.Serialize(fileStream, _data);
        fileStream.Close();

    }



    /// <summary>
    /// 전달된 이름의 파일을 읽어옴 [ .dat 포함된 파일 이름 전달 ], persistentDataPath 기준.
    /// </summary>
    /// <param name="_fileName">파일 이름 [ .dat 포함된 이름으로 전달 ]</param>
    /// <returns></returns>
    public static StageInfo AndroidLoadData(string _fileName) {

        if (CheckAndroidFileExist(_fileName))
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

    public static bool AndroidDeleteFile(string _fileName) {

        if(!CheckAndroidFileExist(_fileName))
            return false;

        try {
            File.Delete(Path.Combine(Application.persistentDataPath, _fileName));
        } catch (Exception _e) {
            throw new Exception(_e.ToString());
        }

        return true;

    }

    public static UserData AndroidLoadUserData()
    {

        if (CheckAndroidFileExist("userData.sav"))
        {
            UserData tmp;
            BinaryFormatter formatter = new BinaryFormatter();
            FileStream fileStream = new FileStream(Path.Combine(Application.persistentDataPath, "userData.sav"), FileMode.Open);
            tmp = (UserData)formatter.Deserialize(fileStream);
            fileStream.Close();
            return tmp;
        }
        else
        {
            Debug.LogWarning("파일을 찾을수가 없습니다. - 바탕화면에 파일을 올려놓고 진행하세요._AndroidLoadUserData");
            return null;
        }

    }


    /// <summary>
    /// 파일의 존재 확인.
    /// </summary>
    /// <param name="_fileName">[.dat]이 포함된 파일 이름</param>
    /// <returns>파일 존재 여부</returns>
    public static bool CheckAndroidFileExist(string _fileName)
    {
        return File.Exists(Path.Combine(Application.persistentDataPath, _fileName));
    }



    public static bool CheckHash(LoadFileNames _fileNames) {

        List<LoadFileNames.FileInfo> fileInfoL = _fileNames.fileInfoL;

        foreach (LoadFileNames.FileInfo fileInfo in fileInfoL) {


            if (!CheckHash(fileInfo)) return false;

        }

        return true;

    }

    public static bool CheckHash(LoadFileNames.FileInfo _fileInfo)
    {
        return CheckHash(_fileInfo.fileName, _fileInfo.sha256Hash);
    }


    public static bool CheckHash(string _fileName, string _hashValue) {

    string path;

#if UNITY_EDITOR
        path = Path.Combine(Application.streamingAssetsPath, _fileName);
#elif UNITY_ANDROID
    path = Path.Combine(Application.persistentDataPath, _fileName);
#endif
        using (var stream = File.OpenRead(path))
        {
            var sha256 = SHA256.Create();
            byte[] hashBytes = sha256.ComputeHash(stream);

            return _hashValue.Equals(BitConverter.ToString(hashBytes).Replace("-", "").ToLower());
        }
    }

}
