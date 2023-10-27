using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using System.IO;
using UnityEditor.Build.Reporting;

public class BuildWindow : EditorWindow
{
    private string repositoryPath = string.Empty;

    [MenuItem("Build/Build Window")]
    public static void ShowWindow()
    {
        GetWindow<BuildWindow>("Build Window");
    }

    private void OnGUI()
    {
        GUILayout.Label("Build Settings", EditorStyles.boldLabel);

        repositoryPath = EditorGUILayout.TextField("Repository Path", repositoryPath);

        if (GUILayout.Button("Build All Scenes"))
        {
            BuildAllScenes();
        }
    }

    private void BuildAllScenes()
    {
        if (string.IsNullOrEmpty(repositoryPath))
        {
            Debug.LogError("Repository path not set.");
            return;
        }

        string projectPath = Application.dataPath.Replace("/Assets", "");
        string scenesDir = Path.Combine(Application.dataPath, "Scenes", "GameScenes");

        string[] levels = new string[] {
           "S_MainMenu",
       //  "Levels\\Mars\\S_Level_1_1_Work",
       //  "Levels\\Mars\\S_Level_1_Intro",
       //  "Levels\\Mars\\S_Level_1_Outro",
       //  "Levels\\Venus\\S_Level_3_0_Work",
       //  "Levels\\Venus\\S_Level_3_1_Work",
       //  "Levels\\Venus\\S_Level_3_2_Work",
       //  "Levels\\Venus\\S_Level_3_3_Work",
       //  "Levels\\Venus\\S_Level_3_4_Work",
       //  "Levels\\Venus\\S_Level_3_5_Work",
       //  "Levels\\Venus\\S_Level_3_Intro",
       //  "Levels\\Venus\\S_Level_3_Outro",
       //  "Levels\\Saturn\\S_Level_4_0_Work",
       //  "Levels\\Saturn\\S_Level_4_1_Work",
       //  "Levels\\Saturn\\S_Level_4_2_Work",
       //  "Levels\\Saturn\\S_Level_4_3_Work",
       //  "Levels\\Saturn\\S_Level_4_4_Work",
       //  "Levels\\Saturn\\S_Level_4_5_Work",
       //  "Levels\\Saturn\\S_Level_4_Intro",
       //  "Levels\\Saturn\\S_Level_4_Outro",
         "Levels\\Pluto\\S_Level_2_0_Work",
         "Levels\\Pluto\\S_Level_2_1_Work",
         "Levels\\Pluto\\S_Level_2_2_Work",
         "Levels\\Pluto\\S_Level_2_Intro",
         "Levels\\Pluto\\S_Level_2_Outro",
     //   "Levels\\Mercury\\S_Level_5_0_Work",
     //   "Levels\\Mercury\\S_Level_5_1_Work",
     //   "Levels\\Mercury\\S_Level_5_2_Work",
     //   "Levels\\Mercury\\S_Level_5_3_Work",
     //   "Levels\\Mercury\\S_Level_5_Intro" ,
     //     "Levels\\Mercury\\S_Level_5_Outro",
            "Levels\\S_Quiz" };


        for (int i = 0; i < levels.Length; i++)
        {
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

        //  EditorUtility.ClearProgressBar();
    }
}