using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(TriggerHandler))]
public class TriggerAction : MonoBehaviour
{
    [SerializeField]
    private ChainAction _action;

    // Start is called before the first frame update
    void Start()
    {
        var trigger = GetComponent<TriggerHandler>();
        trigger.OnTriggered += OnTriggered;
    }

    private void OnTriggered(TriggerHandler me, Collider other, bool hasEntered)
    {
        if (hasEntered && other.GetComponent<PlayerController>())
        {
            _action.Execute();
        }
    }
}
