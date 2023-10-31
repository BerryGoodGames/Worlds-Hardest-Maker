using UnityEditor;

[CustomEditor(typeof(TMPDecimalInputAdjuster))]
public class TMPDecimalInputAdjusterEditor : Editor
{
    private SerializedProperty forbidNegative;
    private SerializedProperty forbidDecimals;
    private SerializedProperty roundToStep;
    private SerializedProperty stepValue;

    public override void OnInspectorGUI()
    {
        TMPDecimalInputAdjuster script = (TMPDecimalInputAdjuster)target;

        EditorGUILayout.PropertyField(forbidNegative);
        EditorGUILayout.PropertyField(forbidDecimals);
        EditorGUILayout.PropertyField(roundToStep);

        if (!script.ForbidDecimals && script.RoundToStep) EditorGUILayout.PropertyField(stepValue);

        if (script.StepValue == 0) script.StepValue = 1;

        serializedObject.ApplyModifiedProperties();
    }

    private void OnEnable()
    {
        forbidNegative = serializedObject.FindProperty("forbidNegative");
        forbidDecimals = serializedObject.FindProperty("ForbidDecimals");
        roundToStep = serializedObject.FindProperty("RoundToStep");
        stepValue = serializedObject.FindProperty("StepValue");
    }
}