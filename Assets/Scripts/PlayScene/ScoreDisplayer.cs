using UnityEngine;
using UnityEngine.UI;

public class ScoreDisplayer : MonoBehaviour {

    [SerializeField]
    Text boardText;

    int corrrect = 0;
    int good = 0;
    int fail = 0;

    public void ResetBoard() {

        corrrect = 0;
        good = 0;
        fail = 0;
        UpdateBoard();

    }

    private void UpdateBoard() {

        boardText.text = $"Correct : {corrrect}\nGood : {good}\nFail : {fail}";

    }

    public void AddCorrect(int _val = 1) {

        corrrect += _val;
        UpdateBoard();

    }

    public void AddGood(int _val = 1) {

        good += _val;
        UpdateBoard();

    }

    public void AddFail(int _val = 1) {

        fail += _val;
        UpdateBoard();

    }

}
