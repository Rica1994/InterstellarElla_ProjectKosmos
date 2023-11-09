using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityCore.Audio;
using UnityEngine;
using UnityEngine.SceneManagement;

[DefaultExecutionOrder(-101)]
public class PickUpManager : Service
{
    #region Events

    public delegate void PickUpDelegate(int pickUpsPickedUp);

    public event PickUpDelegate PickUpPickedUpEvent;

    #endregion

    private List<PickUp> _pickUps = new List<PickUp>();
    private List<EllaPickupType> _foundEllaPickups = new List<EllaPickupType>();

    public int PickUpsPickedUp { get; set; } = 0;
    public List<EllaPickupType> FoundEllaPickUps => _foundEllaPickups;

    public List<PickUp> PickUps => _pickUps;

    private const string _pickupNameBase = "PV_Pickup_";
    private string _pickupToLoad;


    [Header("Pickups")]
    // assign this the pickups visual of our level 
    public static GameObject PickupNormalVisual;
    // assign this in inspector (no real point loading from resources)
    public List<GameObject> PickupsSpecialVisuals = new List<GameObject>();

    [Header("Audio")]
    [SerializeField]
    private AudioElement _soundEffectPickup1;
    [SerializeField]
    private AudioElement _soundEffectPickup2;

    [Header("Pickup timer stuff")]
    [SerializeField]
    private float _pickupComboTimeLimit;
    private float _pickupComboTimer;

    private int _pickupCurrentCombo = 0;

    private void Start()
    {
        var serviceLocator = ServiceLocator.Instance;

        // below statement never happens, as it would just register a new levelmanager if the locator couldn't find one.
        //if (levelManager == null)
        //{
        //    throw new System.Exception("No LevelManager found in scene");
        //}
        
        //#if UNITY_EDITOR
        FindPickUps();
        SceneManager.sceneLoaded += OnSceneLoaded;
        //#endif

        //if (ServiceLocator.Instance.ServiceExists(typeof(LevelManager)) == true)
        //{
        //    var levelManager = ServiceLocator.Instance.GetService<LevelManager>();

        //    levelManager.OnSectionLoaded += OnSectionLoaded;

        //    // assigning proper pickups for level
        //    var levelIndexString = levelManager.DecodeSceneString()[0].ToString();
        //    _pickupToLoad = _pickupNameBase + levelIndexString;
        //    PickupNormalVisual = Resources.Load(_pickupToLoad, typeof(GameObject)) as GameObject;
        //}

        /// Disabling the event subscriptions as they don't really function ///
        // this will only subscribe the prefabs in the SCENE (works as long as script execution order is taken into account)

        // this will only subscribe the prefabs in the ASSETS (is an issue !)
        //serviceLocator.GetService<LevelManager>().Sections.ForEach(x => x.Loaded += OnSectionLoaded);
    }

    private void Update()
    {
        if (_pickupCurrentCombo > 0)
        {
            // start timer
            _pickupComboTimer += Time.deltaTime;

            // limit the timer, reset values
            if (_pickupComboTimer >= _pickupComboTimeLimit)
            {
                _pickupCurrentCombo = 0;
                _pickupComboTimer = 0;
                _soundEffectPickup1.Pitch = 1;
                _soundEffectPickup2.Pitch = 1;
            }
        }
    }

    private void OnSceneLoaded(Scene arg0, LoadSceneMode arg1)
    {
        FindPickUps();
    }

    private void FindPickUps()
    {
        var pickUps = FindObjectsOfType<PickUp>();
        foreach (PickUp pickUp in pickUps)
        {
            pickUp.OnPickUp += OnPickUpPickedUp;
            
        }
        _pickUps.AddRange(pickUps);
    }

    /// <summary>
    /// Gets called this whenever a new segment is loaded with the LevelManager
    /// </summary>
    /// <param name="section"></param>
    private void OnSectionLoaded(Section section)
    {
        var pickUps = section.PickUps;

        // this ...
        // 1) gets all the pickups in 1 section
        // 2) subscribes the pickups to their event
        // 3) adds them to this list
        for (int i = pickUps.Count - 1; i >= 0; i--)
        {
            pickUps[i].OnPickUp += OnPickUpPickedUp;

            if (pickUps[i] as EllaPickUp)
            {
                pickUps.RemoveAt(i);
            }
            
        }

        // Add all pickups in the section to the list
        _pickUps.AddRange(pickUps);
    }

    private void OnPickUpPickedUp(PickUp pickup)
    {
        if (pickup.PickUpType == PickUp.Type.Special) PickUpsPickedUp += 3;
        else PickUpsPickedUp++;

        if (pickup as EllaPickUp)
        {
            _foundEllaPickups.Add(((EllaPickUp)pickup).Type);
           ServiceLocator.Instance.GetService<AudioController>().PlayAudio(_soundEffectPickup2);
           AdjustPitch(_pickupCurrentCombo, true);
        }
        else
        {

            // play sound with specified pitch
            ServiceLocator.Instance.GetService<AudioController>().PlayAudio(_soundEffectPickup1);
            AdjustPitch(_pickupCurrentCombo);
            // adjust the combo-pitch           
        }


        //// spawn particle
        //ServiceLocator.Instance.GetService<ParticleManager>().
        //CreateParticleWorldSpace(ParticleType.PS_PickupTrigger, this.transform.position);

        PickUpPickedUpEvent?.Invoke(PickUpsPickedUp);
    }


    private void AdjustPitch(int currentCombo, bool specialPickup = false)
    {
        _pickupCurrentCombo += 1;

        // only increase pitch 5 times
        if (currentCombo <= 10)
        {
            if (specialPickup)
            {
                _soundEffectPickup2.Pitch += 0.05f;
            }
            else _soundEffectPickup1.Pitch += 0.05f;
        }

        _pickupComboTimer = 0;
    }
}
