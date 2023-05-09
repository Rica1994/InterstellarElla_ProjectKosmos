using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GetInput : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        var input = ServiceLocator.Instance.InputManager;
        Debug.Log(input.enabled);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
