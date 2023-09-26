using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DynamoDiggingTrigger : MonoBehaviour
{
    public bool StartDigging = true;

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
        }
    }
}
