using System.Collections;
using System.Collections.Generic;
#if UNITY_EDITOR
using UnityEditor;
#endif
using UnityEngine;

public class ToolbarSizing : MonoBehaviour
{
    public Canvas canvas;

    [Space]
    public float toolbarHeight;

    public void UpdateSize()
    {
        // set height of toolbar and scale values
        RectTransform rt = transform.parent.GetComponent<RectTransform>();
        rt.sizeDelta = new(0, toolbarHeight);

        // scale tools
        foreach (Transform tool in transform)
        {
            tool.localScale = new(toolbarHeight / 100, toolbarHeight / 100);
        }

        Transform background = transform.parent.GetChild(0);
        RectTransform bgrt = background.GetComponent<RectTransform>();
        bgrt.sizeDelta = new(0, toolbarHeight + 200);
    }

    public void ScaleOptionsInOptionbars()
    {
        ToolOptionbar[] optionbars = FindObjectsOfType<ToolOptionbar>();
        foreach(ToolOptionbar optionbar in optionbars)
        {
            optionbar.ScaleOptions();
        }
    }
    public void UpdateOptionbarHeights()
    {
        ToolOptionbar[] optionbars = FindObjectsOfType<ToolOptionbar>();
        foreach (ToolOptionbar optionbar in optionbars)
        {
            optionbar.UpdateHeight();
        }
    }
}

#if UNITY_EDITOR
[CustomEditor(typeof(ToolbarSizing))]
public class ToolbarSpacingEditor : Editor
{
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();

        ToolbarSizing script = (ToolbarSizing)target;

        if (GUILayout.Button("Set Toolbar height"))
        {
            script.UpdateSize();
        }

        if(GUILayout.Button("Scale every option in optionbars"))
        {
            script.ScaleOptionsInOptionbars();
        }

        if(GUILayout.Button("Update optionbar heights"))
        {
            script.UpdateOptionbarHeights();
        }
    }

}
#endif