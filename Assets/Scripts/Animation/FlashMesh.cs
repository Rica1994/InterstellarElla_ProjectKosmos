using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;
using Random = UnityEngine.Random;

public class FlashMesh : MonoBehaviour
{
    [SerializeField]
    private float _minimumTimeUntillNextFlash = 4.0f;

    [SerializeField]
    private float _maximumTimeUntilNextFlash = 10.0f;

    [FormerlySerializedAs("_flashTime")] [SerializeField]
    private float _flashLength = 1f;

    [SerializeField]
    private int _flashSpeed = 3;
    
    private void OnValidate()
    {
        if (_minimumTimeUntillNextFlash >= _maximumTimeUntilNextFlash)
            _maximumTimeUntilNextFlash = _minimumTimeUntillNextFlash;
    }

    private IEnumerator StartFlash(float flashTime)
    {
        yield return new WaitForSeconds(flashTime);

        StartCoroutine(Helpers.Flicker(gameObject, _flashLength, _flashSpeed));

        yield return new WaitForSeconds(_flashLength);
        
        flashTime = Random.Range(_minimumTimeUntillNextFlash, _maximumTimeUntilNextFlash);

        StartCoroutine(StartFlash(flashTime));
    }
    
    // Start is called before the first frame update
    void Start()
    {
        float flashTime = Random.Range(_minimumTimeUntillNextFlash, _maximumTimeUntilNextFlash);
        StartCoroutine(StartFlash(flashTime));
    }
}
