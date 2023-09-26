#if UNITY_EDITOR

using UnityEngine;
using UnityEditor;
using UnityEditor.SceneManagement;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
using System.IO;
using System.Linq;
using System;
using Cinemachine;

public class ArcVisualizingTool : EditorWindow
{
    private List<JumpPad> _jumpPads = new List<JumpPad>();
    private List<AutoJumpMaster> _autoJumpMasters = new List<AutoJumpMaster>();


    private EllaExploring _ellaExploringScript;

    private SpeederGround _speederGroundScript;
    private SimpleCarController _simpleCarScript;


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
            if (_ellaExploringScript == null)
            {
                Debug.LogWarning("Could not find Ella Exploring, aborting process visualizing jump-pads");
                return;
            }

            // access all the jump pads
            _jumpPads = FindObjectsOfType<JumpPad>().ToList();

            // call for each one its method to draw the arc
            for (int i = 0; i < _jumpPads.Count; i++)
            {
                _jumpPads[i].SetTargetWithSpeed(_ellaExploringScript.Gravity);

                EditorUtility.SetDirty(_jumpPads[i]);
            }

            // inform the editor that the scene has changed
            EditorSceneManager.MarkSceneDirty(EditorSceneManager.GetActiveScene());        
        }

        if (GUILayout.Button("Visualize arcs on AutoJumpMasters"))
        {
            // find the player to access its gravity
            _speederGroundScript = FindObjectOfType<SpeederGround>();
            if (_speederGroundScript == null)
            {
                _simpleCarScript = FindObjectOfType<SimpleCarController>();

                if (_simpleCarScript == null)
                {
                    Debug.LogWarning("Could not find SpeederGround or SimpleCar, aborting process visualizing auto-jumps");
                    return;
                }
            }


            _autoJumpMasters = FindObjectsOfType<AutoJumpMaster>().ToList();


            // call for each one its method to draw the arc
            for (int i = 0; i < _autoJumpMasters.Count; i++)
            {
                _autoJumpMasters[i].CreatePerfectJumpCurveDefault();

                EditorUtility.SetDirty(_autoJumpMasters[i]);
            }

            // inform the editor that the scene has changed
            EditorSceneManager.MarkSceneDirty(EditorSceneManager.GetActiveScene());
        }
    }
}

#endif
