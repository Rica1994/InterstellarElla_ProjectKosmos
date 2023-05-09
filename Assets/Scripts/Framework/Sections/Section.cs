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


    [Header("Assign my 2 children")]
    [SerializeField]
    private GameObject _parentPickups;
    public GameObject ParentPickups => _parentPickups;
    [SerializeField]
    private GameObject _parentEnvironment;
    public GameObject ParentEnvironment => _parentEnvironment;



    private void Start()
    {
        Loaded?.Invoke(this);
    }
}
