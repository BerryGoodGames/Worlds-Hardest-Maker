using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(TMP_Dropdown))]
public class DropdownResize : MonoBehaviour
{
    public void UpdateToBiggestOption()
    {
        // get components
        TMP_Dropdown dropdown = GetComponent<TMP_Dropdown>();
        TMP_Text label = dropdown.captionText;
        LayoutElement labelLayout = label.GetComponent<LayoutElement>();

        List<TMP_Dropdown.OptionData> options = dropdown.options;

        // find option with biggest width
        float biggestWidth = 0;
        foreach (TMP_Dropdown.OptionData option in options)
        {
            // fetch width of option
            string text = option.text;
            Vector2 textDimensions = label.GetPreferredValues(text);

            float textWidth = textDimensions.x;

            // compare
            if (biggestWidth >= textWidth) continue;
            biggestWidth = textWidth;
        }

        // set width of label to biggest
        labelLayout.preferredWidth = biggestWidth;
    }
}