using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Service : MonoBehaviour
{
    virtual protected void OnEnable()
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

    virtual protected void OnDisable()
    {
        ServiceLocator.Instance.UnRegister(this);
    }
}
