using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SwapCameraTrigger : MonoBehaviour
{
    [SerializeField]
    private SwapCameraSpeederGround _swapCamera;

    private Coroutine _coroutineCooldown;

    private float _cooldownTime;
    private bool _triggered;


    private void OnTriggerEnter(Collider other)
    {
        if (other.TryGetComponent(out SpeederGround speeder) && _triggered == false)
        {
            _swapCamera.SwapToNewVirtualCamera(speeder);

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
