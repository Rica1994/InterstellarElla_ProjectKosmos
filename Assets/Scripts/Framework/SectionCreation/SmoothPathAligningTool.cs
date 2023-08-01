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

public class SmoothPathAligningTool : EditorWindow
{
    private CinemachineSmoothPath _cinePath;
    private SmoothPathBox _smoothPathBox;

    private List<SmoothPathToBeAlligned> _objectsToBeAlligned = new List<SmoothPathToBeAlligned>();

    private List<PathPickups> _pathsPickups = new List<PathPickups>();


    [MenuItem("Window/Smooth Path Aligning Tool")]
    public static void ShowWindow()
    {
        GetWindow<SmoothPathAligningTool>("Smooth Path Aligning Tool");
    }

    private void OnGUI()
    {
        ShowButtons();
    }

    public void ShowButtons()
    {
        // button parenting objects
        AllignObjectsWithinSmoothBoxToSelectedPath();
    }

    private void AllignObjectsWithinSmoothBoxToSelectedPath()
    {
        if (GUILayout.Button("Select Path"))
        {
            // find the path to which we want to lock our obstacles/pickups to
            GameObject selectedObject = Selection.activeGameObject;
            _cinePath = selectedObject.GetComponent<CinemachineSmoothPath>();

            if (_cinePath != null)
            {
                
            }
            else
            {
                Debug.LogWarning("no Cinemachine Path found !");
            }
        }

        if (GUILayout.Button("Select The SmoothBox"))
        {
            // find the path to which we want to lock our obstacles/pickups to
            GameObject selectedObject = Selection.activeGameObject;
            _smoothPathBox = selectedObject.GetComponent<SmoothPathBox>();

            if (_smoothPathBox != null)
            {

            }
            else
            {
                Debug.LogWarning("no SmoothBox found !");
            }


            // clear the list
            _smoothPathBox.EmptyTheList();

            // check for all objects(with smoothPathToBeAlligned) in the scene if they fit into this box's bounds
            // if so -> add to smoothbox list
            _objectsToBeAlligned = FindObjectsOfType<SmoothPathToBeAlligned>().ToList();
            Bounds BoxBounds = _smoothPathBox.GetComponent<Collider>().bounds;

            for (int i =0; i < _objectsToBeAlligned.Count; i++)
            {
                var objectOfInterest = _objectsToBeAlligned[i];

                if (BoxBounds.Contains(objectOfInterest.transform.position))
                {
                    _smoothPathBox.ObjectsToBeAlligned.Add(objectOfInterest);
                }
            }
        }



        if (GUILayout.Button("Allign objects in Smoothbox to Path"))
        {
            if (_cinePath != null && _smoothPathBox != null)
            {
                for (int i = 0; i < _smoothPathBox.ObjectsToBeAlligned.Count; i++)
                {
                    float closestPathFloat = _cinePath.FindClosestPoint(_smoothPathBox.ObjectsToBeAlligned[i].transform.position, 0 ,-1, 20);

                    Vector3 closestPathPosition = _cinePath.EvaluatePosition(closestPathFloat);
                    Quaternion closestPathQuaternion = _cinePath.EvaluateOrientation(closestPathFloat);

                    _smoothPathBox.ObjectsToBeAlligned[i].transform.position = closestPathPosition;
                    _smoothPathBox.ObjectsToBeAlligned[i].transform.rotation = closestPathQuaternion;

                    EditorUtility.SetDirty(_smoothPathBox.ObjectsToBeAlligned[i]);
                }

                EditorSceneManager.MarkSceneDirty(EditorSceneManager.GetActiveScene());
            }
            else
            {
                Debug.LogWarning("no path or no box assigned !");
            }
        }



        if (GUILayout.Button("SHOW Visuals for Player bounding boxes"))
        {
            _objectsToBeAlligned = FindObjectsOfType<SmoothPathToBeAlligned>().ToList();

            for (int i = 0; i < _objectsToBeAlligned.Count; i++)
            {
                _objectsToBeAlligned[i].ShowVisuals(true);
            }

            EditorSceneManager.MarkSceneDirty(EditorSceneManager.GetActiveScene());
        }

        if (GUILayout.Button("HIDE Visuals for Player bounding boxes"))
        {
            _objectsToBeAlligned = FindObjectsOfType<SmoothPathToBeAlligned>().ToList();

            for (int i = 0; i < _objectsToBeAlligned.Count; i++)
            {
                _objectsToBeAlligned[i].ShowVisuals(false);
            }

            EditorSceneManager.MarkSceneDirty(EditorSceneManager.GetActiveScene());
        }


        if (GUILayout.Button("Create pickups on the pickup-paths"))
        {
            // find objects with a specific script 'PathPickup' (has a list of pickups 'A', has a list of waypoints 'B')
            _pathsPickups = FindObjectsOfType<PathPickups>().ToList();

            // for each path... 
            for (int i = 0; i < _pathsPickups.Count; i++)
            {
                // access the list 'A', destroy each object in the list -> clear the list.
                var pathOfInterest = _pathsPickups[i];

                // destroy all previous pickups 
                pathOfInterest.DestroyMyChildrenPickups();

                // store the current rotation of the path + parent obj, set path.rot + obj.rot to 0,0,0
                pathOfInterest.StoreRotations();
                pathOfInterest.ResetRotations();

                // clear the list 'B', add each waypoint to it.
                pathOfInterest.GetWaypoints();

                // for each obj in 'B' -> add+instantiate a pickup to 'A'
                pathOfInterest.CreatePickups();

                // reset the rotation to the stored value
                pathOfInterest.RevertRotations();
            }

            EditorSceneManager.MarkSceneDirty(EditorSceneManager.GetActiveScene());
        }

        if (GUILayout.Button("Delete pickups on the pickup-paths"))
        {
            // find objects with a specific script 'PathPickup' (has a list of pickups 'A', has a list of waypoints 'B')
            _pathsPickups = FindObjectsOfType<PathPickups>().ToList();

            // for each path... 
            for (int i = 0; i < _pathsPickups.Count; i++)
            {
                // clear the list, destroy all children of smoothpath.
                var pathOfInterest = _pathsPickups[i];

                pathOfInterest.DestroyMyChildrenPickups();
            }

            EditorSceneManager.MarkSceneDirty(EditorSceneManager.GetActiveScene());
        }
    }
}

#endif
