using Cinemachine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Playables;

public class StartCutscene : MonoBehaviour
{
    [SerializeField]
    private GameObject _dynamo;

    [SerializeField]
    private PlayableDirector _playableDirector;

    private void OnTriggerEnter(Collider other)
    {
        if (other.GetComponent<SpeederGround>())
        {
            _dynamo.transform.parent = transform.parent.transform;
            other.GetComponent<Autopilot>().enabled = true;
            _playableDirector.Play();
        }
    }
}
