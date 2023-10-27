using System.Collections;
using UnityEngine;


public class RockWall : MonoBehaviour
{
    [SerializeField] private float impulseForce = 10.0f;
    [SerializeField] private Vector3 impulseDirection = new Vector3(0.0f, 1.0f, 1.0f);
    [SerializeField] private float maxRandomAngle = 30.0f;
    private bool collided = false;

    private void OnTriggerEnter(Collider other)
    {
        if (other.tag != "Dynamo" || collided) return;
        
        StartCoroutine(ApplyCollisionEffects());
    }

    private IEnumerator ApplyCollisionEffects()
    {
        var rbs = GetComponentsInChildren<Rigidbody>();

        foreach (var rb in rbs)
        {
            rb.isKinematic = true;
        }

       // yield return new WaitForSeconds(collisionDelay);

        foreach (var rb in rbs)
        {
            rb.isKinematic = false;
            rb.mass = 0.01f;
            
            Vector3 randomDirection = Quaternion.Euler(Random.Range(-maxRandomAngle, maxRandomAngle), Random.Range(-maxRandomAngle, maxRandomAngle), 0) * impulseDirection.normalized;
            
            rb.AddForce(randomDirection.normalized * impulseForce, ForceMode.Impulse);
        }

        collided = true;
        
        yield break;
    }
}
