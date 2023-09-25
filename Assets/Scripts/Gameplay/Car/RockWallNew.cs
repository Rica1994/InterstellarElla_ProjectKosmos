using System.Collections;
using System.Collections.Generic;
using UnityCore.Audio;
using UnityEngine;

public class RockWallNew : MonoBehaviour
{
    public bool INeedABoulder = false;
    public bool RequiresSpeedBoost = true;

    [Header("Animation of Rockwall")]
    [SerializeField]
    private Animation _animationRockWall;

    [Header("Colliders & Triggers")]
    [SerializeField]
    private Collider _rockCollider;
    [SerializeField]
    private Collider _rockTrigger;
    [Header("Collider & Trigger Visuals")]
    [SerializeField]
    private MeshRenderer _rockColliderMeshrenderer;
    [SerializeField]
    private MeshRenderer _rockTriggerMeshrenderer;

    [Header("Mesh parent of rock wall")]
    public GameObject MeshParentRocks;

    [Header("Audio")]
    [SerializeField]
    private AudioElement _soundSmash;

    private AudioController _audioController;

    [HideInInspector]
    public bool IsActivated = false;



    private void Start()
    {
        _audioController = ServiceLocator.Instance.GetService<AudioController>();

        if (INeedABoulder == true && RequiresSpeedBoost == true)
        {
            Debug.LogWarning(this.gameObject.name + " is not setup correctly !!! It wants both a boulder and speedboost ! double check your bools.");
            return;
        }
        else if (INeedABoulder == true)
        {
            // disable trigger permanently
            _rockTrigger.enabled = false;
        }
    }


    public void GlitchSmashRockWall()
    {
        _animationRockWall.Play();

        _rockCollider.enabled = false;

        _audioController.PlayAudio(_soundSmash);

        IsActivated = true;
    }

    public void BoulderSmashRockWall()
    {
        _animationRockWall.Play();

        _rockCollider.enabled = false;

        //_audioController.PlayAudio(_soundSmash);

        IsActivated = true;
    }


    public void ToggleColliderVisuals(bool showThem = true)
    {
        if (_rockColliderMeshrenderer == null || _rockTriggerMeshrenderer == null)
        {
            Debug.Log(" my Rockwall is missing references to its visuals. -> " + this.gameObject.name);
        }

        if (_rockColliderMeshrenderer != null)
        {
            _rockColliderMeshrenderer.enabled = showThem;
        }

        if (_rockTriggerMeshrenderer != null)
        {
            _rockTriggerMeshrenderer.enabled = showThem;
        }
    }
}
