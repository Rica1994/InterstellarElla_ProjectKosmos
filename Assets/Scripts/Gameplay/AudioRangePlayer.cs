using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class AudioRangePlayer : MonoBehaviour
{
    public AudioClip[] audioClips; // Array of audio clips to play
    public float minTimeBetweenClips = 1f; // Minimum time between audio clips
    public float maxTimeBetweenClips = 5f; // Maximum time between audio clips
    public float audioRange = 5f; // Range within which audio clips will play

    private AudioSource audioSource;
    private AudioClip _lastPlayedClip;
    private Transform playerTransform;
    private float timeSinceLastClip;
    private float timeUntilNextClip;

    void Start()
    {
        audioSource = GetComponent<AudioSource>();
        playerTransform = Camera.main.transform; // Assumes player is at the camera position
        ScheduleNextClip();
    }

    void Update()
    {
        timeSinceLastClip += Time.deltaTime;
        if (timeSinceLastClip >= timeUntilNextClip && Vector3.Distance(transform.position, playerTransform.position) <= audioRange)
        {
            PlayRandomClip();
            ScheduleNextClip();
        }
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(transform.position, audioRange);
    }

    void PlayRandomClip()
    {
        AudioClip toPlayClip;
        do
        {
            int clipIndex = Random.Range(0, audioClips.Length);
            toPlayClip = audioClips[clipIndex];
        } while (_lastPlayedClip == toPlayClip);

        _lastPlayedClip = toPlayClip;
        audioSource.PlayOneShot(_lastPlayedClip);
        timeSinceLastClip = 0f;
    }

    void ScheduleNextClip()
    {
        timeUntilNextClip = Random.Range(minTimeBetweenClips, maxTimeBetweenClips);
    }
}
