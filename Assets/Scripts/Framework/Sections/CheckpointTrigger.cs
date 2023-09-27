using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CheckpointTrigger : TriggerHandler
{
    public GameObject CheckpointPoint;

    [Header("Visuals")]
    public GameObject Visual;
    public GameObject VisualCheckpointPoint;


    public void ToggleVisuals(bool showThem)
    {
        Visual.SetActive(showThem);
        VisualCheckpointPoint.SetActive(showThem);
    }
}
