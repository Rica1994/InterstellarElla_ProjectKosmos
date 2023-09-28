using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MaggieTriggerChainAction : ChainAction
{
    [SerializeField]
    private GameObject _visual;

    void Start()
    {
        _useUserBasedAction = true;

        if (_visual == null)
        {
            _visual = this.transform.GetChild(0).gameObject;
        }
    }

    private void OnValidate()
    {
        _useUserBasedAction = true;
    }


    private void OnTriggerEnter(Collider other)
    {
        if (other.TryGetComponent(out PlayerController player) == true)
        {
            Debug.Log("Completing action");
            _userBasedActionCompleted = true;
        }
    }


    public void ToggleVisuals(bool showThem)
    {
        _visual.SetActive(showThem);
    }
}
