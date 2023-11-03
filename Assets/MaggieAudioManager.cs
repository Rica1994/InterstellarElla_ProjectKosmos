using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MaggieAudioManager : MonoBehaviour
{
    [SerializeField]
    private MouthAnimation _mouthAnimation;

    [SerializeField]
    private AudioSource _audioSource;

    public void PlayAudio()
    {
        _mouthAnimation.VoiceSource.Stop();
        //_mouthAnimation.VoiceSource = _audioSource;
        _audioSource.Play();
    }
}
