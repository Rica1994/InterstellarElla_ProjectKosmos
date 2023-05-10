using UnityEngine;
using UnityEditor;
using UnityEditor.SceneManagement;
using System.Collections.Generic;
using UnityEngine.SceneManagement;

public class SectionCreationTool : EditorWindow
{
    public LevelSectionCreator SelectedLevelSectionCreator;
    public Section SelectedSection;

    [MenuItem("Window/Section Creation")]
    public static void ShowWindow()
    {
        GetWindow<SectionCreationTool>("Section Tools");
    }

    private void OnGUI()
    {
        PressedButton();
    }


    public void PressedButton()
    {
        // Get selected object in hierarchy
        SelectedLevelSectionCreator = null;
        SelectedSection = null;

        GameObject selectedObject = Selection.activeGameObject;
        if (selectedObject != null)
        {
            SelectedLevelSectionCreator = selectedObject.GetComponent<LevelSectionCreator>();

            if (SelectedLevelSectionCreator != null)
            {
                SelectedSection = SelectedLevelSectionCreator.Section;
            }
            else 
            { 
                SelectedSection = selectedObject.GetComponent<Section>();
            }
        }
        //else
        //{
        //    Debug.Log("You have not selected an object -> Doing nothing");
        //    return;
        //}

        //Undo.RecordObject(selectedObject, "Undo-ing parenting");

        // button parenting objects
        ParentObjectsWithinColliders();

        // buttons analyzing data
        AnalyzeDataInSection();
    }


    private void ParentObjectsWithinColliders()
    {
        if (GUILayout.Button("Parent Objects In Selected Colliders"))
        {
            if (SelectedLevelSectionCreator != null)
            {
                // parenting logic //
                List<GameObject> tempList = new List<GameObject>();
                foreach (GameObject obj in UnityEngine.Object.FindObjectsOfType<GameObject>())
                {
                    // first fill list with objects that do not have a parent...
                    if (obj.transform.parent == null)
                    {
                        tempList.Add(obj);
                    }
                }
                foreach (BoxCollider coll in SelectedLevelSectionCreator.CollidersDefiningMySection)
                {
                    List<GameObject> objectsAdded = new List<GameObject>();
                    foreach (GameObject obj in tempList)
                    {
                        if (coll.bounds.Contains(obj.transform.position))
                        {
                            objectsAdded.Add(obj);
                            if (obj.TryGetComponent(out PickUp pickUp))
                            {
                                obj.transform.SetParent(SelectedLevelSectionCreator.Section.ParentPickups.transform);
                            }
                            else
                            {
                                obj.transform.SetParent(SelectedLevelSectionCreator.Section.ParentEnvironment.transform);
                            }
                        }
                    }

                    foreach (GameObject obj in objectsAdded)
                    {
                        if (tempList.Contains(obj))
                        {
                            tempList.Remove(obj);
                        }
                    }
                    objectsAdded.Clear();
                }

                // naming logic //
                SelectedLevelSectionCreator.Section.name = "P_LevelSection_" + SceneManager.GetActiveScene().name + "_" + SelectedLevelSectionCreator.SectionIndex;
                SelectedLevelSectionCreator.Section.ParentEnvironment.name = "Environment_" + SelectedLevelSectionCreator.SectionIndex;
                SelectedLevelSectionCreator.Section.ParentPickups.name = "Pickups_" + SelectedLevelSectionCreator.SectionIndex;

                // inform the editor that the scene has changed
                EditorSceneManager.MarkSceneDirty(EditorSceneManager.GetActiveScene());
            }
            else
            {
                Debug.LogWarning("You have not selected a LevelSectionCreator -> Doing nothing");
            }
        }
    }

    private void AnalyzeDataInSection()
    {
        // log ALL sections ALL data
        if (GUILayout.Button("log data of EVERYTHING in this level"))
        {
            // access the levelManager...
            LevelManager levelManager = FindObjectOfType<LevelManager>();
            // iterate below logic for each section it has
            if (levelManager != null)
            {
                if (levelManager.Sections.Count > 0)
                {
                    int countObjects = 0;
                    int countTriangles = 0;
                    int countVertices = 0;
                    int countPickups = 0;

                    foreach (Section section in levelManager.Sections)
                    {
                        // add up the objects, tris, and pickups

                        // objects
                        countObjects += GetChildren(section.ParentEnvironment);
                        countObjects += GetChildren(section.ParentPickups);

                        // tris
                        countTriangles += GetTriangleCount(section.ParentEnvironment);
                        countTriangles += GetTriangleCount(section.ParentPickups);

                        // vertices
                        countVertices += GetVertexCount(section.ParentEnvironment);
                        countVertices += GetVertexCount(section.ParentPickups);

                        // pickUps
                        countPickups += GetPickupCount(section.ParentPickups);
                    }

                    Debug.Log("--This Level has " + countObjects + " GameObjects !");
                    Debug.Log("--This Level has " + countTriangles + " Triangles !");
                    Debug.Log("--This Level has " + countVertices + " Vertices !");
                    Debug.Log("--This Level has " + countPickups + " PickUps !");
                }
                else
                {
                    Debug.LogWarning("LevelManager has no assigned Sections -> nothing to calculate");
                }
            }
            else
            {
                Debug.LogWarning("Could not find a LevelManager in the scene -> Doing nothing");
            }
        }

        // log section GameObjects
        if (GUILayout.Button("log Selected Section GamObjects"))
        {
            if (SelectedSection != null)
            {
                // debug object counts
                Debug.Log("-This Section has " + GetChildren(SelectedSection.ParentEnvironment) + " GameObjects in Environment");
                Debug.Log("-This Section has " + GetChildren(SelectedSection.ParentPickups) + " GameObjects in Pickups");
            }
            else
            {
                Debug.LogWarning("You have not selected a LevelSectionCreator OR LevelSection -> Doing nothing");
            }
        }

        // log section Triangles
        if (GUILayout.Button("log Selected Section Triangles"))
        {
            if (SelectedSection != null)
            {
                // debug triangle count
                Debug.Log("This Section has " + GetTriangleCount(SelectedSection.ParentEnvironment) + " Triangles in Environment");
                Debug.Log("This Section has " + GetTriangleCount(SelectedSection.ParentPickups) + " Triangles in Pickups");
            }
            else
            {
                Debug.LogWarning("You have not selected a LevelSectionCreator OR LevelSection -> Doing nothing");
            }
        }

        // log section Vertices
        if (GUILayout.Button("log Selected Section Vertices"))
        {
            if (SelectedSection != null)
            {
                // debug triangle count
                Debug.Log("This Section has " + GetVertexCount(SelectedSection.ParentEnvironment) + " Vertices in Environment");
                Debug.Log("This Section has " + GetVertexCount(SelectedSection.ParentPickups) + " Vertices in Pickups");
            }
            else
            {
                Debug.LogWarning("You have not selected a LevelSectionCreator OR LevelSection -> Doing nothing");
            }
        }

        // log section Pickups
        if (GUILayout.Button("log Selected Section Pickups"))
        {
            if (SelectedSection != null)
            {
                // debug pickup count
                Debug.Log("-This Section has " + GetPickupCount(SelectedSection.ParentPickups) + " Pickups");
            }
            else
            {
                Debug.LogWarning("You have not selected a LevelSectionCreator OR LevelSection -> Doing nothing");
            }
        }
    }





    private int GetChildren(GameObject obj)
    {
        int count = 0;

        for (int i = 0; i < obj.transform.childCount; i++)
        {
            count++;
            Counter(obj.transform.GetChild(i).gameObject, ref count);
        }
        return count;
    }
    private void Counter(GameObject currentObj, ref int count)
    {
        for (int i = 0; i < currentObj.transform.childCount; i++)
        {
            count++;
            Counter(currentObj.transform.GetChild(i).gameObject, ref count);
        }
    }
    private int GetVertexCount(GameObject obj)
    {
        int count = 0;
        MeshFilter[] meshFilters;

        meshFilters = obj.GetComponentsInChildren<MeshFilter>(true);
        for (int i = 0; i < meshFilters.Length; i++)
        {
            count += meshFilters[i].sharedMesh.vertexCount;
            //Debug.Log("this is vertex count of " + meshFilters[i].gameObject.name + " == " + meshFilters[i].sharedMesh.vertexCount);
        }

        return count;
    }
    private int GetTriangleCount(GameObject obj)
    {
        int count = 0;
        MeshFilter[] meshFilters;

        meshFilters = obj.GetComponentsInChildren<MeshFilter>(true);
        for (int i = 0; i < meshFilters.Length; i++)
        {
            int triangleCalc = meshFilters[i].sharedMesh.triangles.Length / 3;
            count += triangleCalc;
            //Debug.Log("this is the triangle count of " + meshFilters[i].gameObject.name + " == " + triangleCalc);
        }

        return count;
    }

    private int GetPickupCount(GameObject obj)
    {
        int count = 0;
        PickUp[] pickups;

        pickups = obj.GetComponentsInChildren<PickUp>(true);
        count += pickups.Length;

        return count;
    }
}
