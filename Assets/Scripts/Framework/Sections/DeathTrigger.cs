using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DeathTrigger : TriggerHandler
{
    [SerializeField]
    private GameObject _visual;


    public void ToggleVisuals(bool showThem = true)
    {
        _visual.SetActive(showThem);
    }
}
