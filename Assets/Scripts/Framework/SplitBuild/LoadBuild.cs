using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LoadBuild : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        // Add the data you want to transfer as URL parameters
        string url = $"./01";
        
        Application.ExternalEval($"window.location.href = '{url}';");
    }
}
