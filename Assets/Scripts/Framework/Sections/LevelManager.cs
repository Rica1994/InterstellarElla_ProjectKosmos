using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LevelManager : Service
{
    #region Events

    public delegate void SectionCallback(Section section);
    public event SectionCallback OnSectionLoaded;

    #endregion
    
    [SerializeField]
    private List<Section> _sectionPrefabs = new List<Section>();

    public List<Section> SectionsInstantiated = new List<Section>(); 

    [Header("Triggers")]
    [SerializeField]
    private TriggerHandler _endGameTrigger;
    private List<TriggerHandler> _deathTriggers = new List<TriggerHandler>();
    private TriggerHandler _lastLoadingTrigger;
    private TriggerHandler _lastDestroyingTrigger;

    private int _currentSectionIndex;

    private GameObject _currentCheckpoint;
    public List<Section> Sections => _sectionPrefabs;

    private string _levelIndexString;
    private int _sectionIndex;
    private string _sectionIndexString;
    private string _sectionNameBase;
    private string _sectionNameToLoad;
    private int[] _myNumbers;

    // to delete
    [Header("Testing particles")]
    public ParticleType ParticleWorld;
    public ParticleType ParticleLocal;


    #region Unity Functions

    private void Awake()
    {
        // subscribe endgame triggers
        _endGameTrigger.OnTriggered += OnEndGameTriggered;

        // subscribe death triggers
        List<DeathTrigger> deathTriggers = GetComponentsInChildren<DeathTrigger>().ToList();
        for (int i = 0; i < deathTriggers.Count; i++)
        {
            var trigger = deathTriggers[i];

            _deathTriggers.Add(trigger);
            trigger.OnTriggered += OnDeathTriggered;
        }
    }

    private void Start()
    {
        _levelIndexString = DecodeSceneString().ToString();
        _sectionIndex = 0;
     
        _sectionNameBase = "PV_LevelSection_S_Level_" + _levelIndexString + "_Work_";

        _sectionIndexString = _sectionIndex.ToString();
        _sectionNameToLoad = _sectionNameBase + _sectionIndexString;
        
        LoadSection();     
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

        // set checkpoint // NEEDS MORE
        _currentCheckpoint = newSection.Checkpoints[0];   

        _sectionIndex += 1;
        _sectionIndexString = _sectionIndex.ToString();
        _sectionNameToLoad = _sectionNameBase + _sectionIndexString;
    }



    private int DecodeSceneString()
    {
        // Split myString wherever there's a _ and make a String array out of it.
        string[] stringArray = SceneManager.GetActiveScene().name.Split("_"[0]);
        _myNumbers = new int[stringArray.Length];

        for (int num = 0; num < stringArray.Length; num++)
        {
            if (int.TryParse(stringArray[num], out int foundInt) == true)
            {
                return foundInt;
            }
        }

        return 404;
    }
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
            var tempPlayer = FindAnyObjectByType<PlayerController>().gameObject;
            ServiceLocator.Instance.GetService<GameManager>().RespawnPlayer(tempPlayer, _currentCheckpoint);
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
}
