using TMPro;
using UnityEngine;
using UnityEngine.UI;
#if UNITY_EDITOR
using UnityEditor.Events;
#endif

[ExecuteInEditMode]
public class SyncInputToSlider : MonoBehaviour
{
    [SerializeField] private Slider slider;
    [SerializeField] private float decimals = 2;
    private TMP_InputField input;

    private void Start() => UpdateInput();

    public void UpdateSlider()
    {
        // try to read input text and set slider value
        if (float.TryParse(input.text, out float value)) slider.value = Rounded(value);
    }

    public void UpdateInput()
    {
        if (input == null) input = GetComponent<TMP_InputField>();
        // convert slider value to text and put in into the input
        input.text = Rounded(slider.value).ToString();
    }

    /// <summary>
    ///     Setup for synchronisation (add event listeners etc.)
    /// </summary>
    public void Synchronise()
    {
#if UNITY_EDITOR
        input = GetComponent<TMP_InputField>();

        // set stuff in input //
        UnityEventTools.AddPersistentListener(input.onValueChanged,
            _ => { UpdateSlider(); }); // add Update Slider to persistent event listener

        // set stuff in slider //
        UnityEventTools.AddPersistentListener(slider.onValueChanged, _ => { UpdateInput(); });
        // add Update Input to persistent event listener
#endif
    }

    private float Rounded(float value) => Mathf.Round(value * Mathf.Pow(10, decimals)) * Mathf.Pow(10, -decimals);
}