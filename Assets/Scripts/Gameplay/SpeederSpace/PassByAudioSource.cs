using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class PassByAudioSource : MonoBehaviour
{
    private AudioSource audioSource;

    public void RecalculateAudioSourceDistances()
    {
        var parent = transform.parent;

        if (parent != null && parent.localScale.sqrMagnitude > 1.1f && audioSource != null)
        {
            audioSource.minDistance *= parent.localScale.magnitude;
            audioSource.maxDistance *= parent.localScale.magnitude;
        }
    }

#if UNITY_EDITOR
    void OnDrawGizmos()
    {
        audioSource = GetComponent<AudioSource>();
        if (audioSource.spatialBlend == 1.0f) // Check if AudioSource is set to 3D
        {
            Gizmos.color = Color.green;
            Gizmos.DrawWireSphere(transform.position, audioSource.minDistance);
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(transform.position, audioSource.maxDistance);
        }
    }
#endif

}
