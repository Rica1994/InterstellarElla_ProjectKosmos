using System;
using UnityCore.Audio;
using UnityEngine;


public class PassByAudioSource : MonoBehaviour
{
    [Serializable]
    public struct AudioSourceSetting
    {
        public Vector2 MinMaxDistance;
        public bool IsLooping;
    }

    private AudioSource audioSource;

    private TriggerHandler _triggerHandler;

    [SerializeField]
    private AudioSourceSetting _setting;

    private void Start()
    {
        _triggerHandler = GetComponent<TriggerHandler>();
        _triggerHandler.OnTriggered += OnTriggered;
    }

    private void OnTriggered(TriggerHandler me, Collider other, bool hasEntered)
    {
        if (hasEntered)
        {
            ServiceLocator.Instance.GetService<SoundManager>().PlayClipAt3DLocation(transform.position, _setting);
        }
    }

    private void OnDestroy()
    {
        _triggerHandler.OnTriggered -= OnTriggered;
    }

    public void RecalculateAudioSourceDistances()
    {
        //  var parent = transform.parent;
        //
        //  if (parent != null && parent.localScale.sqrMagnitude > 1.1f && audioSource != null)
        //  {
        //      audioSource.minDistance *= parent.localScale.magnitude;
        //      audioSource.maxDistance *= parent.localScale.magnitude;
        //  }

        _setting.MinMaxDistance.x = audioSource.minDistance;
        _setting.MinMaxDistance.y = audioSource.maxDistance;
        _setting.IsLooping = audioSource.loop;

        GetComponent<SphereCollider>().radius = _setting.MinMaxDistance.y / 2.0f + 1;
    }

#if UNITY_EDITOR
    void OnDrawGizmos()
    {
   //    audioSource = GetComponent<AudioSource>();
   //    if (audioSource.spatialBlend == 1.0f) // Check if AudioSource is set to 3D
   //    {
   //        Gizmos.color = Color.green;
   //        Gizmos.DrawWireSphere(transform.position, audioSource.minDistance);
   //        Gizmos.color = Color.red;
   //        Gizmos.DrawWireSphere(transform.position, audioSource.maxDistance);
   //    }
    }
#endif

}
