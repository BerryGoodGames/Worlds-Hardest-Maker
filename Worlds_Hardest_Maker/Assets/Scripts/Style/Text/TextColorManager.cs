using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class TextColorManager : MonoBehaviour
{
    public Color defaultColor;
    public List<TMP_Text> ignore = new();

    public void ApplyDefaultColor()
    {
        TMP_Text[] texts = FindObjectsOfType<TMP_Text>();

        foreach (TMP_Text text in texts)
        {
            if (!ignore.Contains(text))
            {
                text.color = defaultColor;
            }
        }
    }
}