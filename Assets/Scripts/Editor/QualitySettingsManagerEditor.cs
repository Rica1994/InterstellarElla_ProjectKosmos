using UnityEngine;
using UnityEditor;
using System;

[CustomEditor(typeof(QualitySettingsManager))]
public class QualitySettingsManagerEditor : Editor
{
    public override void OnInspectorGUI()
    {
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

        // Add a button
        if (GUILayout.Button("Generate Quality Level Enum"))
        {
            QualityLevelEnumGenerator.GenerateQualityLevelEnum();
            EditorApplication.delayCall += UpdateQualityLevelsArray;
        }

        // Custom drawing for 'qualityLevels' array
        QualitySettingsManager manager = (QualitySettingsManager)target;
        EditorGUI.BeginChangeCheck();

        SerializedProperty qualityLevelsArray = serializedObject.FindProperty("qualityLevels");
        if (qualityLevelsArray != null && qualityLevelsArray.isArray)
        {
            for (int i = 0; i < qualityLevelsArray.arraySize; i++)
            {
                string label = ((QualityLevel)i).ToString(); // Get the name from the enum
                EditorGUILayout.LabelField(label);

                SerializedProperty gameObjectArrayProp = qualityLevelsArray.GetArrayElementAtIndex(i).FindPropertyRelative("_qualityLevelFeatures");
                if (gameObjectArrayProp != null)
                {
                    EditorGUILayout.PropertyField(gameObjectArrayProp, GUIContent.none, true);
                }
            }
        }

        if (EditorGUI.EndChangeCheck())
        {
            serializedObject.ApplyModifiedProperties();
            EditorApplication.delayCall += RepaintInspector;
        }
    }

    private void UpdateQualityLevelsArray()
    {
        QualitySettingsManager manager = (QualitySettingsManager)target;
        int numberOfQualityLevels = Enum.GetValues(typeof(QualityLevel)).Length;
        Array.Resize(ref manager.qualityLevels, numberOfQualityLevels);

        // Force the serialized object to update
        serializedObject.Update();
        EditorUtility.SetDirty(manager);
    }

    private void RepaintInspector()
    {
        Repaint();
    }
}
