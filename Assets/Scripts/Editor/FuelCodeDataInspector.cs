using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(FuelCodeData))]
public class FuelCodeDataInspector : Editor
{
    private float testWind = 10f;
    private float testSlope = 10f;
    private MoistureState testMoisture = MoistureState.Medium;

    public override void OnInspectorGUI()
    {
        serializedObject.Update();

        EditorGUILayout.LabelField("Fuel Code", EditorStyles.boldLabel);
        EditorGUILayout.PropertyField(serializedObject.FindProperty("title"));
        EditorGUILayout.PropertyField(serializedObject.FindProperty("codeGIS"));
        EditorGUILayout.PropertyField(serializedObject.FindProperty("fuelCodeID"));
        EditorGUILayout.PropertyField(serializedObject.FindProperty("baseColor"));
        EditorGUILayout.PropertyField(serializedObject.FindProperty("familyBrightness"));
        EditorGUILayout.PropertyField(serializedObject.FindProperty("familySaturation"));

        var fc = (FuelCodeData)target;
        using (new EditorGUI.DisabledScope(true))
        {
            EditorGUILayout.ColorField("Actual Color", fc.GetFamilyAdjustedColor());
        }

        EditorGUILayout.Space();
        EditorGUILayout.PropertyField(serializedObject.FindProperty("hour1"));
        EditorGUILayout.PropertyField(serializedObject.FindProperty("hour10"));
        EditorGUILayout.PropertyField(serializedObject.FindProperty("hour100"));

        DrawCurveGroup("ROS Curves", "rosVeryLow", "rosLow", "rosMedium", "rosHigh");
        DrawCurveGroup("Flame Length Curves", "flameVeryLow", "flameLow", "flameMedium", "flameHigh");
        DrawCurveGroup("Slope Curves", "slopeVeryLow", "slopeLow", "slopeMedium", "slopeHigh");

        EditorGUILayout.Space();
        EditorGUILayout.LabelField("Test Evaluation", EditorStyles.boldLabel);
        testWind = EditorGUILayout.Slider("Wind Speed", testWind, 0f, 50f);
        testSlope = EditorGUILayout.Slider("Slope Angle", testSlope, 0f, 90f);
        testMoisture = (MoistureState)EditorGUILayout.EnumPopup("Moisture", testMoisture);

        float ros = fc.CalculateROS(testWind, testMoisture);
        float flame = fc.CalculateFlameLength(testWind, testMoisture);
        float slopeFactor = fc.CalculateSlopeFactor(testSlope, testMoisture);

        EditorGUILayout.LabelField($"ROS: {ros:F2}");
        EditorGUILayout.LabelField($"Flame Length: {flame:F2} m");
        EditorGUILayout.LabelField($"Slope Factor: {slopeFactor:F2}");

        serializedObject.ApplyModifiedProperties();
    }

    private void DrawCurveGroup(string label, params string[] propertyNames)
    {
        EditorGUILayout.Space();
        EditorGUILayout.LabelField(label, EditorStyles.boldLabel);
        foreach (var propName in propertyNames)
        {
            EditorGUILayout.PropertyField(serializedObject.FindProperty(propName), new GUIContent(ObjectNames.NicifyVariableName(propName)));
        }
    }
}
