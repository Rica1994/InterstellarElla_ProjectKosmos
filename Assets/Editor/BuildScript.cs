using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using System.IO;
using UnityEditor.Build.Reporting;
using System;

public class BuildWindow : EditorWindow
{
    private string repositoryPath = string.Empty;
    private bool _selectAllScenesBoolean = false;
    private bool _lastSelectAllScenesBoolean = false;
    private Vector2 scrollPosition;

    private List<bool> buildToggles = new List<bool>();
    private string[] levels = new string[] {"S_MainMenu",
            "Levels\\S_Quiz",
         "Levels\\Mars\\S_Level_1_1_Work",
         "Levels\\Mars\\S_Level_1_Intro",
         "Levels\\Mars\\S_Level_1_Outro",
         "Levels\\Venus\\S_Level_3_0_Work",
         "Levels\\Venus\\S_Level_3_1_Work",
         "Levels\\Venus\\S_Level_3_2_Work",
         "Levels\\Venus\\S_Level_3_3_Work",
         "Levels\\Venus\\S_Level_3_4_Work",
         "Levels\\Venus\\S_Level_3_5_Work",
         "Levels\\Venus\\S_Level_3_Intro",
         "Levels\\Venus\\S_Level_3_Outro",
         "Levels\\Saturn\\S_Level_4_0_Work",
         "Levels\\Saturn\\S_Level_4_1_Work",
         "Levels\\Saturn\\S_Level_4_2_Work",
         "Levels\\Saturn\\S_Level_4_3_Work",
         "Levels\\Saturn\\S_Level_4_4_Work",
         "Levels\\Saturn\\S_Level_4_5_Work",
         "Levels\\Saturn\\S_Level_4_Intro",
         "Levels\\Saturn\\S_Level_4_Outro",
         "Levels\\Pluto\\S_Level_2_0_Work",
         "Levels\\Pluto\\S_Level_2_1_Work",
         "Levels\\Pluto\\S_Level_2_2_Work",
         "Levels\\Pluto\\S_Level_2_Intro",
         "Levels\\Pluto\\S_Level_2_Outro",
        "Levels\\Mercury\\S_Level_5_0_Work",
        "Levels\\Mercury\\S_Level_5_1_Work",
        "Levels\\Mercury\\S_Level_5_2_Work",
        "Levels\\Mercury\\S_Level_5_3_Work",
        "Levels\\Mercury\\S_Level_5_Intro" ,
          "Levels\\Mercury\\S_Level_5_Outro"};

    [MenuItem("Build/Build Window")]
    public static void ShowWindow()
    {
        GetWindow<BuildWindow>("Build Window");
    }

    private void OnGUI()
    {
        GUILayout.Label("Build Settings", EditorStyles.boldLabel);
        repositoryPath = EditorGUILayout.TextField("Repository Path", repositoryPath);

        _selectAllScenesBoolean = GUILayout.Toggle(_selectAllScenesBoolean, "Select all scenes");
        HandleSelectAllScenes();

        // Use the full width of the editor window for the scroll view
        scrollPosition = GUILayout.BeginScrollView(scrollPosition, GUILayout.ExpandWidth(true), GUILayout.Height(400));

        string lastFolder = "";
        foreach (var level in levels)
        {
            int index = Array.IndexOf(levels, level);
            EnsureBuildToggleExists(index);

            // Extract folder name
            string folderName = ExtractFolderName(level);

            // Display folder label if it's different from the last one
            if (folderName != lastFolder)
            {
                if (!string.IsNullOrEmpty(lastFolder))
                {
                    GUILayout.Space(10); // Extra space between groups
                }
                GUILayout.Label(folderName, EditorStyles.boldLabel);
                lastFolder = folderName;
            }

            // Custom layout for each toggle
            GUILayout.BeginHorizontal();
            buildToggles[index] = GUILayout.Toggle(buildToggles[index], "", GUILayout.Width(20)); // Toggle with a fixed width
            GUILayout.Label(new GUIContent(level, level), GUILayout.ExpandWidth(true)); // Label with tooltip and expanding width
            GUILayout.EndHorizontal();
        }
        GUILayout.EndScrollView();

        if (GUILayout.Button("Build Selected Scenes"))
        {
            BuildSelectedScenes();
        }
    }

    private string ExtractFolderName(string level)
    {
        // Logic to extract the folder name from the level string
        // Example: "Levels\\Mars\\S_Level_1_1_Work" -> "Mars"
        var splitPath = level.Split('\\');
        if (splitPath.Length > 1 && splitPath[0] == "Levels")
        {
            return splitPath[1]; // Returns the subfolder name
        }
        return "MainMenu"; // Default for levels not in subfolders
    }

    private void EnsureBuildToggleExists(int index)
    {
        if (buildToggles.Count <= index)
        {
            buildToggles.Add(true);
        }
    }

    private void HandleSelectAllScenes()
    {
        if (_selectAllScenesBoolean && _lastSelectAllScenesBoolean != _selectAllScenesBoolean)
        {
            _lastSelectAllScenesBoolean = true;
            SetAllToggles(true);
        }
        else if (_selectAllScenesBoolean == false && _lastSelectAllScenesBoolean != _selectAllScenesBoolean)
        {
            _lastSelectAllScenesBoolean = false;
            SetAllToggles(false);
        }
    }

    private void SetAllToggles(bool value)
    {
        for (int i = 0; i < levels.Length; i++)
        {
            EnsureBuildToggleExists(i);
            buildToggles[i] = value;
        }
    }

    private void BuildSelectedScenes()
    {
        if (string.IsNullOrEmpty(repositoryPath))
        {
            Debug.LogError("Repository path not set.");
            return;
        }

        string projectPath = Application.dataPath.Replace("/Assets", "");
        string scenesDir = Path.Combine(Application.dataPath, "Scenes", "GameScenes");


        for (int i = 0; i < levels.Length; i++)
        {
            if (!buildToggles[i])
            {
                continue;  // Skip this scene if its toggle is not checked
            }

            var level = levels[i];

            //     if (EditorUtility.DisplayCancelableProgressBar(
            //         "Building Scenes",
            //         $"Building {levels[i]} ({i + 1}/{levels.Length})",
            //         (float)i / levels.Length))
            //     {
            //         Debug.Log("User pressed the cancel button");
            //         break;
            //     }

            string[] splitLevel = level.Split('\\');
            string sceneName = "";
            string scenePath = "";
            string buildPath = Path.Combine(repositoryPath, level);

            if (splitLevel.Length == 1)  // For "MainMenu"
            {
                sceneName = splitLevel[0];
                scenePath = Path.Combine(scenesDir, "MainMenu", sceneName + ".unity");
                buildPath = Path.Combine(repositoryPath, sceneName);
            }
            else if (splitLevel.Length == 2 && splitLevel[0] == "Levels" && splitLevel[1] == "S_Quiz")  // For "Levels\Quiz"
            {
                sceneName = splitLevel[1];
                scenePath = Path.Combine(scenesDir, "Quiz", sceneName + ".unity");
                buildPath = Path.Combine(repositoryPath, splitLevel[0], sceneName);
            }
            else  // For other levels
            {
                sceneName = splitLevel[splitLevel.Length - 1];
                scenePath = Path.Combine(scenesDir, splitLevel[splitLevel.Length - 2], sceneName + ".unity");
                buildPath = Path.Combine(repositoryPath, level);
            }


            if (!Directory.Exists(buildPath))
            {
                Directory.CreateDirectory(buildPath);
            }

            BuildPlayerOptions buildPlayerOptions = new BuildPlayerOptions();
            buildPlayerOptions.scenes = new[] { scenePath };
            buildPlayerOptions.locationPathName = buildPath;
            buildPlayerOptions.target = BuildTarget.WebGL;
            buildPlayerOptions.options = BuildOptions.None;

            BuildReport report = BuildPipeline.BuildPlayer(buildPlayerOptions);
            if (report.summary.result != BuildResult.Succeeded)
            {
                Debug.LogError("Build failed for " + sceneName + " at " + buildPath);
            }
        }
    }
}