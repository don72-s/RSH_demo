using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SEDropdown : MonoBehaviour
{

    Dropdown dropdown;

    // Start is called before the first frame update
    void Start()
    {
        dropdown = GetComponent<Dropdown>();
        dropdown.AddOptions(new List<string>(Enum.GetNames(typeof(SE_TYPE))));
    }

    public SE_TYPE GetItem()
    {
        return (SE_TYPE)dropdown.value;
    }
}
