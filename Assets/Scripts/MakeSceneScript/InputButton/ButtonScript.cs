using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class ButtonScript : MonoBehaviour, IPointerClickHandler {

    protected Image myImg;
    private NoteType noteType;


    [SerializeField]
    protected Image boarderImg;

    [SerializeField]
    List<NoteIcon> spriteList;

    private Dictionary<NoteType, Sprite> noteSpriteDic;

    [System.Serializable]
    public struct NoteIcon {

        public NoteType noteType;
        public Sprite sprite;

    }

    //del
    //private static int idxCounter = 0;

    // Start is called before the first frame update
    protected virtual void Awake() {
        noteType = NoteType.NONE;
        myImg = gameObject.GetComponent<Image>();

        noteSpriteDic = new Dictionary<NoteType, Sprite>();

        foreach (NoteIcon _icon in spriteList) {

            if (!noteSpriteDic.ContainsKey(_icon.noteType)) {
                noteSpriteDic.Add(_icon.noteType, _icon.sprite);
            }

        }

        SetBoarderImgActive(false);
    }

    public void SetInteractable(bool _isIteract) {
        myImg.raycastTarget = _isIteract;
    }

    public void SetBoarderImgActive(bool _isActive) {
        boarderImg.gameObject.SetActive(_isActive);
    }

    public void OnPointerClick(PointerEventData eventData) {
        if (eventData.button == PointerEventData.InputButton.Left) {

            switch (noteType) {

                case NoteType.NONE:
                    SetNoteType(NoteType.DOWN_NOTE);
                    break;

                case NoteType.DOWN_NOTE:
                    SetNoteType(NoteType.UPPER_NOTE);
                    break;

                case NoteType.UPPER_NOTE:
                    SetNoteType(NoteType.INVERSE_DOWN_NOTE);
                    break;

                case NoteType.INVERSE_DOWN_NOTE:
                    SetNoteType(NoteType.INVERSE_UPPER_NOTE);
                    break;

                case NoteType.INVERSE_UPPER_NOTE:
                    SetNoteType(NoteType.NONE);
                    break;

                default:
                    break;

            }

        } else if (eventData.button == PointerEventData.InputButton.Right) {//에디터용 디버그

            SetNoteType(NoteType.NONE);

        }
    }



    public void ClearNoteState() {
        SetNoteType(NoteType.NONE);
    }

    virtual public void SetNoteType(NoteType _noteType) {

        myImg.sprite = noteSpriteDic[_noteType];
        noteType = _noteType;
    }

    public NoteType GetNoteType() {

        return noteType;

    }

}
