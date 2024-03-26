using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LowerButtonScript : Button
{

    protected override void Awake()
    {
        base.Awake();
        myImg.raycastTarget = false;
        boarderImg.raycastTarget = false;
    }

    public override void SetNoteType(NoteType _noteType)
    {
        if (_noteType == NoteType.INVERSE_UPPER_NOTE) _noteType = NoteType.DOWN_NOTE;
        else if (_noteType == NoteType.INVERSE_DOWN_NOTE) _noteType = NoteType.UPPER_NOTE;
        base.SetNoteType(_noteType);
    }



}
