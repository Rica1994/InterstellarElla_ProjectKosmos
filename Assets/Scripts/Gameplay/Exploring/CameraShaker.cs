using UnityEngine;
using Cinemachine;

[DefaultExecutionOrder(2000)]
public class CameraShaker : MonoBehaviour
{
    public CinemachineBrain cinemachineBrain;

    public float shakeAmplitude = 1.5f;
    public float shakeFrequency = 2.0f;
    public NoiseSettings shakeNoiseProfile;

    private CinemachineVirtualCamera currentVirtualCamera;
    private CinemachineBasicMultiChannelPerlin currentVirtualCameraNoise;

    private bool _isShaking = false;
    private bool _triggerShaking = false;

    void Start()
    {
        if (cinemachineBrain == null)
        {
            var player = FindObjectOfType<PlayerController>();
            if (player != null)  cinemachineBrain = player.GetComponentInChildren<CinemachineBrain>();
        }
        // Initialize the current virtual camera and noise module
        UpdateCurrentVirtualCamera();
    }

    void Update()
    {
        // If the active virtual camera changes, update the current virtual camera and noise module
        if (currentVirtualCamera == null || 
            (cinemachineBrain.ActiveVirtualCamera != null && cinemachineBrain.ActiveVirtualCamera.VirtualCameraGameObject != currentVirtualCamera.gameObject))
        {
            UpdateCurrentVirtualCamera();
        }

        // If the Cinemachine component is not set, avoid update
        if (currentVirtualCameraNoise != null)
        {
            // If Camera Shake effect is still playing
            if (_isShaking == false && _triggerShaking == true)
            {
                SetCameraShaking();               
            }
        }
    }

    private void SetCameraShaking()
    {
        _isShaking = true;
        _triggerShaking = false;
        // Only set the Cinemachine Camera Noise parameters if they are not already set
        if (currentVirtualCameraNoise.m_AmplitudeGain != shakeAmplitude || currentVirtualCameraNoise.m_FrequencyGain != shakeFrequency)
        {
            currentVirtualCameraNoise.m_AmplitudeGain = shakeAmplitude;
            currentVirtualCameraNoise.m_FrequencyGain = shakeFrequency;
        }
    }

    public void TriggerShake()
    {
        _triggerShaking = true;
    }

    public void StopShaking()
    {
        _triggerShaking = false;
        _isShaking = false;

        // If Camera Shake effect is over and amplitude is not yet reset, reset variables
        currentVirtualCameraNoise.m_AmplitudeGain = 0f;
        currentVirtualCameraNoise.m_FrequencyGain = 0f;
    }

    private void UpdateCurrentVirtualCamera()
    {
        if (cinemachineBrain.ActiveVirtualCamera == null || cinemachineBrain.ActiveVirtualCamera.VirtualCameraGameObject == null)
        {
            return;
        }
        // Set the current virtual camera
        currentVirtualCamera = cinemachineBrain.ActiveVirtualCamera.VirtualCameraGameObject.GetComponent<CinemachineVirtualCamera>();
        // Get the noise module from the current virtual camera
        currentVirtualCameraNoise = currentVirtualCamera.GetCinemachineComponent<Cinemachine.CinemachineBasicMultiChannelPerlin>();

        if (currentVirtualCameraNoise == null)
        {
            // If the noise module doesn't exist, add it and set the noise profile
            currentVirtualCameraNoise = currentVirtualCamera.AddCinemachineComponent<Cinemachine.CinemachineBasicMultiChannelPerlin>();
            currentVirtualCameraNoise.m_NoiseProfile = shakeNoiseProfile;

            if (_isShaking) SetCameraShaking();
            else StopShaking();
        }
    }
}
