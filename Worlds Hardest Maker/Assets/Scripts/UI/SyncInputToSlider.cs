using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEditor;
using TMPro;
#if UNITY_EDITOR
using UnityEditor.Events;
#endif

[ExecuteInEditMode]
public class SyncInputToSlider : MonoBehaviour
{
    [SerializeField] private Slider slider;
    private TMP_InputField input;

    private void Start()
    {
        UpdateInput();
    }

    /// <summary>
    /// Updates the slider
    /// </summary>
    /// <param name="val">value that the slider is set to</param>
    public void UpdateSlider()
    {
        // try to read input text and set slider value
        if(float.TryParse(input.text, out float value)) slider.value = value;
    }

    /// <summary>
    /// Updates the input
    /// </summary>
    /// <param name="val">value that the input is set to</param>
    public void UpdateInput()
    {
        if(input == null) input = GetComponent<TMP_InputField>();
        // convert slider value to text and put in into the input
        input.text = slider.value.ToString();
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
        UnityEventTools.AddPersistentListener(slider.onValueChanged, (float input) => { UpdateInput(); }); ; // add Update Input to persistnent event listener
#endif
    }
}


#if UNITY_EDITOR
[CustomEditor(typeof(SyncInputToSlider))]
public class SyncInputToSliderEditor : Editor
{
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();

        SyncInputToSlider script = (SyncInputToSlider)target;
        if (GUILayout.Button("Synchronise"))
        {
            script.Synchronise();
            script.UpdateInput();
        }
    }
}
#endif