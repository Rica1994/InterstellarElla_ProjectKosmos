using Cinemachine;
using UnityCore.Audio;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.Timeline;
using System.Linq;

public class SpeederGroundCutsceneStateAction : ChainAction
{
    enum State
    {
        Cutscene,
        Gameplay,
    }

    [SerializeField]
    private State _state;
    [SerializeField]
    private PlayableDirector _playableDirector;
    private Autopilot _autopilot;
    private FollowWithDamping _followWithDamping;
    private CinemachineBrain _brain;
    private LevelManager _levelManager;
    private GameObject _skipCutscene;
    //private AudioSource _maggieAudioSource;

    private void Awake()
    {
        _autopilot = FindObjectOfType<Autopilot>(true);
        _followWithDamping = FindObjectOfType<FollowWithDamping>(true);
        _brain = FindObjectOfType<CinemachineBrain>(true);
        _levelManager = FindObjectOfType<LevelManager>(true);
        _skipCutscene = FindObjectOfType<SkipCutscene>(true).gameObject;
        //_maggieAudioSource = ServiceLocator.Instance.GetService<AudioController>().TracksMaggie[0].Source as AudioSource;
    }

    public override void Execute()
    {
        base.Execute();
        switch (_state)
        {
            case State.Cutscene:
                _autopilot.enabled = true;
                _followWithDamping.updateMethod = FollowWithDamping.UpdateMethod.Update;
                _followWithDamping.smoothTime = 0;
                _brain.m_DefaultBlend.m_Style = CinemachineBlendDefinition.Style.Cut;
                _levelManager.cutsceneCameras.gameObject.SetActive(true);
                _skipCutscene.SetActive(true);
                //// Find the AudioTrack with the name "MaggieTrack"
                //TimelineAsset timeline = _playableDirector.playableAsset as TimelineAsset;
                //if (timeline == null) return;
                //TrackAsset maggieTrack = timeline.GetOutputTracks().FirstOrDefault(t => t.name == "MaggieTrack");
                //if (maggieTrack != null)
                //{
                //    _playableDirector.SetGenericBinding(maggieTrack, _maggieAudioSource);
                //}
                break;
            case State.Gameplay:
                _autopilot.enabled = false;
                _followWithDamping.updateMethod = FollowWithDamping.UpdateMethod.FixedUpdate;
                _followWithDamping.smoothTime = 0.05f;
                _brain.m_DefaultBlend.m_Style = CinemachineBlendDefinition.Style.EaseOut;
                _brain.m_DefaultBlend.m_Time = 2;
                _levelManager.cutsceneCameras.gameObject.SetActive(false);
                _skipCutscene.SetActive(false);
                break;
        }
    }
}