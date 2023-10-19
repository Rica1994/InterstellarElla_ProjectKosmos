using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(TriggerHandler))]
public class TriggerAction : MonoBehaviour
{
    [SerializeField]
    private ChainAction _action;

    //    [SerializeField]
    private bool _destroyOnActionComplete = false;

    [SerializeField]
    private bool _onlyTriggerOnce = false;

    private TriggerHandler _triggerHandler;

    // Start is called before the first frame update
    void Start()
    {
        _triggerHandler = GetComponent<TriggerHandler>();
        _triggerHandler.OnTriggered += OnTriggered;
        _action.ChainActionDone += OnChainActionDone;
    }

    private void OnChainActionDone()
    {
        if (_destroyOnActionComplete) Destroy(gameObject);
    }

    private void OnTriggered(TriggerHandler me, Collider other, bool hasEntered)
    {
        if (hasEntered && other.GetComponent<PlayerController>())
        {
            _action.Execute();

            if (_onlyTriggerOnce)
            {
                _triggerHandler.OnTriggered -= OnTriggered;
            }
        }
    }
    private void OnDestroy()
    {
        _action.ChainActionDone -= OnChainActionDone;
        _triggerHandler.OnTriggered -= OnTriggered;
    }

#if UNITY_EDITOR
    private void OnDrawGizmos()
    {
        var box = GetComponent<BoxCollider>();
        if (box == null) return;
        Gizmos.color = Color.blue;
        Gizmos.matrix = Matrix4x4.TRS(transform.position, transform.rotation, transform.localScale);
        Gizmos.DrawWireCube(box.center, box.size);
    }
#endif
}
