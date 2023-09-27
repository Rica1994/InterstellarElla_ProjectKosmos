
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
    private List<JumpPad> _jumpPads = new List<JumpPad>();

    private List<CheckpointTrigger> _checkpointTriggers = new List<CheckpointTrigger>();
    private List<SceneTrigger> _sceneTriggers = new List<SceneTrigger>();
    private List<DeathTrigger> _deathTriggers = new List<DeathTrigger>();
    private List<InvisibleWall> _invisibleWalls = new List<InvisibleWall>();


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
        if (GUILayout.Button("Show all triggers"))
        {
            _swapCameras = FindObjectsOfType<SwapCameraBase>().ToList();
            for (int i = 0; i < _swapCameras.Count; i++)
            {
                _swapCameras[i].ToggleSwapCameraVisuals(true);
            }
            _rockWallNews = FindObjectsOfType<RockWallNew>().ToList();
            for (int i = 0; i < _rockWallNews.Count; i++)
            {
                _rockWallNews[i].ToggleColliderVisuals(true);
            }
            _animatedEventTriggers = FindObjectsOfType<AnimatedEventTrigger>().ToList();
            for (int i = 0; i < _animatedEventTriggers.Count; i++)
            {
                _animatedEventTriggers[i].ToggleTriggerVisuals(true);
            }
            _autoJumpMasters = FindObjectsOfType<AutoJumpMaster>().ToList();
            for (int i = 0; i < _autoJumpMasters.Count; i++)
            {
                _autoJumpMasters[i].ToggleAutoJumpVisuals(true);
            }
            _jumpPads = FindObjectsOfType<JumpPad>().ToList();
            for (int i = 0; i < _jumpPads.Count; i++)
            {
                _jumpPads[i].ToggleVisuals(true);
            }
            _checkpointTriggers = FindObjectsOfType<CheckpointTrigger>().ToList();
            for (int i = 0; i < _checkpointTriggers.Count; i++)
            {
                _checkpointTriggers[i].ToggleVisuals(true);
            }
            _sceneTriggers = FindObjectsOfType<SceneTrigger>().ToList();
            for (int i = 0; i < _sceneTriggers.Count; i++)
            {
                _sceneTriggers[i].ToggleVisuals(true);
            }
            _invisibleWalls = FindObjectsOfType<InvisibleWall>().ToList();
            for (int i = 0; i < _invisibleWalls.Count; i++)
            {
                _invisibleWalls[i].ToggleVisuals(true);
            }
            _deathTriggers = FindObjectsOfType<DeathTrigger>().ToList();
            for (int i = 0; i < _deathTriggers.Count; i++)
            {
                _deathTriggers[i].ToggleVisuals(true);
            }

            EditorSceneManager.MarkSceneDirty(EditorSceneManager.GetActiveScene());
        }
        if (GUILayout.Button("Hide all triggers"))
        {
            _swapCameras = FindObjectsOfType<SwapCameraBase>().ToList();
            for (int i = 0; i < _swapCameras.Count; i++)
            {
                _swapCameras[i].ToggleSwapCameraVisuals(false);
            }
            _rockWallNews = FindObjectsOfType<RockWallNew>().ToList();
            for (int i = 0; i < _rockWallNews.Count; i++)
            {
                _rockWallNews[i].ToggleColliderVisuals(false);
            }
            _animatedEventTriggers = FindObjectsOfType<AnimatedEventTrigger>().ToList();
            for (int i = 0; i < _animatedEventTriggers.Count; i++)
            {
                _animatedEventTriggers[i].ToggleTriggerVisuals(false);
            }
            _autoJumpMasters = FindObjectsOfType<AutoJumpMaster>().ToList();
            for (int i = 0; i < _autoJumpMasters.Count; i++)
            {
                _autoJumpMasters[i].ToggleAutoJumpVisuals(false);
            }
            _jumpPads = FindObjectsOfType<JumpPad>().ToList();
            for (int i = 0; i < _jumpPads.Count; i++)
            {
                _jumpPads[i].ToggleVisuals(false);
            }
            _checkpointTriggers = FindObjectsOfType<CheckpointTrigger>().ToList();
            for (int i = 0; i < _checkpointTriggers.Count; i++)
            {
                _checkpointTriggers[i].ToggleVisuals(false);
            }
            _sceneTriggers = FindObjectsOfType<SceneTrigger>().ToList();
            for (int i = 0; i < _sceneTriggers.Count; i++)
            {
                _sceneTriggers[i].ToggleVisuals(false);
            }
            _invisibleWalls = FindObjectsOfType<InvisibleWall>().ToList();
            for (int i = 0; i < _invisibleWalls.Count; i++)
            {
                _invisibleWalls[i].ToggleVisuals(false);
            }
            _deathTriggers = FindObjectsOfType<DeathTrigger>().ToList();
            for (int i = 0; i < _deathTriggers.Count; i++)
            {
                _deathTriggers[i].ToggleVisuals(false);
            }


            EditorSceneManager.MarkSceneDirty(EditorSceneManager.GetActiveScene());
        }

        //  Specifics below //

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



        if (GUILayout.Button("SHOW Visuals --- JumpPads"))
        {
            _jumpPads = FindObjectsOfType<JumpPad>().ToList();
            for (int i = 0; i < _jumpPads.Count; i++)
            {
                _jumpPads[i].ToggleVisuals(true);
            }

            EditorSceneManager.MarkSceneDirty(EditorSceneManager.GetActiveScene());
        }
        if (GUILayout.Button("HIDE Visuals --- JumpPads"))
        {
            _jumpPads = FindObjectsOfType<JumpPad>().ToList();
            for (int i = 0; i < _jumpPads.Count; i++)
            {
                _jumpPads[i].ToggleVisuals(false);
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


        if (GUILayout.Button("SHOW Visuals --- Scene Triggers"))
        {
            _sceneTriggers = FindObjectsOfType<SceneTrigger>().ToList();

            for (int i = 0; i < _sceneTriggers.Count; i++)
            {
                _sceneTriggers[i].ToggleVisuals(true);
            }

            EditorSceneManager.MarkSceneDirty(EditorSceneManager.GetActiveScene());
        }
        if (GUILayout.Button("HIDE Visuals --- Scene Triggers"))
        {
            _sceneTriggers = FindObjectsOfType<SceneTrigger>().ToList();

            for (int i = 0; i < _sceneTriggers.Count; i++)
            {
                _sceneTriggers[i].ToggleVisuals(false);
            }

            EditorSceneManager.MarkSceneDirty(EditorSceneManager.GetActiveScene());
        }



        if (GUILayout.Button("SHOW Visuals --- DEATH Triggers"))
        {
            _deathTriggers = FindObjectsOfType<DeathTrigger>().ToList();
            for (int i = 0; i < _deathTriggers.Count; i++)
            {
                _deathTriggers[i].ToggleVisuals(true);
            }

            EditorSceneManager.MarkSceneDirty(EditorSceneManager.GetActiveScene());
        }
        if (GUILayout.Button("HIDE Visuals --- DEATH Triggers"))
        {
            _deathTriggers = FindObjectsOfType<DeathTrigger>().ToList();
            for (int i = 0; i < _deathTriggers.Count; i++)
            {
                _deathTriggers[i].ToggleVisuals(false);
            }

            EditorSceneManager.MarkSceneDirty(EditorSceneManager.GetActiveScene());
        }


        if (GUILayout.Button("SHOW Visuals --- Invisible Walls"))
        {
            _invisibleWalls = FindObjectsOfType<InvisibleWall>().ToList();
            for (int i = 0; i < _invisibleWalls.Count; i++)
            {
                _invisibleWalls[i].ToggleVisuals(true);
            }

            EditorSceneManager.MarkSceneDirty(EditorSceneManager.GetActiveScene());
        }
        if (GUILayout.Button("HIDE Visuals --- Invisible Walls"))
        {
            _invisibleWalls = FindObjectsOfType<InvisibleWall>().ToList();
            for (int i = 0; i < _invisibleWalls.Count; i++)
            {
                _invisibleWalls[i].ToggleVisuals(false);
            }

            EditorSceneManager.MarkSceneDirty(EditorSceneManager.GetActiveScene());
        }
    }
}

#endif
