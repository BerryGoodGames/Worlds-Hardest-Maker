using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Serialization;

public class TextColorManager : MonoBehaviour
{
    [FormerlySerializedAs("defaultColor")] public Color DefaultColor;
    [FormerlySerializedAs("ignore")] public List<TMP_Text> Ignore = new();

    public void ApplyDefaultColor()
    {
        TMP_Text[] texts = FindObjectsOfType<TMP_Text>();

        foreach (TMP_Text text in texts)
        {
            if (!Ignore.Contains(text))
            {
                text.color = DefaultColor;
            }
        }
    }
}