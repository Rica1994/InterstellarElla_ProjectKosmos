using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SequenceTrigger : MonoBehaviour
{
    [SerializeField] 
    private TriggerHandler _trigger;
    [SerializeField]
    private Sequence _sequence;
    [SerializeField]
    private bool _disableTriggerAfterwards = true;

    [SerializeField]
    private bool _forceSequenceStart = false;

    private void Awake()
    {
        _trigger.OnTriggered += _trigger_OnTriggered;
    }

    private void _trigger_OnTriggered(TriggerHandler me, Collider other, bool hasEntered)
    {
        if (hasEntered && other.tag == "Player")
        {
            ChainManager.Instance.StartChain(_sequence, _forceSequenceStart);
            if (_disableTriggerAfterwards)
            {
                _trigger.gameObject.SetActive(false);
            }
        }
    }

    public void ToggleVisuals(bool showThem = true)
    {
        _trigger.GetComponent<MeshRenderer>().enabled = showThem;
    }
}
