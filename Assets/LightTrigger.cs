using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LightTrigger : MonoBehaviour
{
    [SerializeField]
    private TriggerHandler _trigger;
    [SerializeField]
    private bool _disableTriggerAfterwards = true;
    [SerializeField]
    private Light _light;
    [SerializeField]
    private float _intensity;
    [SerializeField]
    private float _duration;

    private float _initialIntensity;

    private void Awake()
    {
        _trigger.OnTriggered += _trigger_OnTriggered;
    }

    private void _trigger_OnTriggered(TriggerHandler me, Collider other, bool hasEntered)
    {
        if (hasEntered && other.tag == "Player")
        {
            if (_duration <= 0)
            {
                _light.intensity = _intensity;
            }
            else
            {
                _initialIntensity = _light.intensity;
                StartCoroutine(ChangeLightOverTime());
            }

            if (_disableTriggerAfterwards)
            {
                _trigger.gameObject.SetActive(false);
            }
        }
    }

    private IEnumerator ChangeLightOverTime()
    {
        float elapsed = 0f;
        while (elapsed < _duration)
        {
            _light.intensity = Mathf.Lerp(_initialIntensity, _intensity, elapsed / _duration);
            elapsed += Time.deltaTime;
            yield return null;
        }
        _light.intensity = _intensity;
    }
}
    