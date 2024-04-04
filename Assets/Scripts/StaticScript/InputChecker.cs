using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public static class InputChecker
{
    /// <summary>
    /// inputField의 값이 양의 정수인지 확인.
    /// </summary>
    /// <param name="_field">읽어올 inputField</param>
    /// <param name="_includeZero">0을 포함할지의 여부</param>
    /// <returns>양의 정수 여부 반환.</returns>
    public static bool IsPositiveInt(InputField _field, bool _includeZero = false) {

        int ret;

        if (int.TryParse(_field.text, out ret)) {
            if ((ret == 0 && _includeZero) || ret > 0) {
                return true;
            }
        }

        return false;

    }

    /// <summary>
    /// inputField의 값이 양의 실수인지 확인.
    /// </summary>
    /// <param name="_field">읽어올 inputField</param>
    /// <param name="_includeZero">0을 포함할지의 여부</param>
    /// <returns>양의 실수 여부 반환.</returns>
    public static bool IsPositiveFloat(InputField _field, bool _includeZero = false)
    {

        float ret;

        if (float.TryParse(_field.text, out ret))
        {
            if ((ret == 0 && _includeZero) || ret > 0)
            {
                return true;
            }
        }

        return false;

    }



    /// <summary>
    /// inputfield로부터 정수를 읽어옴
    /// </summary>
    /// <param name="_field">읽어올 inputField</param>
    /// <returns>읽어온 정수를 반환. 실패시 minValue반환</returns>
    public static int GetInt(InputField _field) {

        int ret;

        if (int.TryParse(_field.text, out ret))
        {
            return ret;
        }
        else {
            return int.MinValue;
        }

    }

    /// <summary>
    /// inputfield로부터 실수를 읽어옴
    /// </summary>
    /// <param name="_field">읽어올 inputField</param>
    /// <returns>읽어온 실수를 반환. 실패시 minValue반환</returns>
    public static float GetFloat(InputField _field)
    {

        float ret;

        if (float.TryParse(_field.text, out ret))
        {
            return ret;
        }
        else
        {
            return float.MinValue;
        }

    }

}
