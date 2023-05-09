using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Service : MonoBehaviour
{
    virtual protected void Awake()
    {
        if (ServiceLocator.Instance.Contains(this))
        {
            Destroy(this);
        }
        else
        {
            ServiceLocator.Instance.Register(this);
        }
    }
}
