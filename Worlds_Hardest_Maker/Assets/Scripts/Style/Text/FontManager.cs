using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class FontManager : MonoBehaviour
{
    public TMP_FontAsset defaultFont;
    public List<TMP_Text> ignore = new();

    public void ApplyDefaultFont()
    {
        TMP_Text[] texts = Resources.FindObjectsOfTypeAll<TMP_Text>();

        foreach (TMP_Text text in texts)
        {
            if (!ignore.Contains(text))
            {
                text.font = defaultFont;
            }
        }
    }
}