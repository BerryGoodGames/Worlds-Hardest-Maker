using MyBox;
using TMPro;
using UnityEngine;
using UnityEngine.Events;

public class NumberInput : MonoBehaviour
{
    [SerializeField] [PositiveValueOnly] private float step;
    [SerializeField] private bool noMaxLimit;
    [SerializeField] private bool noMinLimit;
    [SerializeField] [ConditionalField(nameof(noMinLimit), true)] private float min;
    [SerializeField] [ConditionalField(nameof(noMaxLimit), true)] private float max;

    [Space] public TMP_InputField Input;
    [Space] public UnityEvent OnChange;

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
        Input.text = num.ToString();
        OnChange.Invoke();
    }

    public float GetCurrentNumber() => float.Parse(Input.text.Replace("​" /* Zero Space Character */, ""));

    public void InvokeChangeEvent() => OnChange.Invoke();

    private void Start() => tweenController = GetComponent<NumberInputTween>();
}