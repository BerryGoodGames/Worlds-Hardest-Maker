using TMPro;
using UnityEngine;

public class NumberInput : MonoBehaviour
{
    [SerializeField] private float step;
    [SerializeField] private bool noMaxLimit;
    [SerializeField] private bool noMinLimit;
    [SerializeField] private float min;
    [SerializeField] private float max;

    [Space] public TMP_InputField Input;

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

    public void SetNumberText(float num) => Input.text = num.ToString();

    public float GetCurrentNumber() => float.Parse(Input.text.Replace("​" /* Zero Space Character */, ""));

    private void Start() => tweenController = GetComponent<NumberInputTween>();
}