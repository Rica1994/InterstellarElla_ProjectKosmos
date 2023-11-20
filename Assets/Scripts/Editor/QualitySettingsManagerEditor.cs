using UnityEngine;
using UnityEditor;
using System;
using System.IO;
using System.Text;

[CustomEditor(typeof(QualitySettingsManager))]
public class QualitySettingsManagerEditor : Editor
{
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();

        // Manually draw each property except 'qualityLevels'
        SerializedProperty prop = serializedObject.GetIterator();
        bool enterChildren = true;
        while (prop.NextVisible(enterChildren))
        {
            enterChildren = false;
            if (prop.name != "qualityLevels") // Skip the 'qualityLevels' property
            {
                EditorGUILayout.PropertyField(prop, true);
            }
        }

        // Add a button for generating the QualityLevel enum
        if (GUILayout.Button("Generate Quality Level Enum"))
        {
            GenerateQualityLevelEnum();
        }

        // Custom drawing for 'qualityLevels' array
        QualitySettingsManager manager = (QualitySettingsManager)target;
        EditorGUI.BeginChangeCheck();
    }

    private void GenerateQualityLevelEnum()
    {
        string filePath = @"C:\Unity\InterstellarEllaGithub\Assets\Scripts\Manager\QualitySettingsManager.cs";

        // Check if file exists
        if (!File.Exists(filePath))
        {
            Debug.LogError("File not found: " + filePath);
            return;
        }

        string fileContent = File.ReadAllText(filePath);

        StringBuilder newEnumContent = new StringBuilder();
        newEnumContent.AppendLine("public enum QualityRank");
        newEnumContent.AppendLine("{");

        int value = 1; // Start with 1 for the first enum value
        int combinedValue = 0; // To accumulate all values for 'None'
        foreach (var name in QualitySettings.names)
        {
            string validEnumName = name.Replace(" ", "_").Replace("-", "_");
            newEnumContent.AppendLine($"    {validEnumName} = {value},");
            combinedValue |= value; // Combine the value
            value *= 2; // Double the value for each subsequent enum member
        }

        // Add 'None' at the end with the combined value of all other settings
        newEnumContent.AppendLine($"    None = {combinedValue}");

        newEnumContent.AppendLine("}");

        string pattern = @"public\s+enum\s+QualityRank\s*{\s*(?:[^\}]*\s*)*}";
        string modifiedContent = System.Text.RegularExpressions.Regex.Replace(fileContent, pattern, newEnumContent.ToString(), System.Text.RegularExpressions.RegexOptions.Singleline);

        if (modifiedContent == fileContent)
        {
            Debug.LogError("No changes made to the file. Regular expression might not have matched.");
            return;
        }

        File.WriteAllText(filePath, modifiedContent);
        Debug.Log("QualityRank enum updated successfully.");

        AssetDatabase.Refresh();
    }

    private void OnSceneGUI()
    {
        var qualitySettingsManager = (QualitySettingsManager)target;

        if (qualitySettingsManager != null)
        {
            qualitySettingsManager.AdjustCameraAspectRatios();
        }
    }
}
