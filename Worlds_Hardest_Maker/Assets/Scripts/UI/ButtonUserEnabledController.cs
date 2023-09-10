using System.Collections.Generic;
using MyBox;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ButtonUserEnabledController : MonoBehaviour
{
    [SerializeField] private List<Image> imageList;
    [SerializeField] private TMP_Text buttonText;
    [SerializeField] private Button button;
    [SerializeField] private ButtonTween buttonTween;
    [Space] [SerializeField] private Color enabledColor;
    [SerializeField] private Color disabledColor;
    [SerializeField] private Color enabledTextColor;
    [SerializeField] private Color disabledTextColor;
    [Separator] [SerializeField] private bool userEnabled = true;

    [ButtonMethod]
    private void SetUserEnabled()
    {
        foreach (Image image in imageList)
        {
            image.color = userEnabled ? enabledColor : disabledColor;
        }

        buttonText.color = userEnabled ? enabledTextColor : disabledTextColor;

        button.interactable = userEnabled;

        buttonTween.enabled = userEnabled;
    }
}