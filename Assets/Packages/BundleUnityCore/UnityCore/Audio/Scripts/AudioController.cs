
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

namespace UnityCore
{
    namespace Audio
    {
        public class AudioController : Service
        {
            [Header("Tracks things")]
            public List<AudioTrack> TracksOST;
            public List<AudioTrack> TracksUI;
            public List<AudioTrack> TracksPlayer;
            public List<AudioTrack> TracksWorld;

            [Header("Mixer snapshots")]
            [SerializeField]
            private AudioMixerSnapshot _snapshotNormal;
            [SerializeField]
            private AudioMixerSnapshot _snapshotOSTMuted;
            [SerializeField]
            private AudioMixerSnapshot _snapshotFXMuted;
            [SerializeField]
            private AudioMixerSnapshot _snapshotALLMuted;

            // bools that get set when toggling audio
            private bool _mutedOST;
            private bool _mutedFX;

            private AudioJob _lastViableJob;

            // initialized in awake to Mixer.Normal
            private MixerType _currentMixerType;



            #region Extra Classes

            [System.Serializable]
            public class AudioTrack
            {
                public AudioType Type;
                public AudioSource Source;
            }
            private class AudioJob
            {
                public AudioElement AudioEM = new AudioElement(); 
                public AudioAction Action;               
                public bool Fade;
                public float Delay;


                // called for play()
                public AudioJob(AudioElement audioEM, AudioAction action, bool fade, float delay)
                {
                    AudioEM.Clip = audioEM.Clip;
                    AudioEM.Type = audioEM.Type;
                    AudioEM.Volume = audioEM.Volume;
                    AudioEM.Pitch = audioEM.Pitch;
                    AudioEM.RandomizePitchSlightly = audioEM.RandomizePitchSlightly;
                    AudioEM.PitchLowerLimitAddition = audioEM.PitchLowerLimitAddition;
                    AudioEM.PitchUpperLimitAddition = audioEM.PitchUpperLimitAddition;

                    Action = action;
                    Fade = fade;
                    Delay = delay;
                }

                // called for stop() 
                public AudioJob(AudioType type, AudioAction action, bool fade, float delay)
                {
                    AudioEM.Type = type;
                    Action = action;
                    Fade = fade;
                    Delay = delay;
                } 
            }
            public enum AudioAction
            {
                START,
                STOP,
                RESTART
            }

            #endregion



            #region Unity Functions

            private void Awake()
            {
                _currentMixerType = MixerType.MixerNormal;
            }

            #endregion



            #region Public Functions

            public void PlayAudio(AudioElement audioEm, bool fade = false, float delay = 0f)
            {
                AddJobClip(new AudioJob(audioEm, AudioAction.START, fade, delay));
            }
            // restart and stop should not require a specific clip, but should realize what the currently playing clip is
            public void StopAudio(AudioType type, bool fade = false, float delay = 0f)
            {
                AddJobClip(new AudioJob(type, AudioAction.STOP, fade, delay));
            }
            public void RestartAudio(AudioType type, bool fade = false, float delay = 0f)
            {
                AddJobClip(new AudioJob(type, AudioAction.RESTART, fade, delay));
            }


            // called on audio toggle buttons
            public void ToggleAudio(MixerType audioToInfluence)
            {
                if (audioToInfluence == MixerType.MixerOSTMuted)
                {
                    _mutedOST = !_mutedOST;
                }
                else
                {
                    _mutedFX = !_mutedFX;
                }

                MixerType snapshotMixerToUse = MixerType.None;
                if (_mutedFX == true && _mutedOST == true)
                {
                    snapshotMixerToUse = MixerType.MixerAllMuted;
                }
                else if (_mutedFX == true)
                {
                    snapshotMixerToUse = MixerType.MixerFXMuted;
                }
                else if (_mutedOST == true)
                {
                    snapshotMixerToUse = MixerType.MixerOSTMuted;
                }
                else
                {
                    snapshotMixerToUse = MixerType.MixerNormal;
                }

                MixerAdjustment(snapshotMixerToUse);
            }


            // called when changing scenes
            public void VerifyAudioTracks()
            {
                List<AudioTrack> tracksToRemove = new List<AudioTrack>();
                RemoveNullAudioTrack(TracksOST, tracksToRemove);
                RemoveNullAudioTrack(TracksUI, tracksToRemove);
                RemoveNullAudioTrack(TracksPlayer, tracksToRemove);
                RemoveNullAudioTrack(TracksWorld, tracksToRemove);
            }

            // called when wanting to change OST
            public void ChangeOST(AudioSource sourceToInfluence, AudioClip audioClip, float transitionSeconds)
            {
                StartCoroutine(TransitionMusic(sourceToInfluence, audioClip, transitionSeconds));
                StartCoroutine(TransitionFade(transitionSeconds));
            }

            #endregion




            #region Private Functions


            private void AddJobClip(AudioJob job)
            {
                
                IEnumerator jobRunner = null;

                if (job.AudioEM.Clip == null && _lastViableJob != null)
                {
                    job.AudioEM.Clip = _lastViableJob.AudioEM.Clip;
                    job.AudioEM.Clip = _lastViableJob.AudioEM.Clip;                   
                }

                // start job
                if (job.AudioEM.Clip != null)
                {
                    jobRunner = RunAudioJobClip(job);
                    _lastViableJob = job;

                    StartCoroutine(jobRunner);
                }
            }

            private IEnumerator RunAudioJobClip(AudioJob job)
            {
                yield return new WaitForSeconds(job.Delay);

                // figuring out what track to use
                AudioTrack trackToUse = null;
                trackToUse = FindTrackThatsCurrentlyNotPlaying(job.AudioEM.Type);

                // getting the volume/pitch and leaving them on default value if got values are 0
                float volumeToUse = 1;
                float pitchToUse = 1;
                if (job.AudioEM.Volume != 0)
                {
                    volumeToUse = job.AudioEM.Volume;
                }
                if (job.AudioEM.Pitch != 0)
                {
                    pitchToUse = job.AudioEM.Pitch;
                }

                // adjust pitch slightly if bool was checked
                if (job.AudioEM.RandomizePitchSlightly == true)
                {   
                    float bottomLimit = pitchToUse - job.AudioEM.PitchLowerLimitAddition;
                    float upperLimit = pitchToUse + job.AudioEM.PitchUpperLimitAddition;

                    float randomPitch = Random.Range(bottomLimit, upperLimit);

                    pitchToUse = randomPitch;
                }


                if (trackToUse != null)
                {
                    switch (job.Action)
                    {
                        case AudioAction.START:
                            trackToUse.Source.clip = job.AudioEM.Clip; 
                            trackToUse.Source.volume = volumeToUse;
                            trackToUse.Source.pitch = pitchToUse;
                            trackToUse.Source.Play();
                            break;
                        case AudioAction.STOP:
                            if (job.Fade == false)
                            {
                                trackToUse.Source.Stop();
                            }
                            break;
                        case AudioAction.RESTART:
                            trackToUse.Source.Stop();
                            trackToUse.Source.Play();
                            break;
                    }
                    if (job.Fade == true)
                    {
                        float initial = job.Action == AudioAction.START || job.Action == AudioAction.RESTART ? 0.0f : 1.0f;
                        float target = initial == 0 ? 1 : 0;
                        float duration = 1.0f;
                        float timer = 0.0f;

                        while (timer <= duration)
                        {
                            trackToUse.Source.volume = Mathf.Lerp(initial, target, timer / duration);
                            timer += Time.deltaTime;
                            yield return null;
                        }

                        if (job.Action == AudioAction.STOP)
                        {
                            trackToUse.Source.Stop();
                        }
                    }
                }

                yield return null;
            }
            private IEnumerator TransitionFade(float transitionTime)
            {
                // only do a fade effect if we have not toggle off the OST
                if (_currentMixerType != MixerType.MixerOSTMuted || _currentMixerType != MixerType.MixerAllMuted)
                {
                    _snapshotOSTMuted.TransitionTo(1.5f);

                    yield return new WaitForSeconds(transitionTime);

                    _snapshotNormal.TransitionTo(1.5f);
                }
            }
            private IEnumerator TransitionMusic(AudioSource SourceToChangeMusic, AudioClip audioClip, float transitionTime)
            {
                yield return new WaitForSeconds(transitionTime);

                SourceToChangeMusic.clip = audioClip;
                SourceToChangeMusic.Play();
            }

            private AudioTrack FindTrackThatsCurrentlyNotPlaying(AudioType audioType)
            {
                AudioTrack trackToUse = null;

                switch (audioType)
                {
                    case AudioType.OST:
                        for (int i = 0; i < TracksOST.Count; i++)
                        {
                            if (TracksOST[i].Source.isPlaying == false)
                            {
                                trackToUse = TracksOST[i];
                                return trackToUse;
                            }
                        }
                        trackToUse = TracksOST[0];
                        return trackToUse;

                    case AudioType.SFX_UI:
                        for (int i = 0; i < TracksUI.Count; i++)
                        {
                            if (TracksUI[i].Source.isPlaying == false)
                            {
                                trackToUse = TracksUI[i];
                                return trackToUse;
                            }
                        }
                        trackToUse = TracksUI[0];
                        return trackToUse;
                    case AudioType.SFX_Player:
                        for (int i = 0; i < TracksPlayer.Count; i++)
                        {
                            if (TracksPlayer[i].Source.isPlaying == false)
                            {
                                trackToUse = TracksPlayer[i];
                                return trackToUse;
                            }
                        }
                        trackToUse = TracksPlayer[0];
                        return trackToUse;
                    case AudioType.SFX_World:
                        for (int i = 0; i < TracksWorld.Count; i++)
                        {
                            if (TracksWorld[i].Source.isPlaying == false)
                            {
                                trackToUse = TracksWorld[i];
                                return trackToUse;
                            }
                        }
                        trackToUse = TracksWorld[0];
                        return trackToUse;
                    default:
                        Debug.Log("you forgot to add an AudioType to an AudioElement");
                        return null;
                }
            }           
            private void RemoveNullAudioTrack(List<AudioTrack> trackListToCheck, List<AudioTrack> tracksToRemove)
            {
                for (int i = 0; i < trackListToCheck.Count; i++)
                {
                    if (trackListToCheck[i].Source == null)
                    {
                        tracksToRemove.Add(trackListToCheck[i]);
                    }
                }
                for (int i = 0; i < tracksToRemove.Count; i++)
                {
                    trackListToCheck.Remove(tracksToRemove[i]);
                }
                tracksToRemove.Clear();
            }

            // when toggling audio settings
            private void MixerAdjustment(MixerType mixerType)
            {
                switch (mixerType)
                {
                    case MixerType.None:
                        Debug.Log("incorrect mixer has been assigned, nothing happens");
                        break;
                    case MixerType.MixerNormal:
                        _snapshotNormal.TransitionTo(0.1f);
                        _currentMixerType = MixerType.MixerNormal;
                        break;
                    case MixerType.MixerOSTMuted:
                        _snapshotOSTMuted.TransitionTo(0.1f);
                        _currentMixerType = MixerType.MixerOSTMuted;
                        break;
                    case MixerType.MixerFXMuted:
                        _snapshotFXMuted.TransitionTo(0.1f);
                        _currentMixerType = MixerType.MixerFXMuted;
                        break;
                    case MixerType.MixerAllMuted:
                        _snapshotALLMuted.TransitionTo(0.1f);
                        _currentMixerType = MixerType.MixerAllMuted;
                        break;
                }
            }

            #endregion
        }
    }
}
