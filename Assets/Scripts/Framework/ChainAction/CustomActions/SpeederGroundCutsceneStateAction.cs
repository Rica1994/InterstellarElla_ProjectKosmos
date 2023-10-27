using Cinemachine;
using UnityCore.Audio;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.Timeline;
using System.Linq;
using System.Collections;

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
    private bool _smoothSpeedTransition = false;
    [SerializeField]
    private float _dynamoSpeedScale = 7;
    [SerializeField]
    private float _dynamoTargetDistance = 35;
    [SerializeField]
    private bool _autopilotSwitch = true;
    [SerializeField]
    private bool _speederGroundSwitch = true;
    [SerializeField]    
    private bool _characterControllerSwitch = true;
    [SerializeField]
    private bool _speederParticleSwitch = true;
    [SerializeField]
    private bool _dynamoSwitch = true;
    [SerializeField]
    private bool _switchDirection = false;

    [SerializeField]
    private bool _cutsceneCameras = true;

    [SerializeField]
    private Transform _transform;
    [SerializeField]
    private float _cameraBlendTime = 0;

    private Autopilot _autopilot;
    private FollowWithDamping _followWithDamping;
    private CinemachineBrain _brain;
    private LevelManager _levelManager;
    private GameObject _skipCutscene;
    private SpeederGround _speederGround;
    private CharacterController _characterController;
    private CinemachineDollyCart _dynamoCart;
    private DynamoDistance _dynamoDistance;

    protected override void Awake()
    {
        base.Awake();
        _autopilot = FindObjectOfType<Autopilot>();
        _followWithDamping = FindObjectOfType<FollowWithDamping>();
        _brain = FindObjectOfType<CinemachineBrain>();
        _levelManager = FindObjectOfType<LevelManager>();
        var button = FindObjectOfType<SkipCutscene>();
        if (button != null) _skipCutscene = button.gameObject;
        _speederGround = FindObjectOfType<SpeederGround>();
        _characterController = FindObjectOfType<CharacterController>();
        _dynamoCart = FindObjectOfType<CinemachineDollyCart>();
        _dynamoDistance = _dynamoCart.gameObject.GetComponent<DynamoDistance>();
    }

    public override void Execute()
    {
        base.Execute();
        switch (_state)
        {
            case State.Cutscene:
                GameManager.IsInCutscene = true;
                _autopilot.enabled = _autopilotSwitch;
                _followWithDamping.updateMethod = FollowWithDamping.UpdateMethod.Update;
                _followWithDamping.smoothTime = 0;
                if (_cameraBlendTime <= 0)
                {
                    _brain.m_DefaultBlend.m_Style = CinemachineBlendDefinition.Style.Cut;
                }
                else
                {
                    _brain.m_DefaultBlend.m_Style = CinemachineBlendDefinition.Style.EaseOut;
                    _brain.m_DefaultBlend.m_Time = _cameraBlendTime;
                }
                _levelManager.cutsceneCameras.gameObject.SetActive(_cutsceneCameras);
                if (_skipCutscene != null) _skipCutscene.SetActive(true);
                if (_smoothSpeedTransition)
                {
                    StartCoroutine(SmoothSpeedTransition());
                }
                else
                {
                    _speederGround.speedForward = _speederSpeed;
                }
                _speederGround.enabled = _speederGroundSwitch;
                _characterController.enabled = _characterControllerSwitch;
                if (!_speederParticleSwitch)
                {
                    _speederGround.GetComponent<CollisionParticle>().activate = _speederParticleSwitch;
                }
                _dynamoCart.enabled = _dynamoSwitch; 
                _dynamoDistance.enabled = _dynamoSwitch;
                _dynamoDistance.SpeedFactor = _dynamoSpeedScale;
                _dynamoDistance.TargetDistance = _dynamoTargetDistance;
                break;
            case State.Gameplay:
                GameManager.IsInCutscene = false;
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
                if (_cameraBlendTime <= 0)
                {
                    _brain.m_DefaultBlend.m_Style = CinemachineBlendDefinition.Style.Cut;
                }
                else
                {
                    _brain.m_DefaultBlend.m_Style = CinemachineBlendDefinition.Style.EaseOut;
                    _brain.m_DefaultBlend.m_Time = _cameraBlendTime;
                }
                _levelManager.cutsceneCameras.gameObject.SetActive(false);
                if (_skipCutscene != null) _skipCutscene.SetActive(false);
                _speederGround.speedForward = _speederSpeed;
                _speederGround.enabled = true;
                _characterController.enabled = true;
                if (!_speederParticleSwitch)
                {
                    _speederGround.GetComponent<CollisionParticle>().activate = true;
                }
                _dynamoCart.enabled = true;
                _dynamoDistance.enabled = true;
                _dynamoDistance.SpeedFactor = _dynamoSpeedScale;
                _dynamoDistance.TargetDistance = _dynamoTargetDistance;
                break;
        }
    }

    private IEnumerator SmoothSpeedTransition()
    {
        float startSpeed = _speederGround.speedForward;
        float speedDifferenceScale = Mathf.Abs(startSpeed - _speederSpeed)/startSpeed;
        float durationScale = 5;
        float duration = durationScale * speedDifferenceScale;
        float elapsed = 0f;

        Debug.Log("Transition duration: " + duration);

        while (elapsed < duration)
        {
            _speederGround.speedForward = Mathf.Lerp(startSpeed, _speederSpeed, elapsed / duration);
            elapsed += Time.deltaTime;
            yield return null;
        }
    }
}