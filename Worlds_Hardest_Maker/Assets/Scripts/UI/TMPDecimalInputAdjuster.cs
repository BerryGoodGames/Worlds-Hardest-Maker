using System;
using MyBox;
using TMPro;
using UnityEngine;

[RequireComponent(typeof(TMP_InputField))]
public class TMPDecimalInputAdjuster : MonoBehaviour
{
    [SerializeField] private bool forbidNegative;
    public bool ForbidDecimals;

    [ConditionalField(nameof(ForbidDecimals), true)]
    public bool RoundToStep;

    public float StepValue;

    private TMP_InputField inputField;

    private void Awake() => inputField = GetComponent<TMP_InputField>();

    public void ApplyRules()
    {

        float inputFloat = inputField.GetFloatInput();

        if (forbidNegative)
        {
            // input = input.Replace("-", "");
            inputFloat = MathF.Abs(inputFloat);
        }

        if (ForbidDecimals)
            inputFloat = MathF.Round(inputFloat);
        else
        {
            if (RoundToStep) inputFloat = MathF.Round(inputFloat / StepValue) * StepValue;
        }

        inputField.text = inputFloat.ToString();
    }
}