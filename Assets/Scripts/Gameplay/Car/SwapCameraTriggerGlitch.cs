using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SwapCameraTriggerGlitch : MonoBehaviour
{
    [SerializeField]
    private SwapCameraGlitch _swapCameraGlitch;

    private Coroutine _coroutineCooldown;

    private float _cooldownTime;
    private bool _triggered;


    private void OnTriggerEnter(Collider other)
    {
        if (other.TryGetComponent(out SimpleCarController glitch) && _triggered == false)
        {
            _swapCameraGlitch.SwapToNewVirtualCamera(glitch);

            _coroutineCooldown = StartCoroutine(CooldDown());

            _triggered = true;
        }
    }

    private IEnumerator CooldDown()
    {
        yield return new WaitForSeconds(_cooldownTime);

        _triggered = false;
    }
}
