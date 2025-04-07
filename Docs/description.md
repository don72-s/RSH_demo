# 프로젝트 문서

프로젝트의 전체적인 구조와 설명이 작성된 문서입니다.  

<br>

# 개요

**●자이로 센서를 이용한 입력을 사용하는 간단한 리듬게임을 구현한다.**  
  
**●등록되어 있는 음원을 이용하여 사용자가 직접 스테이지를 작성할 수 있는 에디터 기능을 구현한다.**

<br>

# 목차

<h3>

  ### [1. 리듬게임](#리듬게임)
  ### [2. 에디터](#에디터)
  ### [3. 데이터](#데이터)
  
</h3><br>


우측 상단의 버튼을 통해 목차를 펼칠 수 있습니다.

![목차](https://github.com/user-attachments/assets/4da446d5-b0b9-404f-988d-90b65c47469d)

<hr><br>

# 리듬게임

리듬게임의 구조와 플레이방식에 대해 기술한다.  
<br>

## 플레이방식

게임의 플레이는 기본적으로 **암기형**리듬게임의 구조를 가지며  
노트막대의 첫 스윕 때 노트를 **재생**  
두번째 스윕 때 첫 스윕을 **플레이**하는 방식으로 진행된다.  

![ddd](https://github.com/don72-s/RSH_demo/assets/66211881/83013175-dd40-443e-8f19-4241efa0cd11)


<br>

### 입력

기기의 자이로센서를 이용한 입력을 이용하기로 한다.  
<br>

기기를 **왼쪽**으로 기울이는 입력과 **오른쪽**으로 기울이는 입력을 받기 위해 자이로 순간속도 벡터의 z값을 이용하도록 한다.  
<br>

자이로센서 벡터의 z값에 따라 대응 행동을 실행.
```cpp
if (!isLowerPlaying && vec.z > lowerOffset) {
    StartCoroutine(playLowerSnd());
}

if (!isUpperPlaying && vec.z < upperOffset)
{
    StartCoroutine(playUpperSnd());
}
```

<br><br>

### 노트의 로직 구현

노트의 로직을 단순화하면 다음과 같다.  
<br>
![판정선](https://github.com/don72-s/RSH_demo/assets/66211881/79df000b-6664-4fb3-9e91-1804f58e1d3d)
<br><br>
생성, 소멸이 잦으므로 오브젝트 풀 방식을 사용하기로 하며 각 노트의 독립적인 동작을 위해 노트의 동작은 코루틴으로 구현하도록 한다.  
<br>

### 정지

모든 노트의 진행은 코루틴을 이용한다. 노트의 진행 로직 코드는 **waitforseconds**와 +=**Time.deltaTime**에 의존하기 때문에  
정지시에는 BGM 재생을 중지한 뒤, **Time.timeScale**을 0으로 지정하는것으로 일시정지를 구현할 수 있다.  
<br>

일시정지 코드  
```cpp
public void Pause()
{
    if (!bgmPause)
    {
        bgmPause = true;
        Time.timeScale = 0;
        bgmplayer.Pause();
    }
    else {
        bgmPause = false;
        Time.timeScale = 1;
        bgmplayer.Play();
    }
}
```

## 판정

가벼운 플레이를 위하여 판정 조건은 넉넉하게 잡도록 한다.  
<br>

### 판정 대기

각 노트는 생성된 후에 다음과 같은 대기시간 뒤의 시점을 **정확한 판정 시점**으로 가진다.
이를 그림과 식으로 표현하면 아래와 같은 구성이 된다.
<br>
![마디설명](https://github.com/don72-s/RSH_demo/assets/66211881/efd9a627-04bf-4c88-95a4-58071d1da009)

**단위비트간 시간(사각형 사이 시간)** <br>
x **비트 세분화 계수(큰 사각형 사이의 사각형 갯수)** <br>
x **사용자 지정 마디 박자(가로 한 줄의 큰 사각형 갯수)** <br>
x **대기마디 수(채보 영역의 가로줄 갯수)** <br>

=> 1 * 4 * 2 * 2 = 16 <br>
즉, 특정 노트로부터 판정 시점까지의 시간은 **사각형 16개** 분량의 시간이 지난 뒤로 계산된다.

<br>

해당 내용에 대응되는 코드는 다음과 같다.

```cpp
IEnumerator PlayNote(int _watingUnit, Action _swipeSnd, Action _displayNote, Func<bool> _checkInput, Action _useInput, Action _playSnd) {

    //...생략...

    //판정선의 직전까지 대기시간을 계산한 뒤 대기한다.
    float waitTime = bpmUnitSecond * stageData.bpmMultiplier * stageData.scoreUnit * _watingUnit - (bpmUnitSecond * 2);

    yield return new WaitForSeconds(waitTime);

    //...노트 판정 구간 후술...
}
```
<br>

### 판정 세분화

**정확한 판정 시점**으로부터 +- **단위비트 시간**영역을 **정확(correct)** 판정으로, **정확**판정에 해당하지 않는 +- **단위비트 시간 * 2** 영역을 **성공(good)** 
판정으로 정의하기로 한다.  
<br>

![판정](https://github.com/don72-s/RSH_demo/assets/66211881/dc86cdbb-7401-43e0-a7e8-590e87ace566)

<br>

<hr><br>

# 에디터
사용자가 사용자 지정 스테이지를 제작할 수 있는 기능을 제공한다.  
<br>

## 제한 사항
앱에서 제공되는 노래에 한해서만 스테이지 제작이 가능하며, BPM변경은 불가능하다.  
<br>

### 데이터 구조 정의

데이터를 저장하기 위한 요소를 가진 구조를 정의한다.  
<br>

직렬화하여 저장하기 위해 **Serializable**을 지정한다.
```cpp
[System.Serializable]
public class StageInfo {

    public readonly float offsetSecond;//bgm플레이 전 대기시간
    public readonly int bpm;
    public readonly int bpmMultiplier;
    public readonly int scoreUnit;//기본 마디수 지정

    //사운드 타입 지정
    public readonly BGM_TYPE bgmType;
    public readonly SE_TYPE upperSeType;
    public readonly SE_TYPE lowerSeType;

    //노트 배열 저장
    public readonly NoteInfo[] noteArray;

    public StageInfo(NoteInfo[] _noteArr, float _offsetSecond, int _bpm, int _bpmMultiplier, int _scoreUnit, BGM_TYPE _bgm, SE_TYPE _upper, SE_TYPE _lower) { 
        //생성자 생략
    }

}
```
<br>

### 채보 방식 구현

채보할 구간을 지정한 뒤 출력부분을 채보, 그 뒤에 대응되는 플레이 영역은 자동으로 완성되도록 구현.  
<br>

NoteInfo[] 타입의 임시 노트 배열을 선언한 뒤. 모든 채보가 완료된 뒤, 파일로 출력 저장.

### 채보 적용 방법

**비트 세분화 계수** ,  **마디 박자 수** 에 대응되는 디스플레이 버튼을 이용하여 노트를 지정.  
<br>

원하는 대로 채보 후, apply버튼을 눌러 테스트 가능.  
<br>

![제작](https://github.com/don72-s/RSH_demo/assets/66211881/e9a5bbe9-ceaa-4e21-860a-21c50187ef41)



<br>

### 내보내기

현재까지 채보된 NoteInfo[] 타입의 변수를 파일로 저장하는 기능을 구현.  
<br>

serializable로 선언한 클래스들을 이용하여 직렬화시키는 코드를 작성한다.  
<br>

파일을 Application.persistentDataPath에 저장.  
```cpp
public static void AndroidSaveData(StageInfo _stageInfo, string _fileName = "noteData.dat")
{
    BinaryFormatter formatter = new BinaryFormatter();
    FileStream fileStream = new FileStream(Path.Combine(Application.persistentDataPath, _fileName), FileMode.Create);
    formatter.Serialize(fileStream, _stageInfo);
    fileStream.Close();
}
```

<br>

단 이미 같은 이름의 파일이 존재할 경우, 덮어쓰기 여부를 확인하는 기능을 추가한다.
<br>

```cpp
if (NoteDataManager.CheckAndroidFileExist(saveFileName))
{
    alertWindow.ShowDoubleAlertWindow("[ " + saveFileName + " ] 같은 이름의 파일이 존재합니다.\n덮어쓰시겠습니까?", "OK!", () => {
    
        NoteDataManager.AndroidSaveData(noteArray, offsetSecond, (int)BPM, BPM_Multiplyer, scoreUnit, bgmType, upperSeType, lowerSeType, saveFileName);
        alertWindow.ShowSingleAlertWindow("노트 파일 [ " + saveFileName + " ] 이 저장되었습니다!");
        btn_Export_Window(false);

    });
}
else {
    NoteDataManager.AndroidSaveData(noteArray, offsetSecond, (int)BPM, BPM_Multiplyer, scoreUnit, bgmType, upperSeType, lowerSeType, saveFileName);
    alertWindow.ShowSingleAlertWindow("노트 파일 [ " + saveFileName + " ] 이 저장되었습니다!");
    btn_Export_Window(false);

}
```

<br>

<hr><br>

# 데이터

기본 제공 노트맵, 커스텀 노트맵 저장과 취급에 대하여 설명한다.  
<br>

데이터파일은 안드로이드 기기의 data\conDefaultCompany.rhythmShaker\files 디렉토리에 저장한다.  
<br>

## 노트맵 데이터

노트맵 데이터는 **시스템 제공 스테이지**와 **커스텀 제작 스테이지**데이터로 분류하여 저장한다.  
<br>

### 데이터의 구분

간단하게 파일의 이름이 **Stage**로 시작하는 .dat파일을 **시스템 제공 스테이지**파일로 구분하기로 한다.  
<br>

이를 위해 데이터 저장 시 **Stage**로 시작하는 파일명으로 저장할 경우 저장 불가 창을 띄운다.  
<br>

```cpp
if (saveFileName.StartsWith("stage")) { alertWindow.ShowSingleAlertWindow("저장 파일명은 stage로 시작할 수 없습니다."); return; }
```
<br>

### 데이터의 무결성 확인

특정 이름으로의 저장을 막았어도 파일 경로로 이동하여 파일명을 바꾸는 방법이 있을 수 있다.  
<br>

이를 위하여 **sha-256**해시 암호를 이용하기로 한다.  
<br>

스테이지 제공 노트맵 파일의 **sha-256**값을 저장해둔 뒤 메뉴 씬이 로드될 때 데이터 경로에 있는 **스테이지 제공 파일**들의 암호값을 비교하여
파일이 하나라도 없거나 암호값이 일치하지 않으면 모든 해당되는 **스테이지 제공 파일**을 제거한 뒤 다시 다운로드 하는 방식을 이용한다.  
<br>

![기본파일해시이름](https://github.com/don72-s/RSH_demo/assets/66211881/c1d10136-1f5c-4cb3-a1fb-3833af4e7650)


<br>

파일의 존재, 무결성 확인 후 재다운로드하는 코드.  
<br>
```cpp
//dat파일 확인 (노트파일과 해시 무결성 체크)
if (checkDefaultFilesExist(fileInfoSO))
{
    InitElements(nameList);
    return;
}

//깨진 파일들이 감지되었으므로 파일들 제거
RemoveFiles(fileInfoSO.GetFileNames());

//파일들을 다운로드
foreach (string _fileName in nameList)
{
    StartCoroutine(AndroidUnpackingNoteFile(_fileName));
}

StartCoroutine(AndroidNoteFilesDownloadCheck(fileInfoSO));
```
<br>





