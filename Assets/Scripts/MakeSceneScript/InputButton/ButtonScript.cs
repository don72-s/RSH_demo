﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class ButtonScript : MonoBehaviour, IPointerClickHandler
{

    protected Image myImg;
    private MusicInfoSetter.NoteType noteType;


    [SerializeField]
    protected Image boarderImg;

    [SerializeField]
    List<NoteIcon> spriteList;

    private Dictionary<MusicInfoSetter.NoteType, Sprite> noteSpriteDic;

    [System.Serializable]
    public struct NoteIcon {

        public MusicInfoSetter.NoteType noteType;
        public Sprite sprite;

    }

    private static int iidd = 0;

    // Start is called before the first frame update
    protected virtual void Awake()
    {
        noteType = MusicInfoSetter.NoteType.NONE;
        myImg = gameObject.GetComponent<Image>();

        noteSpriteDic = new Dictionary<MusicInfoSetter.NoteType, Sprite>();

        foreach (NoteIcon _icon in spriteList) {

            if (!noteSpriteDic.ContainsKey(_icon.noteType)) {
                noteSpriteDic.Add(_icon.noteType, _icon.sprite);
            }

        }
        Debug.Log(iidd++);

        SetBoarderImg(false);
    }

    public void SetInteractable(bool _isIteract) {
        myImg.raycastTarget = _isIteract;
    }

    public void SetBoarderImg(bool _isActive) { 
        boarderImg.gameObject.SetActive(_isActive);
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if (eventData.button == PointerEventData.InputButton.Left) {

            switch (noteType) {

                case MusicInfoSetter.NoteType.NONE:
                    SetNoteType(MusicInfoSetter.NoteType.DOWN_NOTE);
                    break;

                case MusicInfoSetter.NoteType.DOWN_NOTE:
                    SetNoteType(MusicInfoSetter.NoteType.UPPER_NOTE);
                    break;

                case MusicInfoSetter.NoteType.UPPER_NOTE:
                    SetNoteType(MusicInfoSetter.NoteType.INVERSE_DOWN_NOTE);
                    break;

                case MusicInfoSetter.NoteType.INVERSE_DOWN_NOTE:
                    SetNoteType(MusicInfoSetter.NoteType.INVERSE_UPPER_NOTE);
                    break;

                case MusicInfoSetter.NoteType.INVERSE_UPPER_NOTE:
                    SetNoteType(MusicInfoSetter.NoteType.DOWN_NOTE);
                    break;

                default:
                    break;

            }

        } else if (eventData.button == PointerEventData.InputButton.Right) {

            SetNoteType(MusicInfoSetter.NoteType.NONE);

        }
    }



    public void ClearNoteState() {
        SetNoteType(MusicInfoSetter.NoteType.NONE);
    }

    virtual public void SetNoteType(MusicInfoSetter.NoteType _noteType) {

        myImg.sprite = noteSpriteDic[_noteType];
        noteType = _noteType;
    }

    public MusicInfoSetter.NoteType GetNoteType() {

        return noteType;

    }

}