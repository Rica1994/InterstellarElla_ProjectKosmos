using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[DefaultExecutionOrder(-100)]
public class PickUpManager : Service
{
    private List<PickUp> _pickUps = new List<PickUp>();
    
    private int _pickUpsPickedUp = 0;
    private string _foundEllaPickUps;
    
    public int PickUpsPickedUp => _pickUpsPickedUp;
    public string FoundEllaPickUps => _foundEllaPickUps;
    public List<PickUp> PickUps => _pickUps;
    
    private void Start()
    {
        var serviceLocator = ServiceLocator.Instance;


        /// Disabling the event subscriptions as they don't really function ///
        // this will only subscribe the prefabs in the SCENE (works as long as script execution order is taken into account)
        //serviceLocator.GetService<LevelManager>().SectionsInstantiated.ForEach(x => x.Loaded += OnSectionLoaded);

        // this will only subscribe the prefabs in the ASSETS (is an issue !)
        //serviceLocator.GetService<LevelManager>().Sections.ForEach(x => x.Loaded += OnSectionLoaded);
    }

    private void OnSectionLoaded(Section section)
    {
        Debug.Log("? ! ?");

        // this ...
        // 1) gets all the pickups in 1 section
        // 2) subscribes the pickups to their event
        // 3) adds them to this list
        SectionLoaded(section);
    }

    private void OnPickUpPickedUp(PickUp pickup)
    {
        if (pickup as EllaPickUp) _foundEllaPickUps += ((EllaPickUp)pickup).Letter;
        else _pickUpsPickedUp++;
    }


    // call this whenever a new segment is loaded with the LevelManager
    public void SectionLoaded(Section section)
    {
        var pickUps = section.ParentPickups.GetComponentsInChildren<PickUp>(true).ToList();

        for (int i = pickUps.Count - 1; i >= 0; i--)
        {
            pickUps[i].OnPickUp += OnPickUpPickedUp;
            if (pickUps[i] as EllaPickUp) pickUps.RemoveAt(i);
        }

        // Add all pickups in the section to the list
        _pickUps.AddRange(pickUps);
    }
}
