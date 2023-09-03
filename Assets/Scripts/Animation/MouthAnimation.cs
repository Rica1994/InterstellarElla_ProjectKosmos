using UnityEngine;

public class MouthAnimation : MonoBehaviour
{
    [SerializeField]
    private AudioSource _voiceSource;
    [SerializeField]
    private SkinnedMeshRenderer _closedMouth;
    [SerializeField]
    private SkinnedMeshRenderer _halfOpenMouth;
    [SerializeField]
    private SkinnedMeshRenderer _openMouth;

    [Header("Thresholds")]
    [SerializeField]
    [Range(0f, 1f)]
    private float _thresholdHalfOpen = 0.06f;  // Default value
    [SerializeField]
    [Range(0f, 1f)]
    private float _thresholdOpen = 0.15f;      // Default value

    private int _sampleDataLength = 1024;
    private float[] samples;

    private void Start()
    {
        samples = new float[_sampleDataLength];
    }

    private void Update()
    {
        if (_voiceSource.isPlaying)
        {
            float volume = RMSValue();

            if (volume < _thresholdHalfOpen)
            {
                //Debug.Log("Closed Mouth Activated");
                _closedMouth.enabled = true;
                _halfOpenMouth.enabled = false;
                _openMouth.enabled = false;
            }
            else if (volume >= _thresholdHalfOpen && volume < _thresholdOpen)
            {
                //Debug.Log("Half Open Mouth Activated");
                _closedMouth.enabled = false;
                _halfOpenMouth.enabled = true;
                _openMouth.enabled = false;
            }
            else
            {
                //Debug.Log("Open Mouth Activated");
                _closedMouth.enabled = false;
                _halfOpenMouth.enabled = false;
                _openMouth.enabled = true;
            }
        }
    }

    float RMSValue()
    {
        _voiceSource.GetOutputData(samples, 0);

        float sum = 0;
        for (int i = 0; i < samples.Length; i++)
        {
            sum += samples[i] * samples[i];
        }
        return Mathf.Sqrt(sum / samples.Length);
    }
}
