using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DynamoDiggingTrigger : MonoBehaviour
{
    public bool StartDigging = true;
    [SerializeField]
    private bool _destroyAfter = true;

    private void OnTriggerEnter(Collider other)
    {
        if (other.TryGetComponent(out DynamoDistance dynamoDist))
        {
            if (StartDigging == true)
            {
                dynamoDist.DynamoGoesDigging();
            }
            else
            {
                dynamoDist.DynamoStopsDigging();
            }

            if (_destroyAfter == true)
            {
                Destroy(gameObject);
            }
        }
    }
}
