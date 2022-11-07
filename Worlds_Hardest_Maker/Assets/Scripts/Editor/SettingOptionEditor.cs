using System.Collections;
using System.Collections.Generic;
using System.Drawing.Printing;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(SettingOption))]
public class SettingOptionEditor : Editor
{
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();

        SettingOption script = (SettingOption)target;
        script.Response();
    }
}

[CustomEditor(typeof(DropdownMenu))]
public class DropdownMenuEditor : SettingOptionEditor { }
