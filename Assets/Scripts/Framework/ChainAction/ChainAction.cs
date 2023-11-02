using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.Serialization;

[DefaultExecutionOrder(-99)]
public class ChainAction : MonoBehaviour
{
    #region Events

    public delegate void ChainActionDelegate();

    public event ChainActionDelegate ChainActionDone;
    public event ChainActionDelegate ChainActionStarted;

    #endregion

    /// <summary>
    /// When true, the action will be completed when the user has performed a certain action.
    /// </summary>
    [SerializeField, Tooltip("When true, the action will be completed when the user has performed a certain action.")]
    protected bool _useUserBasedAction = false;

    [SerializeField, HideInInspector]
    private bool _repeat = false;

    [SerializeField, HideInInspector]
    private bool _repeatUntilRequisitIsMet = false;

    [SerializeField, HideInInspector]
    private bool _repeatNumberOfTimes = false;

    [SerializeField, HideInInspector]
    private int _timesToRepeat = 0;
    private int _timesRepeated = 0;

    [SerializeField, HideInInspector]
    private float _timeUntilNextRepeat = 10.0f;

    [SerializeField, HideInInspector]
    private RequisiteLogic _requisiteLogic = RequisiteLogic.All;

    [SerializeField, HideInInspector]
    private List<GameObject> _requisiteObjects = new List<GameObject>();

    private List<IRequisite> _requisites = new List<IRequisite>();

    protected float _maxTime = -1.0f;
    private float _elapsedTime = 0.0f;
    protected bool _userBasedActionCompleted = false;

    private bool _isBeingExecuted = false;

    public bool Repeat => _repeat;

    [SerializeField]
    private bool _playAndCompleteAction = false;

    protected virtual void Awake()
    {
        foreach (var obj in _requisiteObjects)
        {
            var requisite = obj.GetComponent<IRequisite>();
            if (requisite != null)
            {
                _requisites.Add(requisite);
            }
            else
            {
                Debug.LogWarning("One of the assigned requisiteObjects does not implement IRequisite!");
            }
        }
    }

    protected virtual void Start()
    {
        if (_playAndCompleteAction)
        {
            _maxTime = -1;
        }
    }

    public virtual void Execute()
    {
        _isBeingExecuted = true;
    }

    private void Update()
    {
        if (_isBeingExecuted)
        {
            UpdateAction(Time.deltaTime);
        }
    }

    public virtual void UpdateAction(float elapsedTime)
    {
        _elapsedTime += elapsedTime;

        if (IsCompleted())
        {
            _timesRepeated++;
            if (Repeat && AreRequisitesMet() == false)
            {
                StartCoroutine(RepeatAction(_timeUntilNextRepeat));
                return;
            }

            _isBeingExecuted = false;
            ChainActionDone?.Invoke();
        }
    }

    public virtual bool IsCompleted()
    {
        if (_useUserBasedAction) return _userBasedActionCompleted;
        return _elapsedTime >= _maxTime + Time.deltaTime;
    }

    public virtual void OnEnter()
    {
        ChainActionStarted?.Invoke();
        //Debug.Log("ChainAction: " + _nameChainAction + " started.");
    }

    public virtual void OnExit()
    {
        // ChainActionDone?.Invoke();
        //Debug.Log("ChainAction: " + _nameChainAction + " finished.");
    }

    private IEnumerator RepeatAction(float afterTime)
    {
        _isBeingExecuted = false;
        ResetChainAction();

        if (_repeatUntilRequisitIsMet)
        {
            yield return new WaitForSeconds(afterTime);
        }

        if (AreRequisitesMet() && Repeat)
        {
            ChainActionDone?.Invoke();
            yield break;
        }
        else
        {
            Execute();
        }
    }

    public bool AreRequisitesMet()
    {
        if (_repeatUntilRequisitIsMet)
        {
            if (_requisiteLogic == RequisiteLogic.All)
            {
                // All requisites must be met (AND logic)
                return _requisites.All(r => r.IsRequisiteMet());
            }
            else
            {
                // Only one requisite must be met (OR logic)
                return _requisites.Any(r => r.IsRequisiteMet());
            }
        }
        // else check for amount of times repeated
        else  return _repeatNumberOfTimes && _timesRepeated >= _timesToRepeat;
    }

    public void SetUserBasedActionComplete()
    {
        _userBasedActionCompleted = true;
    }

    private void ResetChainAction()
    {
        _elapsedTime = 0.0f;
        _userBasedActionCompleted = false;
    }
}
