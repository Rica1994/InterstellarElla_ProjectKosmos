#if UNITY_EDITOR

using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;

public class VisualizeCollidersTool : EditorWindow
{
    private List<VisualizeCollider> _visualizeColliders = new List<VisualizeCollider>();


    [MenuItem("Window/Visualize Colliders Tool")]
    public static void ShowWindow()
    {
        GetWindow<VisualizeCollidersTool>("Visualize Collider Tool");
    }

    private void OnGUI()
    {
        ShowButtons();
    }

    public void ShowButtons()
    {
        if (GUILayout.Button("Toggle Colliders"))
        {
            _visualizeColliders = FindObjectsOfType<VisualizeCollider>().ToList();
            
            if (_visualizeColliders.Count > 0)
            {
                bool drawCollider = !_visualizeColliders[0].DrawCollider;

                for (int i = 0; i < _visualizeColliders.Count; i++)
                {
                    _visualizeColliders[i].ToggleVisuals(drawCollider);
                }
            }
        }
    }
}
#endif