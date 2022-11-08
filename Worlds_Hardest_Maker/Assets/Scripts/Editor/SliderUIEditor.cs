using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(SliderUI))]
public class SliderUIEditor : Editor
{
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();

        SliderUI slider = (SliderUI)target;
        slider.Response();
    }
}
