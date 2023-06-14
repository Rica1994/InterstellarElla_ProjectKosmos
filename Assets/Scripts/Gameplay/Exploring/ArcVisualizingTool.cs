#if UNITY_EDITOR

using UnityEngine;
using UnityEditor;
using UnityEditor.SceneManagement;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
using System.IO;
using System.Linq;
using System;

public class ArcVisualizingTool : EditorWindow
{
    private List<JumpPad> _jumpPads = new List<JumpPad>();

    private EllaExploring _ellaExploringScript;



    [MenuItem("Window/Arc Visualizing Tool")]
    public static void ShowWindow()
    {
        GetWindow<ArcVisualizingTool>("Arc Visualizing Tool");
    }

    private void OnGUI()
    {
        ShowButtons();
    }

    public void ShowButtons()
    {     
       // button parenting objects
       VisualizeAllJumpads();
    }

    private void VisualizeAllJumpads()
    {
        if (GUILayout.Button("Visualize arcs on jump pads"))
        {
            // find the player to access its gravity
            _ellaExploringScript = FindObjectOfType<EllaExploring>();

            // access all the jump pads
            _jumpPads = FindObjectsOfType<JumpPad>().ToList();

            // call for each one its method to draw the arc
            for (int i = 0; i < _jumpPads.Count; i++)
            {
                _jumpPads[i].SetTargetWithSpeed(_ellaExploringScript.Gravity);
            }

            // inform the editor that the scene has changed
            EditorSceneManager.MarkSceneDirty(EditorSceneManager.GetActiveScene());
        }
    }
}

#endif
