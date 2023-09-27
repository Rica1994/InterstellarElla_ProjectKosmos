
#if UNITY_EDITOR

using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;

public class TriggerVisualizingTool : EditorWindow
{
    private List<SwapCameraBase> _swapCameras = new List<SwapCameraBase>();
    private List<RockWallNew> _rockWallNews = new List<RockWallNew>();
    private List<AnimatedEventTrigger> _animatedEventTriggers = new List<AnimatedEventTrigger>();
    private List<AutoJumpMaster> _autoJumpMasters = new List<AutoJumpMaster>();
    private List<CheckpointTrigger> _checkpointTriggers = new List<CheckpointTrigger>();


    [MenuItem("Window/Trigger visualizer")]
    public static void ShowWindow()
    {
        GetWindow<TriggerVisualizingTool>("Trigger visualizer");
    }

    private void OnGUI()
    {
        ShowButtons();
    }

    public void ShowButtons()
    {
        if (GUILayout.Button("SHOW Visuals --- SwapCameras"))
        {
            _swapCameras = FindObjectsOfType<SwapCameraBase>().ToList();

            for (int i = 0; i < _swapCameras.Count; i++)
            {
                _swapCameras[i].ToggleSwapCameraVisuals(true);
            }

            EditorSceneManager.MarkSceneDirty(EditorSceneManager.GetActiveScene());
        }
        if (GUILayout.Button("HIDE Visuals --- SwapCameras"))
        {
            _swapCameras = FindObjectsOfType<SwapCameraBase>().ToList();

            for (int i = 0; i < _swapCameras.Count; i++)
            {
                _swapCameras[i].ToggleSwapCameraVisuals(false);
            }

            EditorSceneManager.MarkSceneDirty(EditorSceneManager.GetActiveScene());
        }



        if (GUILayout.Button("SHOW Visuals --- RockWalls"))
        {
            _rockWallNews = FindObjectsOfType<RockWallNew>().ToList();

            for (int i = 0; i < _rockWallNews.Count; i++)
            {
                _rockWallNews[i].ToggleColliderVisuals(true);
            }

            EditorSceneManager.MarkSceneDirty(EditorSceneManager.GetActiveScene());
        }
        if (GUILayout.Button("HIDE Visuals --- RockWalls"))
        {
            _rockWallNews = FindObjectsOfType<RockWallNew>().ToList();

            for (int i = 0; i < _rockWallNews.Count; i++)
            {
                _rockWallNews[i].ToggleColliderVisuals(false);
            }

            EditorSceneManager.MarkSceneDirty(EditorSceneManager.GetActiveScene());
        }



        if (GUILayout.Button("SHOW Visuals --- GlitchAnimatedEventTriggers"))
        {
            _animatedEventTriggers = FindObjectsOfType<AnimatedEventTrigger>().ToList();

            for (int i = 0; i < _animatedEventTriggers.Count; i++)
            {
                _animatedEventTriggers[i].ToggleTriggerVisuals(true);
            }

            EditorSceneManager.MarkSceneDirty(EditorSceneManager.GetActiveScene());
        }
        if (GUILayout.Button("HIDE Visuals --- GlitchAnimatedEventTriggers"))
        {
            _animatedEventTriggers = FindObjectsOfType<AnimatedEventTrigger>().ToList();

            for (int i = 0; i < _animatedEventTriggers.Count; i++)
            {
                _animatedEventTriggers[i].ToggleTriggerVisuals(false);
            }

            EditorSceneManager.MarkSceneDirty(EditorSceneManager.GetActiveScene());
        }


        if (GUILayout.Button("SHOW Visuals --- AutoJumpMasters"))
        {
            _autoJumpMasters = FindObjectsOfType<AutoJumpMaster>().ToList();

            for (int i = 0; i < _autoJumpMasters.Count; i++)
            {
                _autoJumpMasters[i].ToggleAutoJumpVisuals(true);
            }

            EditorSceneManager.MarkSceneDirty(EditorSceneManager.GetActiveScene());
        }
        if (GUILayout.Button("HIDE Visuals --- AutoJumpMasters"))
        {
            _autoJumpMasters = FindObjectsOfType<AutoJumpMaster>().ToList();

            for (int i = 0; i < _autoJumpMasters.Count; i++)
            {
                _autoJumpMasters[i].ToggleAutoJumpVisuals(false);
            }

            EditorSceneManager.MarkSceneDirty(EditorSceneManager.GetActiveScene());
        }

        if (GUILayout.Button("SHOW Visuals --- CheckpointTriggers"))
        {
            _checkpointTriggers = FindObjectsOfType<CheckpointTrigger>().ToList();

            for (int i = 0; i < _checkpointTriggers.Count; i++)
            {
                _checkpointTriggers[i].ToggleVisuals(true);
            }

            EditorSceneManager.MarkSceneDirty(EditorSceneManager.GetActiveScene());
        }
        if (GUILayout.Button("HIDE Visuals --- CheckpointTriggers"))
        {
            _checkpointTriggers = FindObjectsOfType<CheckpointTrigger>().ToList();

            for (int i = 0; i < _checkpointTriggers.Count; i++)
            {
                _checkpointTriggers[i].ToggleVisuals(false);
            }

            EditorSceneManager.MarkSceneDirty(EditorSceneManager.GetActiveScene());
        }
    }
}

#endif
