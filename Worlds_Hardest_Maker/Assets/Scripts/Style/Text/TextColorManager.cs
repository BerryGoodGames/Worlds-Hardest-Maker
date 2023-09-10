using System.Collections.Generic;
using MyBox;
using TMPro;
using UnityEngine;

public class TextColorManager : MonoBehaviour
{
    public Color DefaultColor;
    public List<TMP_Text> Ignore = new();

    [ButtonMethod]
    public void ApplyDefaultColor()
    {
        TMP_Text[] texts = FindObjectsOfType<TMP_Text>();

        foreach (TMP_Text text in texts)
        {
            if (!Ignore.Contains(text)) text.color = DefaultColor;
        }
    }
}