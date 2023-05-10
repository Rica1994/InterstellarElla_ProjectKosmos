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
        serviceLocator.GetService<LevelManager>().Sections.ForEach(x => x.Loaded += OnSectionLoaded);
    }

    private void OnSectionLoaded(Section section)
    {
        var pickUps = section.GetComponentsInChildren<PickUp>(true).ToList();
        for (int i = pickUps.Count - 1; i >= 0; i--)
        {
            pickUps[i].OnPickUp += OnPickUpPickedUp;
            if (pickUps[i] as EllaPickUp) pickUps.RemoveAt(i);
        }
        
        // Add all pickups in the section to the list
        _pickUps.AddRange(pickUps);
    }

    private void OnPickUpPickedUp(PickUp pickup)
    {
        if (pickup as EllaPickUp) _foundEllaPickUps += ((EllaPickUp)pickup).Letter;
        else _pickUpsPickedUp++;
    }
}
