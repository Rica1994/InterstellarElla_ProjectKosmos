using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AmbientRandomSoundPlayer : MonoBehaviour
{
    [SerializeField]
    private AudioClip[] _soundClips;

    [SerializeField] 
    private AudioSource _ambientSoundSource;

    [SerializeField]
    private float _minTimeBetweenClips = 5.0f;

    [SerializeField]
    private float _maxTimeBetweenClips = 30.0f;

    private int _lastPlayedClipIndex = -1;

    private float _startMinTimeBetweenClips = 0;
    private float _startMaxTimeBetweenClips = 0;

    private void OnValidate()
    {
        _minTimeBetweenClips = Mathf.Min(_minTimeBetweenClips, _maxTimeBetweenClips - 0.01f);
        _maxTimeBetweenClips = Mathf.Max(_minTimeBetweenClips + 0.01f, _maxTimeBetweenClips);
    }

    private void Start()
    {
        StartCoroutine(PlayAmbientSound(3f));

        _startMinTimeBetweenClips = _minTimeBetweenClips;
        _startMaxTimeBetweenClips = _maxTimeBetweenClips;
    }

    private IEnumerator PlayAmbientSound(float waitTime)
    {
        yield return new WaitForSeconds(waitTime);

        int randIndex = Random.Range(0, _soundClips.Length);
        if (randIndex == _lastPlayedClipIndex)
        {
            randIndex = (randIndex + 1) % _soundClips.Length;
        }

        _lastPlayedClipIndex = randIndex;
        _ambientSoundSource.clip = _soundClips[randIndex];
        _ambientSoundSource.Play();

        yield return new WaitForSeconds(_ambientSoundSource.clip.length);

        float timeUntilNextAmbientSound = Random.Range(_minTimeBetweenClips, _maxTimeBetweenClips);

        StartCoroutine(PlayAmbientSound(timeUntilNextAmbientSound));
    }

    public void SetMaxTimeBetweenClips(float time)
    {
        _maxTimeBetweenClips = time;
    }

    public void SetMinTimeBetweenClips(float time)
    {
        _minTimeBetweenClips = time;
    }

    public void ResetTimeBetweenCracks()
    {
        _minTimeBetweenClips = _startMinTimeBetweenClips;
        _maxTimeBetweenClips = _startMaxTimeBetweenClips;
    }

    public void StopPlaying()
    {
        _ambientSoundSource.Stop();
        StopAllCoroutines();
    }

    public void StartPlaying()
    {
        StartCoroutine(PlayAmbientSound(0.0f));
    }
}
