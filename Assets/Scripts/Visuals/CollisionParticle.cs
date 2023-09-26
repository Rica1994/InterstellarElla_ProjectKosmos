using UnityEngine;

public class CollisionParticle : MonoBehaviour
{
    [SerializeField]
    private ParticleSystem _impactParticle; // Assign your impact particle prefab in the Inspector
    //[SerializeField]
    //private ParticleSystem _groundTrailParticle;
    [SerializeField]
    private float _yThreshold = 1.0f;

    public bool activate = true;

    private void OnCollisionEnter(Collision collision)
    {
        if (activate)
        {
            foreach (ContactPoint contact in collision.contacts)
            {
                Vector3 collisionPoint = contact.point;

                // Spawn impact particles at the collision point
                if (collisionPoint.y > gameObject.transform.position.y - _yThreshold)
                {
                    SpawnImpactParticles(_impactParticle, collisionPoint);
                }
            }
        }
    }

    //private void OnCollisionStay(Collision collision)
    //{
    //    foreach (ContactPoint contact in collision.contacts)
    //    {
    //        Vector3 collisionPoint = contact.point;

    //        // Spawn impact particles at the collision point
    //        if (collisionPoint.y < gameObject.transform.position.y - _yThreshold)
    //        {
    //            SpawnImpactParticles(_groundTrailParticle, collisionPoint);
    //        }
    //    }
    //}

    private void SpawnImpactParticles(ParticleSystem particleSystem, Vector3 position)
    {
        Instantiate(particleSystem, position, Quaternion.identity);
    }
}
