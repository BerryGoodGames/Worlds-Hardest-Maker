using System.Collections.Generic;
using MyBox;
using TMPro;
using UnityEngine;

public class FontManager : MonoBehaviour
{
    public TMP_FontAsset DefaultFont;
    public List<TMP_Text> Ignore = new();

    [ButtonMethod]
    public void ApplyDefaultFont()
    {
        TMP_Text[] texts = Resources.FindObjectsOfTypeAll<TMP_Text>();

        foreach (TMP_Text text in texts)
        {
            if (!Ignore.Contains(text)) text.font = DefaultFont;
        }
    }
}