using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LowerButtonScript : ButtonScript
{

    protected override void Awake()
    {
        base.Awake();
        myImg.raycastTarget = false;
        boarderImg.raycastTarget = false;
    }

    public override void SetNoteType(MusicInfoSetter.NoteType _noteType)
    {
        if (_noteType == MusicInfoSetter.NoteType.INVERSE_UPPER_NOTE) _noteType = MusicInfoSetter.NoteType.DOWN_NOTE;
        else if (_noteType == MusicInfoSetter.NoteType.INVERSE_DOWN_NOTE) _noteType = MusicInfoSetter.NoteType.UPPER_NOTE;
        base.SetNoteType(_noteType);
    }



}
