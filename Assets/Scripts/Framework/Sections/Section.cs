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

    [Header("Respawn Point")]
    [SerializeField]
    private GameObject _checkPoint;
    public GameObject Checkpoint => _checkPoint;

    [Header("Assign my 2 children")]
    [SerializeField]
    private GameObject _parentPickups;
    public GameObject ParentPickups => _parentPickups;
    [SerializeField]
    private GameObject _parentEnvironment;
    public GameObject ParentEnvironment => _parentEnvironment;



    #region Public Functions

    public void RespawnPlayer()
    {

    }

    #endregion


    #region Private Functions

    private void OnEnable()
    {
        //Debug.Log("Enabled a Section");
        Loaded?.Invoke(this);

        // delaying the logic allow the pickupManager to properly subscribe to Loaded
        //StartCoroutine(DelayEnable());      
    }


    IEnumerator DelayEnable()
    {
        yield return new WaitForSeconds(0.5f);

        Debug.Log("Enabled a Section");
        Loaded?.Invoke(this);
    }

    #endregion
}
