using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SmoothPathBox : MonoBehaviour
{
    [HideInInspector]
    public List<SmoothPathToBeAlligned> ObjectsToBeAlligned = new List<SmoothPathToBeAlligned>();

    public void EmptyTheList()
    {
        if (ObjectsToBeAlligned.Count > 0)
        {
            ObjectsToBeAlligned.Clear();
        }     
    }
}
