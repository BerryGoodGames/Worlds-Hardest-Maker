using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(SettingOption))]
public class SettingOptionEditor : Editor
{
    public override void OnInspectorGUI()
    {
        SettingOption script = (SettingOption)target;

        base.OnInspectorGUI();

        script.UpdateHeight();
        script.UpdateFontSize();
    }

}
