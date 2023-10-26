using UnityEditor;

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

[CustomEditor(typeof(DropdownMenuOption))]
public class DropdownMenuOptionEditor : SettingOptionEditor
{
}

[CustomEditor(typeof(NumberInputOption))]
public class NumberInputOptionEditor : SettingOptionEditor
{
}

[CustomEditor(typeof(SliderOption))]
public class SliderOptionEditor : SettingOptionEditor
{
}