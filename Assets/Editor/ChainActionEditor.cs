using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(ChainAction), true)]
public class ChainActionEditor : Editor
{
    private SerializedProperty _requisiteObjects;

    private void OnEnable()
    {
        // Get the SerializedProperty of the list we're interested in
        _requisiteObjects = serializedObject.FindProperty("_requisiteObjects");
    }

    public override void OnInspectorGUI()
    {
        serializedObject.Update();

        DrawDefaultInspector();


        EditorGUILayout.Space(); // Adds a little spacing

        // Add a header for "Repeat until requisite is met"
        EditorGUILayout.LabelField("Repeat options", EditorStyles.boldLabel);

        EditorGUILayout.PropertyField(serializedObject.FindProperty("_repeat"));
        
        bool repeat = serializedObject.FindProperty("_repeat").boolValue;
        if (repeat)
        {
            EditorGUILayout.PropertyField(serializedObject.FindProperty("_completeImmediatelyIfRequisiteIsMet"));
            EditorGUILayout.PropertyField(serializedObject.FindProperty("_repeatUntilRequisitIsMet"));
            EditorGUILayout.PropertyField(serializedObject.FindProperty("_repeatNumberOfTimes"));

            bool repeatUntilRequisitIsMet = serializedObject.FindProperty("_repeatUntilRequisitIsMet").boolValue;
            bool repeatNumberOfTimes = serializedObject.FindProperty("_repeatNumberOfTimes").boolValue;

            EditorGUILayout.Space(EditorGUIUtility.singleLineHeight);


            if (repeatUntilRequisitIsMet) serializedObject.FindProperty("_repeatNumberOfTimes").boolValue = false;
            else if (repeatNumberOfTimes) serializedObject.FindProperty("_repeatUntilRequisitIsMet").boolValue = false;

            if (repeatUntilRequisitIsMet)
            {
                serializedObject.FindProperty("_timesToRepeat").intValue = 0;

                // If true, draw the requisite fields
                EditorGUILayout.PropertyField(serializedObject.FindProperty("_timeUntilNextRepeat"));
                EditorGUILayout.PropertyField(serializedObject.FindProperty("_requisiteLogic"));

                // Manually draw the list
                EditorGUILayout.LabelField("Requisite Objects", EditorStyles.boldLabel);
                for (int i = 0; i < _requisiteObjects.arraySize; i++)
                {
                    SerializedProperty element = _requisiteObjects.GetArrayElementAtIndex(i);
                    EditorGUILayout.PropertyField(element, GUIContent.none);
                }

                // Buttons to add or remove elements from the list
                EditorGUILayout.BeginHorizontal();
                if (GUILayout.Button("Add Requisite"))
                {
                    _requisiteObjects.InsertArrayElementAtIndex(_requisiteObjects.arraySize);
                }
                if (GUILayout.Button("Remove Last Requisite") && _requisiteObjects.arraySize > 0)
                {
                    _requisiteObjects.DeleteArrayElementAtIndex(_requisiteObjects.arraySize - 1);
                }

                EditorGUILayout.EndHorizontal();
            }
            else if (repeatNumberOfTimes)
            {
                EditorGUILayout.PropertyField(serializedObject.FindProperty("_timesToRepeat"));
            }
        }

        serializedObject.ApplyModifiedProperties();
    }
}
