using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Section : MonoBehaviour
{
    public delegate void SectionCallBack(Section section);
    public event SectionCallBack Loaded;
    
    /// <summary>
    /// The list of pickups in the section
    /// </summary>
    private List<PickUp> _pickUps = new List<PickUp>();

    private void OnEnable()
    {
        Loaded?.Invoke(this);
    }
}
