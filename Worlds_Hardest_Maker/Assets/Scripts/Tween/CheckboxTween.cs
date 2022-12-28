using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class CheckboxTween : MonoBehaviour
{
    [SerializeField] private RectTransform checkMark;
    [SerializeField] private Toggle toggle;
    [Space]
    [SerializeField] private float duration;
    [SerializeField] private Ease easeType;

    private bool isChecked;

    public void SetCheck(bool check)
    {
        switch (isChecked)
        {
            case true when !check:
                // the frame setting to unchecked
                checkMark.DOScale(Vector2.zero, duration)
                    .SetEase(easeType);
                break;

            case false when check:
                // the frame setting to checked
                checkMark.DOScale(new Vector2(1, 1), duration)
                    .SetEase(easeType);
                break;
        }

        isChecked = check;
    }

    private void Start()
    {
        isChecked = toggle.isOn;

        checkMark.localScale = isChecked ? new(1, 1) : Vector2.zero;
    }
}
