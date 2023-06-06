using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[DefaultExecutionOrder(-100)]
public class PickUpManager : Service
{
    private List<PickUp> _pickUps = new List<PickUp>();
    
    private int _pickUpsPickedUp = 0;
    private List<EllaPickupType> _foundEllaPickups =new List<EllaPickupType>();
    
    public int PickUpsPickedUp => _pickUpsPickedUp;
    public List<EllaPickupType> FoundEllaPickUps => _foundEllaPickups;

    public List<PickUp> PickUps => _pickUps;


    // assign this the pickups visual of our level 
    public static GameObject PickupNormalVisual;
    // assign this in inspector (no real point loading from resources)
    public List<GameObject> PickupsSpecialVisuals = new List<GameObject>();

    private const string _pickupNameBase = "PV_Pickup_";
    private string _pickupToLoad;


    private void Start()
    {
        var serviceLocator = ServiceLocator.Instance;

        // below statement never happens, as it would just register a new levelmanager if the locator couldn't find one.
        //if (levelManager == null)
        //{
        //    throw new System.Exception("No LevelManager found in scene");
        //}

        if (ServiceLocator.Instance.ServiceExists(typeof(LevelManager)) == true)
        {
            var levelManager = ServiceLocator.Instance.GetService<LevelManager>();

            levelManager.OnSectionLoaded += OnSectionLoaded;


            // assigning proper pickups for level
            var levelIndexString = levelManager.DecodeSceneString()[0].ToString();
            _pickupToLoad = _pickupNameBase + levelIndexString;
            PickupNormalVisual = Resources.Load(_pickupToLoad, typeof(GameObject)) as GameObject;
        }

        /// Disabling the event subscriptions as they don't really function ///
        // this will only subscribe the prefabs in the SCENE (works as long as script execution order is taken into account)

        // this will only subscribe the prefabs in the ASSETS (is an issue !)
        //serviceLocator.GetService<LevelManager>().Sections.ForEach(x => x.Loaded += OnSectionLoaded);



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
        if (pickup as EllaPickUp)
        {
            _foundEllaPickups.Add(((EllaPickUp)pickup).Type);
        } 
        else
        {
            _pickUpsPickedUp++;
        } 
    }
}
