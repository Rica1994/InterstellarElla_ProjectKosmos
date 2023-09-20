
#if UNITY_EDITOR

using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;

public class SwapCameraVisualizingTool : EditorWindow
{
    private List<SwapCameraBase> _swapCameras = new List<SwapCameraBase>();

    [MenuItem("Window/Swap camera visualizer")]
    public static void ShowWindow()
    {
        GetWindow<SwapCameraVisualizingTool>("Swap camera visualizer");
    }

    private void OnGUI()
    {
        ShowButtons();
    }

    public void ShowButtons()
    {
        if (GUILayout.Button("SHOW Visuals for Swap Cameras"))
        {
            _swapCameras = FindObjectsOfType<SwapCameraBase>().ToList();

            for (int i = 0; i < _swapCameras.Count; i++)
            {
                _swapCameras[i].ToggleSwapCameraVisuals(true);
            }

            EditorSceneManager.MarkSceneDirty(EditorSceneManager.GetActiveScene());
        }


        if (GUILayout.Button("HIDE Visuals for Swap Cameras"))
        {
            _swapCameras = FindObjectsOfType<SwapCameraBase>().ToList();

            for (int i = 0; i < _swapCameras.Count; i++)
            {
                _swapCameras[i].ToggleSwapCameraVisuals(false);
            }

            EditorSceneManager.MarkSceneDirty(EditorSceneManager.GetActiveScene());
        }
    }
}

#endif
