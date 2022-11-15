using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FontManager : MonoBehaviour
{
    public TMPro.TMP_FontAsset defaultFont;
    public List<TMPro.TMP_Text> ignore = new();

    public void ApplyDefaultFont()
    {
        TMPro.TMP_Text[] texts = Resources.FindObjectsOfTypeAll<TMPro.TMP_Text>();

        foreach(TMPro.TMP_Text text in texts)
        {
            if (!ignore.Contains(text))
            {
                text.font = defaultFont;
            }
        }
    }
}