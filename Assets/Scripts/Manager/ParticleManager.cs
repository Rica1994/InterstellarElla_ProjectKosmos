using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum ParticleType
{
    PS_BoostSpeederGround = 0,
    PS_BoostSpeederSpace = 1,
    PS_BoostBoots = 2,
    PS_BoostCar = 3,

    PS_Collision = 4,
    PS_PickupTrigger = 5,
    PS_JumpPadIdle = 6,
    PS_Footstep = 7
}


public class ParticleManager : Service
{
    [SerializeField]
    private ParticleReferences _particleRefs;

    private List<ParticleEntity> _createdParticles = new List<ParticleEntity>();


    public void CreateParticleWorldSpace(ParticleType particleType, Vector3 particleSpawnPosition, Vector3 particleRotation = new Vector3(), bool destroyAllParticlesInstantly = false)
    {
        var allParticles = _particleRefs.ParticlePrefabs;
        var particleIndex = ((int)particleType);

        // make sure we're not out of bounds && whether the object is null
        if (particleIndex <= allParticles.Count-1 && allParticles[particleIndex] != null)
        {

            ParticleEntity particleCreated = Instantiate(allParticles[particleIndex], particleSpawnPosition, Quaternion.Euler(particleRotation));
            _createdParticles.Add(particleCreated);

            StartCoroutine(DestroyParticle(particleCreated, destroyAllParticlesInstantly));
        }
        else
        {
            Debug.LogWarning("Index out of bounds or index has no reference. Double check my Prefab List.");
        }
    }
  
    public void CreateParticleLocalSpace(ParticleType particleType, Transform particleParentTransform, bool destroyAllParticlesInstantly = false)
    {
        var allParticles = _particleRefs.ParticlePrefabs;
        var particleIndex = ((int)particleType);

        if (particleIndex <= allParticles.Count && allParticles[particleIndex] != null)
        {
            ParticleEntity particleCreated = Instantiate(allParticles[particleIndex], particleParentTransform);
            _createdParticles.Add(particleCreated);

            StartCoroutine(DestroyParticle(particleCreated, destroyAllParticlesInstantly));
        }
        else
        {
            Debug.LogWarning("Index out of bounds or index has no reference. Double check my Prefab List.");
        }
    }
    public ParticleSystem CreateParticleLocalSpacePermanent(ParticleType particleType, Transform particleParentTransform)
    {
        var allParticles = _particleRefs.ParticlePrefabs;
        var particleIndex = ((int)particleType);

        if (particleIndex <= allParticles.Count && allParticles[particleIndex] != null)
        {
            ParticleEntity particleCreated = Instantiate(allParticles[particleIndex], particleParentTransform);
            ParticleSystem particleSystem = particleCreated.GetComponent<ParticleSystem>();

            return particleSystem;
        }
        else
        {
            Debug.LogWarning("Index out of bounds or index has no reference. Double check my Prefab List.");
            return null;
        }
    }




    private IEnumerator DestroyParticle(ParticleEntity particleToDestroy , bool destroyPaticlesInstantly = false)
    {
        yield return new WaitForSeconds(particleToDestroy.ParticleLifeTime);

        // disable the particle system first
        if (destroyPaticlesInstantly == false)
        {
            particleToDestroy.ParticleSystemParent.Stop(true);

            yield return new WaitForSeconds(2f);
        }

        // after possible additional wait time, fully destroy the particle
        if (particleToDestroy.gameObject != null)
        {
            _createdParticles.Remove(particleToDestroy);
            Destroy(particleToDestroy.gameObject);
        }       
    }
}
