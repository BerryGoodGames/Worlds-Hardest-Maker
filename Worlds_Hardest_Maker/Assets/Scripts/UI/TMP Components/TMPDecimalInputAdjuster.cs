using System;
using MyBox;
using TMPro;
using UnityEngine;

[RequireComponent(typeof(TMP_InputField))]
public class TMPDecimalInputAdjuster : MonoBehaviour
{
    [SerializeField] private bool forbidNegative;
    public bool ForbidDecimals;

    [ConditionalField(nameof(ForbidDecimals), true)] public bool RoundToStep;

    public float StepValue;

    [SerializeField] private bool maxLimit;
    [SerializeField] private bool minLimit;
    [SerializeField] [ConditionalField(nameof(maxLimit))] private float max;
    [SerializeField] [ConditionalField(nameof(minLimit))] private float min;

    private TMP_InputField inputField;

    private void Awake() => inputField = GetComponent<TMP_InputField>();

    public void ApplyRules()
    {
        float inputFloat = inputField.GetFloatInput();

        if (forbidNegative) inputFloat = MathF.Abs(inputFloat);

        if (ForbidDecimals) { inputFloat = MathF.Round(inputFloat); }
        else
        {
            if (RoundToStep) inputFloat = MathF.Round(inputFloat / StepValue) * StepValue;
        }

        if (maxLimit) inputFloat = Mathf.Min(inputFloat, max);
        if (minLimit) inputFloat = Mathf.Max(inputFloat, min);

        inputField.text = inputFloat.ToString();
    }
}