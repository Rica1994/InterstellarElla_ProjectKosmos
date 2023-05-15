using System.Collections;
using System.Collections.Generic;
using UnityEngine;


/// <summary>
/// Assign this to the core prefab, from which we create more prefab variants
/// </summary>
public class ParticleEntity : MonoBehaviour
{
    public float ParticleLifeTime = 1;

    public ParticleSystem ParticleSystemParent;
}
