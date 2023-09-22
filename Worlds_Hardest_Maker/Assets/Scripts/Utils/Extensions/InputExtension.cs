using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public static class InputExtension
{
    public static float GetFloatInput(this TMP_InputField inputField)
    {
        string input = inputField.text;

        float inputFloat;

        if (input == string.Empty || !IsDigitsOnly(input))
        {
            inputFloat = 0f;
        }
        else if (!float.TryParse(input, out inputFloat))
        {
            throw new("Input was not a float");
        }
            

        return inputFloat;
    }

    private static bool IsDigitsOnly(this string str)
    {
        foreach (char c in str)
        {
            if (c is < '0' or > '9' && c != '.' && c != '-') return false;
        }

        return true;
    }
}
