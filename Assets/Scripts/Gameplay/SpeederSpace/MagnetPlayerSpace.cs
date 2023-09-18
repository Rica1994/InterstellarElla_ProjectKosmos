using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MagnetPlayerSpace : MonoBehaviour
{
    //private List<GameObject> _pickupsToAttract = new List<GameObject>();

    [SerializeField]
    private ParticleSystem _particleMagnetizing;


    private void OnTriggerEnter(Collider other)
    {
        if (other.TryGetComponent(out PickUp pickup) == true)
        {
            //if (_pickupsToAttract.Contains(other.gameObject) == false)
            //{
            other.gameObject.AddComponent<MagnetPickupSpace>().Target = this.transform;
            //_pickupsToAttract.Add(other.gameObject);
            //}
        }
    }

    public void StartParticleSystem(bool started = true)
    {
        if (started == true)
        {
            _particleMagnetizing.Play();
        }
        else
        {
            _particleMagnetizing.Stop();
        }
    }
    public void EnableFullObject(bool enabled = true)
    {
        this.gameObject.SetActive(enabled);
    }
}
