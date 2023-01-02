using UnityEditor;


[CustomEditor(typeof(Tooltip))]
public class TooltipEditor : Editor
{
    private SerializedProperty text;
    private SerializedProperty fontSize;
    private SerializedProperty customTweenDelay;
    private SerializedProperty tweenDelay;

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