using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SequenceTrigger : MonoBehaviour
{
    [SerializeField] 
    private TriggerHandler _trigger;
    [SerializeField]
    private Sequence _sequence;

    private void Awake()
    {
        _trigger.OnTriggered += _trigger_OnTriggered;
    }

    private void _trigger_OnTriggered(TriggerHandler me, Collider other, bool hasEntered)
    {
        if (hasEntered && other.tag == "Player")
        {
            ChainManager.Instance.StartChain(_sequence);
        }
    }
}
