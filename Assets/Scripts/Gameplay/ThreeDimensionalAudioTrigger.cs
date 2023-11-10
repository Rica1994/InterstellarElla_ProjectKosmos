using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(SphereCollider), typeof(TriggerHandler))]
public class ThreeDimensionalAudioTrigger : MonoBehaviour
{
    [SerializeField]
    private AudioClip _clip;

    [SerializeField]
    private AudioSourceSetting _setting;

    private TriggerHandler _triggerHandler;
    private AudioSource _myAudioSource;

    private void Start()
    {
        _triggerHandler = GetComponent<TriggerHandler>();
        _triggerHandler.OnTriggered += OnTriggered;
    }
    private void OnTriggered(TriggerHandler me, Collider other, bool hasEntered)
    {
        if (hasEntered)
        {
            _myAudioSource = ServiceLocator.Instance.GetService<SoundManager>().PlaySFXAt3DLocation(_clip, transform.position, _setting);
        }
        else
        {
            if (_myAudioSource != null) _myAudioSource.Stop();
        }
    }

    private void OnDestroy()
    {
        _triggerHandler.OnTriggered -= OnTriggered;
    }

#if UNITY_EDITOR
    void OnDrawGizmos()
    {
        if (_setting.IsThreeDimensional)
        {

            Gizmos.color = Color.green;
            Gizmos.DrawWireSphere(transform.position, _setting.MinMaxDistance.x);
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(transform.position, _setting.MinMaxDistance.y);
        }
    }
#endif
}
