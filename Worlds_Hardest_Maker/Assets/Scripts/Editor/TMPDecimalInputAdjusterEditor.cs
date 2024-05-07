using UnityEditor;

[CustomEditor(typeof(TMPDecimalInputAdjuster))]
public class TMPDecimalInputAdjusterEditor : Editor
{
    private SerializedProperty forbidNegative;
    private SerializedProperty forbidDecimals;
    private SerializedProperty roundToStep;
    private SerializedProperty stepValue;
    private SerializedProperty maxLimit;
    private SerializedProperty minLimit;
    private SerializedProperty max;
    private SerializedProperty min;
    
    public override void OnInspectorGUI()
    {
        TMPDecimalInputAdjuster script = (TMPDecimalInputAdjuster)target;
        
        EditorGUILayout.PropertyField(forbidNegative);
        EditorGUILayout.PropertyField(forbidDecimals);
        EditorGUILayout.PropertyField(roundToStep);
        
        if (!script.ForbidDecimals && script.RoundToStep) EditorGUILayout.PropertyField(stepValue);
        
        EditorGUILayout.PropertyField(maxLimit);
        EditorGUILayout.PropertyField(minLimit);
        EditorGUILayout.PropertyField(max);
        EditorGUILayout.PropertyField(min);
        
        if (script.StepValue == 0) script.StepValue = 1;
        
        serializedObject.ApplyModifiedProperties();
    }
    
    private void OnEnable()
    {
        forbidNegative = serializedObject.FindProperty("forbidNegative");
        forbidDecimals = serializedObject.FindProperty("ForbidDecimals");
        roundToStep = serializedObject.FindProperty("RoundToStep");
        stepValue = serializedObject.FindProperty("StepValue");
        maxLimit = serializedObject.FindProperty("maxLimit");
        minLimit = serializedObject.FindProperty("minLimit");
        max = serializedObject.FindProperty("max");
        min = serializedObject.FindProperty("min");
    }
}