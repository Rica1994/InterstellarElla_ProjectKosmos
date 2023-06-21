using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LeverTrigger : MonoBehaviour
{
    [SerializeField]
    private Lever _lever;

    [SerializeField]
    private Collider _trigger;

    private void OnTriggerEnter(Collider other)
    {
        // use layers here for collision
        if (other.TryGetComponent(out EllaExploring exploringScript))
        {
            _lever.ActivateCameraCutscene(exploringScript);

            _trigger.enabled = false;
        }
    }
}
