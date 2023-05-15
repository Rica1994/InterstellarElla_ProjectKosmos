

#if UNITY_EDITOR

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class ParticleTool : EditorWindow
{
    //[MenuItem("Window/ParticleTool")]
    //public static void ShowWindow()
    //{
    //    GetWindow<ParticleTool>("ParticleTool");
    //}

    //private void OnGUI()
    //{
    //    FindParticlePrefabs();
    //}



    private void FindParticlePrefabs()
    {
        //if (GUILayout.Button("Find Particles in project, add to enum"))
        //{
        //    // Find all assets with the Class "A"
        //    string[] assetGuids = AssetDatabase.FindAssets("t:ClassA"); // Replace "ClassA" with the actual class name

        //    List<ParticleEntity> particleObjects = new List<ParticleEntity>();

        //    foreach (string guid in assetGuids)
        //    {
        //        string assetPath = AssetDatabase.GUIDToAssetPath(guid);
        //        ParticleEntity asset = AssetDatabase.LoadAssetAtPath<ParticleEntity>(assetPath);

        //        if (asset != null)
        //        {
        //            particleObjects.Add(asset);
        //        }
        //    }

        //    // Empty the existing enum class
        //    ParticleType enumClass = new ParticleType();
        //    enumClass.

        //    // Fill the enum class with values from found objects
        //    foreach (ParticleEntity obj in particleObjects)
        //    {
        //        enumClass.values.Add(obj.SomeProperty); // Modify this line according to your ClassA properties
        //    }

        //    // Get the GameObject with Class "B" script attached
        //    GameObject bObject = GameObject.Find("YourGameObjectName"); // Replace "YourGameObjectName" with the actual name of the GameObject
        //    ClassB bScript = bObject.GetComponent<ClassB>();

        //    // Add found objects to the public list in Class "B"
        //    bScript.objectsList.AddRange(particleObjects);


        //}
    }
}

#endif
