using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
#if UNITY_EDITOR
using UnityEditor.Events;
#endif

[ExecuteInEditMode]
public class SyncInputToSlider : MonoBehaviour
{
    [SerializeField] private Slider slider;
    [SerializeField] private float decimals = 2;
    private TMP_InputField input;

    private void Start()
    {
        UpdateInput();
    }

    /// <summary>
    /// Updates the slider
    /// </summary>
    public void UpdateSlider()
    {
        // try to read input text and set slider value
        if (float.TryParse(input.text, out float value)) slider.value = Rounded(value);
    }

    /// <summary>
    /// Updates the input
    /// </summary>
    public void UpdateInput()
    {
        if(input == null) input = GetComponent<TMP_InputField>();
        // convert slider value to text and put in into the input
        input.text = Rounded(slider.value).ToString();
    }

    /// <summary>
    /// setup for synchronisation (add event listeners etc.)
    /// </summary>
    public void Synchronise()
    {
#if UNITY_EDITOR
        input = GetComponent<TMP_InputField>();

        // set stuff in input //
        UnityEventTools.AddPersistentListener(input.onValueChanged, (string input) => { UpdateSlider(); }); // add Update Slider to persistent event listener

        // set stuff in slider //
        UnityEventTools.AddPersistentListener(slider.onValueChanged, (float input) => { UpdateInput(); }); ; // add Update Input to persistent event listener
#endif
    }

    private float Rounded(float value) { return Mathf.Round(value * Mathf.Pow(10, decimals)) * Mathf.Pow(10, -decimals); }
}