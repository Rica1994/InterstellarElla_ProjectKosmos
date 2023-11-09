using System.Collections;
using System.Collections.Generic;
using UnityCore.Audio;
using UnityEngine;
using UnityEngine.UI;

public class SettingsMenu : MonoBehaviour
{
    [SerializeField]
    private Slider _musicSlider;

    [SerializeField]
    private Slider _sfxSlider;

    // Start is called before the first frame update
    void Start()
    {
        // Add listeners for the sliders
        _musicSlider.onValueChanged.AddListener(SetMusicVolume);
        _sfxSlider.onValueChanged.AddListener(SetSFXVolume);
    }

    private void SetMusicVolume(float volume)
    {
        var soundTrackManager = ServiceLocator.Instance.GetService<SoundManager>();
        // Apply the volume to all music audio sources managed by SoundtrackManager
        soundTrackManager.SetMusicVolume(volume);

        // Save the preference
       // PlayerPrefs.SetFloat("MusicVolume", volume);
    }

    private void SetSFXVolume(float volume)
    {
        var soundTrackManager = ServiceLocator.Instance.GetService<SoundManager>();
        var audioController = ServiceLocator.Instance.GetService<AudioController>();

        // Apply the volume to all SFX audio sources managed by SoundtrackManager
        soundTrackManager.SetSFXVolume(volume);
        audioController.SetSFXVolume(volume);

        // Save the preference
        //PlayerPrefs.SetFloat("SFXVolume", volume);
    }
}
