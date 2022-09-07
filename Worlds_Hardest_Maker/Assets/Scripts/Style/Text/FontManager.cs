using System.Collections;
using System.Collections.Generic;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

public class FontManager : MonoBehaviour
{
    public TMPro.TMP_FontAsset defaultFont;
    public List<TMPro.TMP_Text> ignore = new();

    public void ApplyDefaultFont()
    {
        TMPro.TMP_Text[] texts = Resources.FindObjectsOfTypeAll<TMPro.TMP_Text>();

        foreach(TMPro.TMP_Text text in texts)
        {
            if (!ignore.Contains(text))
            {
                text.font = defaultFont;
            }
        }
    }
}

#if UNITY_EDITOR
[CustomEditor(typeof(FontManager))]
public class FontManagerEditor : Editor
{
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();

        FontManager script = (FontManager)target;
        if (GUILayout.Button("Apply Default Font"))
        {
            script.ApplyDefaultFont();
        }
    }
}
#endif