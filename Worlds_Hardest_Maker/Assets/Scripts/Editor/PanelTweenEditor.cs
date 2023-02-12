using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(PanelTween))]
public class PanelTweenEditor : Editor
{
    public override void OnInspectorGUI()
    {
        PanelTween script = (PanelTween)target;

        base.OnInspectorGUI();

        if (Application.isPlaying && GUILayout.Button("Toggle"))
        {
            script.Toggle();
        }
    }
}