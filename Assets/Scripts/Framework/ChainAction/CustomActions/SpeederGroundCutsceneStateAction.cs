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
    [SerializeField]
    private bool _autopilotSwitch = true;
    [SerializeField]
    private bool _speederGroundSwitch = true;
    [SerializeField]
    private bool _characterControllerSwitch = true;
    [SerializeField]
    private bool _speederParticleSwitch = true;

    [SerializeField]
    private bool _switchDirection = false;
    [SerializeField]
    private Transform _transform;

    private Autopilot _autopilot;
    private FollowWithDamping _followWithDamping;
    private CinemachineBrain _brain;
    private LevelManager _levelManager;
    private GameObject _skipCutscene;
    private SpeederGround _speederGround;
    private CharacterController _characterController;

    private void Awake()
    {
        _autopilot = FindObjectOfType<Autopilot>();
        _followWithDamping = FindObjectOfType<FollowWithDamping>();
        _brain = FindObjectOfType<CinemachineBrain>();
        _levelManager = FindObjectOfType<LevelManager>();
        _skipCutscene = FindObjectOfType<SkipCutscene>().gameObject;
        _speederGround = FindObjectOfType<SpeederGround>();
        _characterController = FindObjectOfType<CharacterController>();
    }

    public override void Execute()
    {
        base.Execute();
        switch (_state)
        {
            case State.Cutscene:
                _autopilot.enabled = _autopilotSwitch;
                _followWithDamping.updateMethod = FollowWithDamping.UpdateMethod.Update;
                _followWithDamping.smoothTime = 0;
                _brain.m_DefaultBlend.m_Style = CinemachineBlendDefinition.Style.Cut;
                _levelManager.cutsceneCameras.gameObject.SetActive(true);
                _skipCutscene.SetActive(true);
                _speederGround.speedForward = _speederSpeed;
                _speederGround.enabled = _speederGroundSwitch;
                _characterController.enabled = _characterControllerSwitch;
                if (!_speederParticleSwitch)
                {
                    _speederGround.GetComponent<CollisionParticle>().activate = _speederParticleSwitch;
                }
                break;
            case State.Gameplay:
                if (_switchDirection)
                {
                    _speederGround.moveDirection = new Vector3(0, 0, -1);
                    _speederGround.gameObject.transform.position = _transform.position;
                    _speederGround.gameObject.transform.rotation = _transform.rotation;
                    _speederGround.speedForward = _speederSpeed;
                    _speederGround.Initialize();
                }
                _autopilot.enabled = false;
                _followWithDamping.updateMethod = FollowWithDamping.UpdateMethod.FixedUpdate;
                _followWithDamping.smoothTime = 0.05f;
                _brain.m_DefaultBlend.m_Style = CinemachineBlendDefinition.Style.EaseOut;
                _brain.m_DefaultBlend.m_Time = 2;
                _levelManager.cutsceneCameras.gameObject.SetActive(false);
                _skipCutscene.SetActive(false);
                _speederGround.speedForward = _speederSpeed;
                _speederGround.enabled = true;
                _characterController.enabled = true;

                if (!_speederParticleSwitch)
                {
                    _speederGround.GetComponent<CollisionParticle>().activate = true;
                }

                break;
        }
    }
}