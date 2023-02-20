using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Serialization;

public class FontManager : MonoBehaviour
{
    [FormerlySerializedAs("defaultFont")] public TMP_FontAsset DefaultFont;
    [FormerlySerializedAs("ignore")] public List<TMP_Text> Ignore = new();

    public void ApplyDefaultFont()
    {
        TMP_Text[] texts = Resources.FindObjectsOfTypeAll<TMP_Text>();

        foreach (TMP_Text text in texts)
        {
            if (!Ignore.Contains(text))
            {
                text.font = DefaultFont;
            }
        }
    }
}