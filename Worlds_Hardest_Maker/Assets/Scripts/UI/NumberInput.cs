using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NumberInput : MonoBehaviour
{
    [SerializeField] private float step;
    [SerializeField] private bool noMaxLimit;
    [SerializeField] private bool noMinLimit;
    [SerializeField] private float min;
    [SerializeField] private float max;
    [Space]
    public TMPro.TMP_Text numberText;
    private NumberInputTween tweenController;
    
    public void Increase()
    {
        float increased = GetCurrentNumber() + step;
        if (noMaxLimit || increased <= max)
        {
            SetNumberText(increased);
            tweenController.IncreaseTween();
        }
    }

    public void Decrease()
    {
        float decreased = GetCurrentNumber() - step;
        if (noMinLimit || decreased >= min)
        {
            SetNumberText(decreased);
            tweenController.DecreaseTween();
        }
    }

    public void SetNumberText(float num) { numberText.text = num.ToString(); }
    public float GetCurrentNumber() { return float.Parse(numberText.text); }

    private void Start()
    {
        tweenController = GetComponent<NumberInputTween>();
    }
}
