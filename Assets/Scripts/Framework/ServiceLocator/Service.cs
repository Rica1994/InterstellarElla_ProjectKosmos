using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Service : MonoBehaviour
{
    private bool _isApplicationQuitting = false;
    protected bool IsApplicationQuitting => _isApplicationQuitting;

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
        if (_isApplicationQuitting)
        {
            return;
        }
        ServiceLocator.Instance.UnRegister(this);
    }

    private void OnApplicationQuit()
    {
        _isApplicationQuitting = true;
    }
}
