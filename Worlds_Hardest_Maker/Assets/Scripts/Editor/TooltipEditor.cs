using UnityEditor;



#if UNITY_EDITOR
[CustomEditor(typeof(Tooltip))]
public class TooltipEditor : Editor
{
    SerializedProperty text;
    SerializedProperty fontSize;
    SerializedProperty customTweenDelay;
    SerializedProperty tweenDelay;

    public override void OnInspectorGUI()
    {
        Tooltip script = (Tooltip)target;

        EditorGUILayout.PropertyField(text);
        EditorGUILayout.PropertyField(fontSize);
        EditorGUILayout.PropertyField(customTweenDelay);
        if(script.customTweenDelay) 
            EditorGUILayout.PropertyField(tweenDelay);

        serializedObject.ApplyModifiedProperties();
    }

    private void OnEnable()
    {
        text = serializedObject.FindProperty("text");
        fontSize = serializedObject.FindProperty("fontSize");
        customTweenDelay = serializedObject.FindProperty("customTweenDelay");
        tweenDelay = serializedObject.FindProperty("tweenDelay");
    }
}
#endif