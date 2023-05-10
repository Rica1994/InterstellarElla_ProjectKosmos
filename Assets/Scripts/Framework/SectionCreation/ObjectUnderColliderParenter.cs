using UnityEngine;
using UnityEditor;
using UnityEditor.SceneManagement;
using System.Collections.Generic;
using UnityEngine.SceneManagement;

public class ObjectUnderColliderParenter : EditorWindow
{
    public LevelSectionCreator SelectedLevelSectionCreator;

    [MenuItem("Window/Parent Game Objects")]
    public static void ShowWindow()
    {
        GetWindow<ObjectUnderColliderParenter>("ObjectParenter");
    }

    private void OnGUI()
    {
        ParentGameObjects();
    }

    
    public void ParentGameObjects()
    {
        // Get selected object in hierarchy
        GameObject selectedObject = Selection.activeGameObject;
        if (selectedObject != null)
        {
            SelectedLevelSectionCreator = selectedObject.GetComponent<LevelSectionCreator>();
        }
        else
        {
            Debug.Log("You did not select a LevelSectionCreator. Doing nothing");
            return;
        }


        Undo.RecordObject(selectedObject, "Undo-ing parenting");


        // all of this happens when clicking the button ...
        if (GUILayout.Button("Parent Objects In Selected Collider"))
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
            SelectedLevelSectionCreator.Section.ParentEnvironment.name = "Pickups_" + SelectedLevelSectionCreator.SectionIndex;
            SelectedLevelSectionCreator.Section.ParentPickups.name = "Environment_" + SelectedLevelSectionCreator.SectionIndex;
            
            // inform the editor that the scene has changed
            EditorSceneManager.MarkSceneDirty(EditorSceneManager.GetActiveScene());
        }
    }
}
