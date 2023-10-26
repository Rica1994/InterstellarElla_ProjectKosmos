using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[DefaultExecutionOrder(-1000)]
public class Service : MonoBehaviour
{
    private bool _isApplicationQuitting = false;

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
        if (!_isApplicationQuitting)
        {
            ServiceLocator.Instance.UnRegister(this);
        }
    }

    private void OnApplicationQuit()
    {
        _isApplicationQuitting = true;
    }
}
