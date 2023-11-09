#if UNITY_EDITOR
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[CanEditMultipleObjects]
[CustomEditor(typeof(ObstacleRoot))]
public class ObstacleRootEditor : Editor
{
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();
        
        ObstacleRoot obstacleRoot = (ObstacleRoot)target;

        bool destroyOnHit = serializedObject.FindProperty("_destroyOnHit").boolValue;
        
        // Show the additional fields only when _destroyOnHit is true
        if (destroyOnHit)
        {
            // SerializedProperty for _destroyTrigger
            SerializedProperty destroyTriggerProperty = serializedObject.FindProperty("_destroyTrigger");
            EditorGUILayout.PropertyField(destroyTriggerProperty);

            // SerializedProperty for _destroyAfterWhile
            SerializedProperty destroyAfterWhileProperty = serializedObject.FindProperty("_destroyAfterWhile");
            EditorGUILayout.PropertyField(destroyAfterWhileProperty);

            // Access _knockBackMultiplierComponent using the serialized field
            if (obstacleRoot.KnockBackMultiplierComponent != null && destroyTriggerProperty.boolValue == false)
            {
                destroyAfterWhileProperty.floatValue = obstacleRoot.KnockBackMultiplierComponent.Time + 0.05f;
                if (obstacleRoot.KnockBackMultiplierComponent.ShouldLerpIn && obstacleRoot.KnockBackMultiplierComponent.LerpInSpeed > 0.001f)
                {
                    destroyAfterWhileProperty.floatValue += 1 / obstacleRoot.KnockBackMultiplierComponent.LerpInSpeed;
                }
                
                if (obstacleRoot.KnockBackMultiplierComponent.ShouldLerpOut && obstacleRoot.KnockBackMultiplierComponent.LerpOutSpeed > 0.001f)
                {
                    destroyAfterWhileProperty.floatValue += 1 / obstacleRoot.KnockBackMultiplierComponent.LerpOutSpeed;
                }
            }
            
            // Apply modifications to serializedObject
            serializedObject.ApplyModifiedProperties();
        }
        
        // Set the button size
        float buttonWidth = 200f;
        float buttonHeight = 20f;

        // Calculate the position to center the button
        float centerX = (EditorGUIUtility.currentViewWidth - buttonWidth) * 0.5f;
        Rect buttonRect = new Rect(centerX, GUILayoutUtility.GetLastRect().y + 5, buttonWidth, buttonHeight);

        // Set the space size above the button
        float spaceAboveButton = 10f;
        
        // Add space above the button
        GUILayout.Space(spaceAboveButton);
        
        EditorGUILayout.BeginHorizontal();
        GUILayout.FlexibleSpace();
        
        EditorGUI.BeginChangeCheck();
       // obstacleRoot.PrintColliders();
        
        // Create the button
        if (GUILayout.Button("Collect Colliders", GUILayout.Width(buttonWidth), GUILayout.Height(buttonHeight)))
        {
            // Record the targetScript object for undo purposes
            Undo.RecordObject(obstacleRoot, "Undo add Obstacle Collision");
            
            // Button click logic here
            obstacleRoot.CollectColliders(); 
            obstacleRoot.PrintColliders();

            EditorGUI.EndChangeCheck();
        }
        
        // Create the button
        if (GUILayout.Button("Reset Obstacle Root", GUILayout.Width(buttonWidth), GUILayout.Height(buttonHeight)))
        {
            obstacleRoot.ResetObstacleRoot();
        }

        GUILayout.FlexibleSpace();
        EditorGUILayout.EndHorizontal();
        
        
    }
}
#endif