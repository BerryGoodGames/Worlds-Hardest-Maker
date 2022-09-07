using System.Collections;
using System.Collections.Generic;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

public class BackgroundLineSize : MonoBehaviour
{
    // order: top, right, bottom, left
    [Header("Line order: top, right, bottom, left")]
    public RectTransform[] lines;

    [Space]
    public float newLineSize;

    public void SetLineSize()
    {
        SetLineSize(newLineSize);
    }
    public void SetLineSize(float size)
    {
        for (int i = 0; i < lines.Length; i++)
        {
            RectTransform line = lines[i];
            if (i % 2 == 0) line.sizeDelta = new(0, size);
            else line.sizeDelta = new(size, 0);
        }
    }
}

#if UNITY_EDITOR
[CustomEditor(typeof(BackgroundLineSize))]
public class SomeScriptEditor : Editor
{
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();

        BackgroundLineSize script = (BackgroundLineSize)target;
        if (GUILayout.Button("Set Line Size"))
        {
            script.SetLineSize();
        }
    }
}
#endif
