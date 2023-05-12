using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class LevelManager : Service
{
    [Header("Assign prefabs")]
    [SerializeField]
    private List<Section> _sectionPrefabs = new List<Section>();
    [Header("Assign scene present sections")]
    [SerializeField]
    private List<Section> _sectionsInstantiated = new List<Section>(); 

    [Header("Triggers")]
    [SerializeField]
    private TriggerHandler _endGameTrigger;
    private List<SectionTrigger> _loadingTriggers = new List<SectionTrigger>();
    private List<SectionTrigger> _destroyingTriggers = new List<SectionTrigger>();
    private List<DeathTrigger> _deathTriggers = new List<DeathTrigger>();
    private TriggerHandler _lastLoadingTrigger;
    private TriggerHandler _lastDestroyingTrigger;

    private int _currentSectionIndex;

    private GameObject _currentCheckpoint;

    public List<Section> SectionsInstantiated => _sectionsInstantiated;
    public List<Section> Sections => _sectionPrefabs;

    // to delete
    [Header("Testing particles")]
    public ParticleType ParticleWorld;
    public ParticleType ParticleLocal;


    #region Unity Functions

    protected override void Awake()
    {
        base.Awake();

        // subscribe endgame triggers
        _endGameTrigger.OnTriggered += OnEndGameTriggered;

        // subscribe section triggers
        List<SectionTrigger> sectionTriggers = GetComponentsInChildren<SectionTrigger>().ToList();
        for (int i = 0; i < sectionTriggers.Count; i++)
        {
            var trigger = sectionTriggers[i];
            if (trigger.IsSectionLoader == true)
            {
                _loadingTriggers.Add(trigger);
                trigger.OnTriggered += OnLoadingTriggered;
            }
            else
            {
                _destroyingTriggers.Add(trigger);
                trigger.OnTriggered += OnDestroyingTriggered;
            }
        }

        // subscribe death triggers
        List<DeathTrigger> deathTriggers = GetComponentsInChildren<DeathTrigger>().ToList();
        for (int i = 0; i < deathTriggers.Count; i++)
        {
            var trigger = deathTriggers[i];

            _deathTriggers.Add(trigger);
            trigger.OnTriggered += OnDeathTriggered;
        }

        // check what sections are currently instantiated in the scene (typically 1 or 2 sections)
        // this decides from what point we start instantiating new segments (decides index)
        _currentSectionIndex = _sectionsInstantiated.Count - 1;

        // don't forget to call loading logic for already existing segments
        PickUpManager pickupManager = ServiceLocator.Instance.GetService<PickUpManager>();
        for (int i = 0; i < _sectionsInstantiated.Count; i++)
        {
            pickupManager.SectionLoaded(_sectionsInstantiated[i]);
        }
        // set initial checkpoint
        _currentCheckpoint = _sectionsInstantiated[0].Checkpoint;
    }

    #endregion






    private void OnEndGameTriggered(TriggerHandler trigger, Collider other, bool hasEntered)
    {
        if (hasEntered)
        {
            ServiceLocator.Instance.GetService<GameManager>().EndGame();
        }
    }

    private void OnDeathTriggered(TriggerHandler trigger, Collider other, bool hasEntered)
    {
        if (hasEntered)
        {
            var tempPlayer = FindAnyObjectByType<ToDeleteMoveScript>().gameObject;
            ServiceLocator.Instance.GetService<GameManager>().RespawnPlayer(tempPlayer, _currentCheckpoint);
        }
    }

    private void OnLoadingTriggered(TriggerHandler trigger, Collider other, bool hasEntered)
    {
        // double check if we have already entered this trigger
        if (hasEntered && _lastLoadingTrigger != trigger)
        {
            //Debug.Log("Entered loading trigger");

            // increase index
            _currentSectionIndex += 1;

            // double check if anything exists in this index
            if (_currentSectionIndex < _sectionPrefabs.Count)
            {
                // store this trigger in last trigger entered
                _lastLoadingTrigger = trigger;

                // disable the trigger
                _lastLoadingTrigger.GetComponent<Collider>().enabled = false;

                // instantiate section dependant on index
                Section instantiatedSection = Instantiate(_sectionPrefabs[_currentSectionIndex]);
                _sectionsInstantiated.Add(instantiatedSection);

                // set checkpoint
                _currentCheckpoint = instantiatedSection.Checkpoint;

                // testing particles
                ServiceLocator.Instance.GetService<ParticleManager>().CreateParticleWorldSpace(ParticleWorld, _currentCheckpoint.transform.position);
                var player = FindAnyObjectByType<ToDeleteMoveScript>();
                ServiceLocator.Instance.GetService<ParticleManager>().CreateParticleLocalSpace(ParticleLocal, player.transform);

                // call PickupManager logic
                ServiceLocator.Instance.GetService<PickUpManager>().SectionLoaded(instantiatedSection);
            }
            else
            {
                Debug.LogWarning("You have entered a trigger for which no Section can be loaded! index out of bounds");
            }
        }
    }
    private void OnDestroyingTriggered(TriggerHandler trigger, Collider other, bool hasEntered)
    {
        if (hasEntered && _lastDestroyingTrigger != trigger)
        {
            //Debug.Log("Entered destroying trigger");

            // store this trigger in last trigger entered
            _lastDestroyingTrigger = trigger;

            // disable the trigger
            _lastDestroyingTrigger.GetComponent<Collider>().enabled = false;

            if (_sectionsInstantiated.Count > 0)
            {
                // Destroy the oldest segment
                Destroy(_sectionsInstantiated[0].gameObject);

                // update the instantiated list
                _sectionsInstantiated.RemoveAt(0);
            }
        }
    }
}
