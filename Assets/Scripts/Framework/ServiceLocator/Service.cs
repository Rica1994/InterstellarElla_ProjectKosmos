using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[DefaultExecutionOrder(-1000)]
public class Service : MonoBehaviour
{
    private bool _isApplicationQuitting = false;
    protected bool _isDestroyed = false;
    public bool IsDestroyed => _isDestroyed;

    virtual protected void OnEnable()
    {
       
    }

    virtual protected void Awake()
    {
        if (ServiceLocator.Instance.Contains(this))
        {
            _isDestroyed = true;
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
