using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PickUpManager : MonoBehaviour
{
    private List<PickUp> _pickUps = new List<PickUp>();
    private List<PickUp> _ellaPickUps = new List<PickUp>();
    
    private int _pickUpsPickedUp = 0;
    private int _ellaPickUpsPickedUp = 0;
    
    private void Awake()
    {
        LevelManager.Instance.Sections.ForEach(x => x.Loaded += OnSectionLoaded);
    }

    private void OnSectionLoaded(Section section)
    {
        var pickUps = section.GetComponentsInChildren<PickUp>(true);
        foreach (var pickUp in pickUps)
        {
            pickUp.OnPickUp += OnPickUpPickedUp;
        }
        // Add all pickups in the section to the list
        _pickUps.AddRange(pickUps);
    }

    private void OnPickUpPickedUp(PickUp pickup)
    {
        if (pickup.IsSpecial) _ellaPickUpsPickedUp++;
        else _pickUpsPickedUp++;
    }
}
