using MyBox;
using TMPro;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;
#if UNITY_EDITOR
using UnityEditor.Events;
#endif

[RequireComponent(typeof(TMPDecimalInputAdjuster))]
public class SyncInputToSlider : MonoBehaviour
{
    [SerializeField] [InitializationField] [MustBeAssigned] private TMPDecimalInputAdjuster numberSettings;
    [FormerlySerializedAs("input")] [InitializationField] [MustBeAssigned] public TMP_InputField Input;
    [Separator]
    [InitializationField] [MustBeAssigned] public Slider Slider;
    
    [SerializeField] [InitializationField] private uint decimals = 2;

    private void Start()
    {
        UpdateInput();
    }

    public void UpdateSlider()
    {
        // try to read input text and set slider value
        if (!float.TryParse(Input.text, out float value)) return;

        float clampedValue = Mathf.Clamp(
            Rounded(value), 
            Slider.minValue * (!numberSettings.ForbidDecimals && numberSettings.RoundToStep ? numberSettings.StepValue : 1), 
            Slider.maxValue * (!numberSettings.ForbidDecimals && numberSettings.RoundToStep ? numberSettings.StepValue : 1)
        );
        
        Input.text = clampedValue.ToString();
        Slider.value = clampedValue / (!numberSettings.ForbidDecimals && numberSettings.RoundToStep ? numberSettings.StepValue : 1);
    }

    public void UpdateInput()
    {
        if (Input == null) Input = GetComponent<TMP_InputField>();

        float value = GetCurrentSliderValue();
        
        // convert slider value to text and put in into the input
        Input.text = value.ToString();
    }
    
    public float GetCurrentSliderValue() => Rounded(Slider.value) * (!numberSettings.ForbidDecimals && numberSettings.RoundToStep ? numberSettings.StepValue : 1);

    /// <summary>
    ///     Setup for synchronisation (add event listeners etc.)
    /// </summary>
    public void Synchronise()
    {
#if UNITY_EDITOR
        Input = GetComponent<TMP_InputField>();
        numberSettings = GetComponent<TMPDecimalInputAdjuster>();

        // set stuff in input //
        UnityEventTools.AddPersistentListener(
            Input.onValueChanged,
            _ => { UpdateSlider(); }
        ); // add Update Slider to persistent event listener

        // set stuff in slider //
        UnityEventTools.AddPersistentListener(Slider.onValueChanged, _ => { UpdateInput(); });
        // add Update Input to persistent event listener
#endif
    }

    private float Rounded(float value) => Mathf.Round(value * Mathf.Pow(10, decimals)) * Mathf.Pow(10, -decimals);
}