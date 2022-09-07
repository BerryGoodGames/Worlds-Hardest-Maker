using System.Collections;
using System.Collections.Generic;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

public class TextColorManager : MonoBehaviour
{
    public Color defaultColor;
    public List<TMPro.TMP_Text> ignore = new();

    public void ApplyDefaultColor()
    {
        TMPro.TMP_Text[] texts = FindObjectsOfType<TMPro.TMP_Text>();

        foreach (TMPro.TMP_Text text in texts)
        {
            if (!ignore.Contains(text))
            {
                text.color = defaultColor;
            }
        }
    }
}

#if UNITY_EDITOR
[CustomEditor(typeof(TextColorManager))]
public class TextColorManagerEditor : Editor
{
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();

        TextColorManager script = (TextColorManager)target;
        if (GUILayout.Button("Apply Default Color"))
        {
            script.ApplyDefaultColor();
        }
    }
}
#endif