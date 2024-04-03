using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

[Serializable]
public class UserData {

    public UserData(float _defaultUpper, float _defaultLower) { 
        upperOffset = _defaultUpper;
        lowerOffset = _defaultLower;
    }

    public float upperOffset { get; set; }
    public float lowerOffset { get; set; }
    
}
