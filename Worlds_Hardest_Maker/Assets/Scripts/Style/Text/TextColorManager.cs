using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TextColorManager : MonoBehaviour
{
    public Color defaultColor;
    public List<TMPro.TMP_Text> ignore = new();

    public void ApplyDefaultColor()
    {
        TMPro.TMP_Text[] texts = FindObjectsOfType<TMPro.TMP_Text>();

        foreach (TMPro.TMP_Text text in texts)
        {
            if (!ignore.Contains(text))
            {
                text.color = defaultColor;
            }
        }
    }
}