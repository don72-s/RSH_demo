using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BGMDropdown : MonoBehaviour
{

    Dropdown dropdown;

    // Start is called before the first frame update
    void Start()
    {
        dropdown = GetComponent<Dropdown>();
        dropdown.AddOptions(new List<string>(Enum.GetNames(typeof(BGM_TYPE))));
    }

    public BGM_TYPE GetItem() {
        return (BGM_TYPE)dropdown.value;
    }
}
