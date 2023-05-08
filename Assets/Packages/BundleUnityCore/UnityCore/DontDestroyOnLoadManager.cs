using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class DontDestroyOnLoadManager
{
    public static List<GameObject> DontDestroyOnLoadObjects = new List<GameObject>();

    public static void DontDestroyOnLoad(GameObject go)
    {
        UnityEngine.Object.DontDestroyOnLoad(go);
        DontDestroyOnLoadObjects.Add(go);
    }

    
    public static void DestroyAllDontDestroyOnLoadObjects()
    {
        foreach (var go in DontDestroyOnLoadObjects)
        {
            UnityEngine.Object.Destroy(go);
        }
    }
}
