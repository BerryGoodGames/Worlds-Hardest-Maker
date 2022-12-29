﻿using TMPro;
using UnityEngine;

public class NumberInput : MonoBehaviour
{
    [SerializeField] private float step;
    [SerializeField] private bool noMaxLimit;
    [SerializeField] private bool noMinLimit;
    [SerializeField] private float min;
    [SerializeField] private float max;
    [Space] public TMP_InputField numberInput;
    private NumberInputTween tweenController;

    public void Increase()
    {
        float increased = GetCurrentNumber() + step;

        if (!noMaxLimit && increased > max) return;

        SetNumberText(increased);
        tweenController.IncreaseTween();
    }

    public void Decrease()
    {
        float decreased = GetCurrentNumber() - step;

        if (!noMinLimit && decreased < min) return;

        SetNumberText(decreased);
        tweenController.DecreaseTween();
    }

    public void SetNumberText(float num)
    {
        numberInput.text = num.ToString();
    }

    public float GetCurrentNumber()
    {
        return float.Parse(numberInput.text.Replace("​" /* Zero Space Character */, ""));
    }

    private void Start()
    {
        tweenController = GetComponent<NumberInputTween>();
    }
}