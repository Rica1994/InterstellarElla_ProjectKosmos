using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class PlayerController : MonoBehaviour
{
    public virtual void UpdateController()
    {
    }

    public virtual void Collide(Vector3 impulse)
    {
        
    }
}
