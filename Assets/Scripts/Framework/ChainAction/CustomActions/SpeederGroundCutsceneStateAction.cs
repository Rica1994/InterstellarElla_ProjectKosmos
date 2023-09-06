using Cinemachine;
using System.Diagnostics;
using UnityEngine;
using static MouthAnimation;

public class SpeederGroundCutsceneStateAction : ChainAction
{
    enum State
    {
        Cutscene,
        Gameplay,
    }

    [SerializeField]
    private State _state;
    private Autopilot _autopilot;
    private FollowWithDamping _followWithDamping;
    private CinemachineBrain _brain;

    private void Awake()
    {
        _autopilot = FindObjectOfType<Autopilot>(true);
        _followWithDamping = FindObjectOfType<FollowWithDamping>(true);
        _brain = FindObjectOfType<CinemachineBrain>(true);
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
                break;
            case State.Gameplay:
                _autopilot.enabled = false;
                _followWithDamping.updateMethod = FollowWithDamping.UpdateMethod.FixedUpdate;
                _followWithDamping.smoothTime = 0.05f;
                _brain.m_DefaultBlend.m_Style = CinemachineBlendDefinition.Style.EaseOut;
                _brain.m_DefaultBlend.m_Time = 2;

                break;
        }
    }
}