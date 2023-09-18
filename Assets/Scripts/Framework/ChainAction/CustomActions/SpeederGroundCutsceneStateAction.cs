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
    private float _speederSpeed = 26;
    private Autopilot _autopilot;
    private FollowWithDamping _followWithDamping;
    private CinemachineBrain _brain;
    private LevelManager _levelManager;
    private GameObject _skipCutscene;
    private SpeederGround _speederGround;

    private void Awake()
    {
        _autopilot = FindObjectOfType<Autopilot>();
        _followWithDamping = FindObjectOfType<FollowWithDamping>();
        _brain = FindObjectOfType<CinemachineBrain>();
        _levelManager = FindObjectOfType<LevelManager>();
        _skipCutscene = FindObjectOfType<SkipCutscene>().gameObject;
        _speederGround = FindObjectOfType<SpeederGround>();
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
                _speederGround.speedForward = _speederSpeed;
                break;
            case State.Gameplay:
                _autopilot.enabled = false;
                _followWithDamping.updateMethod = FollowWithDamping.UpdateMethod.FixedUpdate;
                _followWithDamping.smoothTime = 0.05f;
                _brain.m_DefaultBlend.m_Style = CinemachineBlendDefinition.Style.EaseOut;
                _brain.m_DefaultBlend.m_Time = 2;
                _levelManager.cutsceneCameras.gameObject.SetActive(false);
                _skipCutscene.SetActive(false);
                _speederGround.speedForward = _speederSpeed;
                break;
        }
    }
}