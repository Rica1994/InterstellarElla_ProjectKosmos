using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityCore.Audio;
using UnityCore.Scene;
using UnityEngine;
using UnityEngine.SceneManagement;
using Object = UnityEngine.Object;

[DefaultExecutionOrder(-200)]
public class LevelManager : Service
{
    #region Events

    public delegate void SectionCallback(Section section);
    public event SectionCallback OnSectionLoaded;

    #endregion
    
    [SerializeField]
    private List<Section> _sectionPrefabs = new List<Section>();
    public List<Section> Sections => _sectionPrefabs;

    public List<Section> SectionsInstantiated = new List<Section>(); 

    [Header("Triggers")]
    [SerializeField]
    private TriggerHandler _endGameTrigger;
    [SerializeField]
    private SceneType _nextScene;
    public SceneType NextScene => _nextScene;

    [Header("Respawn Logic")]
    [SerializeField]
    private GameObject _firstCheckPoint;
    
    [SerializeField]
    private float _maxHitIntervalThreshold = 2.0f;
    
    [SerializeField]
    private int _maxAmountHitsForRespawn = 3;
    
    private List<TriggerHandler> _deathTriggers = new List<TriggerHandler>();
    private TriggerHandler _lastLoadingTrigger;
    private TriggerHandler _lastDestroyingTrigger;

    private int _currentSectionIndex;

    // store a new checkpoint gameobject on here when entering trigger
    private GameObject _currentCheckpoint;
    public GameObject CurrentCheckpoint => _currentCheckpoint;
    
    private string[] _sceneStringArrray;

    private string _levelIndexString;
    private string _levelSceneIndexString;
    private string _sectionIndexString;
    private int _sectionIndex;

    private string _sectionNameBase; // this stays the same for the current scene
    private string _sectionNameToLoad; 

    private const string _sectionNamePrefix = "PV_LevelSection_S_Level"; // is follow by "_0_1_2"
    // where 0 == level index, 1 == scene index, 2 == section index

    private bool _hasEndedLevel;

    private float _timeSinceLastHit = 0.0f;
    private int _amountTimesHit = 0;

    [Header("Cutscene")]
    public Transform cutsceneCameras;

    #region Unity Functions

    private void Awake()
    {
        // subscribe endgame triggers
        if (_endGameTrigger != null) _endGameTrigger.OnTriggered += OnEndGameTriggered;

        // subscribe death triggers
        List<DeathTrigger> deathTriggers = GetComponentsInChildren<DeathTrigger>().ToList();
        for (int i = 0; i < deathTriggers.Count; i++)
        {
            var trigger = deathTriggers[i];

            _deathTriggers.Add(trigger);
            trigger.OnTriggered += OnDeathTriggered;
        }

        if (_firstCheckPoint == null)
        {
            var newSpawnPoint = new GameObject("Place Holder CheckPoint");
            if (FindObjectOfType<PlayerController>())
            {
                var player = FindObjectOfType<PlayerController>().gameObject;
                newSpawnPoint.transform.SetPositionAndRotation(player.transform.position, player.transform.rotation);
                _currentCheckpoint = newSpawnPoint;
                Debug.LogWarning("First checkPoint not attached! Using Players transform instead");
            }
        }
        else _currentCheckpoint = _firstCheckPoint;
    }

    private void Start()
    {
        //_sceneStringArrray = SceneManager.GetActiveScene().name.Split("_"[0]);
        //_levelIndexString = DecodeSceneString()[0].ToString();
        //_levelSceneIndexString = DecodeSceneString()[1].ToString();

        //_sectionIndex = 0;
        //_sectionIndexString = _sectionIndex.ToString();

        //_sectionNameBase = _sectionNamePrefix + "_" + _levelIndexString + "_" + _levelSceneIndexString;
        //_sectionNameToLoad = _sectionNameBase + "_" + _sectionIndexString;

        ////Debug.Log(_sectionNameToLoad + " first section im trying to load");

        //LoadSection();


        // subscribe all checkpoint triggers
        List<CheckpointTrigger> checkpointTriggers = this.GetComponentsInChildren<CheckpointTrigger>().ToList();
        for (int i = 0; i < checkpointTriggers.Count; i++)
        {
            checkpointTriggers[i].OnTriggered += OnCheckpointTriggered;
        }
    }

    private void Update()
    {
        if (_amountTimesHit >= 1) _timeSinceLastHit += Time.deltaTime;
        if (_timeSinceLastHit > _maxHitIntervalThreshold)
        {
            _amountTimesHit = 0;
        }
    }

    #endregion


    private void LoadSection()
    {
        Object newSectionObject = Resources.Load(_sectionNameToLoad, typeof(GameObject)) as GameObject;
        if (newSectionObject == null)
        {
            Debug.LogWarning("No more sections are left to load!");
            return;
        }
        GameObject newSectionGameobject = Instantiate(newSectionObject) as GameObject;
        Section newSection = null;
        if (newSectionGameobject.TryGetComponent(out Section section))
        {
            newSection = section;
        }

        SectionsInstantiated.Add(newSection);
        OnSectionLoaded?.Invoke(newSection);

        // subscribe section triggers
        List<SectionTrigger> sectionTriggers = newSection.GetComponentsInChildren<SectionTrigger>().ToList();
        for (int i = 0; i < sectionTriggers.Count; i++)
        {
            var trigger = sectionTriggers[i];
            if (trigger.IsSectionLoader == true)
            {
                trigger.OnTriggered += OnLoadingTriggered;
            }
            else
            {
                trigger.OnTriggered += OnDestroyingTriggered;
            }
        }

        // subscribe checkpoints of loaded section 
        List<CheckpointTrigger> checkpointTriggers = newSection.GetComponentsInChildren<CheckpointTrigger>().ToList();
        CheckpointTrigger currentCheckpointTrigger = null;
        for (int i = 0; i < checkpointTriggers.Count; i++)
        {
            currentCheckpointTrigger = checkpointTriggers[i];
            currentCheckpointTrigger.OnTriggered += OnCheckpointTriggered;
        }

        // assign a checkpoint (whenever loading a new section, we currently always assign its first checkpoint)
        _currentCheckpoint = currentCheckpointTrigger.CheckpointPoint;
        
        _sectionIndex += 1;
        _sectionIndexString = _sectionIndex.ToString();

        _sectionNameToLoad = _sectionNameBase + "_" + _sectionIndexString;        
    }



    public List<int> DecodeSceneString()
    {
        List<int> numbersInSceneName = new List<int>();

        for (int num = 0; num < _sceneStringArrray.Length; num++)
        {
            if (int.TryParse(_sceneStringArrray[num], out int foundInt) == true)
            {
                numbersInSceneName.Add(foundInt);
            }
        }

        return numbersInSceneName;
    }


    private void OnEndGameTriggered(TriggerHandler trigger, Collider other, bool hasEntered)
    {
        if (hasEntered && _hasEndedLevel == false)
        {
            EndLevel();
        }
    }
    public void EndLevel()
    {
        _hasEndedLevel = true;
        ServiceLocator.Instance.GetService<GameManager>().EndGame();
    }

    private void OnDeathTriggered(TriggerHandler trigger, Collider other, bool hasEntered)
    {
        if (other.tag == "Player") // fix this with correct layer matrix !!!!
        {
            if (hasEntered)
            {
                var tempPlayer = FindObjectOfType<PlayerController>().gameObject;
                ServiceLocator.Instance.GetService<GameManager>().RespawnPlayer(tempPlayer, _currentCheckpoint);
            }
        }
    }
    private void OnCheckpointTriggered(TriggerHandler trigger, Collider other, bool hasEntered)
    {
        if (hasEntered)
        {
            // set current checkpoint to this checkpoint
            if (trigger.TryGetComponent(out CheckpointTrigger checkpointTrigger))
            {
                _currentCheckpoint = checkpointTrigger.CheckpointPoint;
            }          
        }
    }
    private void OnLoadingTriggered(TriggerHandler trigger, Collider other, bool hasEntered)
    {
        if (other.gameObject.GetComponent<PlayerController>() == null)
        {
            return;
        }
       
        // double check if we have already entered this trigger
        if (hasEntered && _lastLoadingTrigger != trigger)
        {
            LoadSection();

            // store this trigger in last trigger entered
            _lastLoadingTrigger = trigger;

            // disable the trigger
            _lastLoadingTrigger.GetComponent<Collider>().enabled = false;
        }
    }
    private void OnDestroyingTriggered(TriggerHandler trigger, Collider other, bool hasEntered)
    {
        if (hasEntered && _lastDestroyingTrigger != trigger)
        {
            // store this trigger in last trigger entered
            _lastDestroyingTrigger = trigger;

            // disable the trigger
            _lastDestroyingTrigger.GetComponent<Collider>().enabled = false;

            if (SectionsInstantiated.Count > 0)
            {
                // Destroy the oldest segment
                Destroy(SectionsInstantiated[0].gameObject);

                // update the instantiated list
                SectionsInstantiated.RemoveAt(0);
            }
        }
    }

    public void PlayerHitObstacle(SpeederGround speeder)
    {
        _amountTimesHit++;
        _timeSinceLastHit = 0.0f;

        // play sound
        speeder.PlayBounceBackSound();

        if (_amountTimesHit >= _maxAmountHitsForRespawn)
        {
            Debug.Log("Player Hit more than " + _maxAmountHitsForRespawn + " times!");
            var tempPlayer = FindObjectOfType<PlayerController>().gameObject;
            ServiceLocator.Instance.GetService<GameManager>().RespawnPlayer(tempPlayer, _currentCheckpoint, true);
        }
    }
    public void PlayerHitObstacleSpace(SpeederSpace speederSpace)
    {
        _timeSinceLastHit = 0.0f;

        // play sound
        speederSpace.PlayCollisionSound();

        // play animation
        speederSpace.PlayCollisionAnimation();

        // lose pickups
        speederSpace.LosePickups();
    }
}
