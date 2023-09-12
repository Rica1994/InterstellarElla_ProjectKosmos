using Cinemachine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(TriggerHandler))]
public class AimCamera : MonoBehaviour
{
    private TriggerHandler _triggerHandler;

    [SerializeField]
    private float _lerpSpeed = 1.0f; // Speed of the rotation

    [SerializeField]
    private float _lerpDuration = 1.0f; // Duration of the rotation

    [SerializeField]
    private bool _useHeightOffset = false;

    [SerializeField]
    private Vector3 _shoulderOffset; 

    private Coroutine rotationCoroutine;

    // Start is called before the first frame update
    void Start()
    {
        _triggerHandler = GetComponent<TriggerHandler>();
        _triggerHandler.OnTriggered += OnTriggered;
    }

    private void OnTriggered(TriggerHandler me, Collider other, bool hasEntered)
    {
        var carController = other.GetComponent<SimpleCarController>();

        if (hasEntered && carController != null)
        {
            if (rotationCoroutine != null)
            {
                StopCoroutine(rotationCoroutine);
            }
            rotationCoroutine = StartCoroutine(MoveAndRotateTowardsTarget(carController));
        }
    }

    private IEnumerator MoveAndRotateTowardsTarget(SimpleCarController carController)
    {
        float elapsedTime = 0f;
        Quaternion initialRotation = carController.TransformFollower.transform.rotation;
        Quaternion targetRotation = transform.rotation;
        var thirdPersonFollow = carController.VirtualCamera.GetCinemachineComponent<Cinemachine3rdPersonFollow>();
        var initialFollowOffset = thirdPersonFollow.ShoulderOffset;
        var targetFollowOffset = new Vector3(initialFollowOffset.x, _useHeightOffset ? _shoulderOffset.y : initialFollowOffset.y, _shoulderOffset.z);

        while (elapsedTime < _lerpDuration)
        {
            float t = elapsedTime / _lerpDuration;
            carController.TransformFollower.transform.rotation = Quaternion.Slerp(initialRotation, targetRotation, t * _lerpSpeed);
            thirdPersonFollow.ShoulderOffset = Vector3.Slerp(initialFollowOffset, targetFollowOffset, t * _lerpSpeed);
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        carController.TransformFollower.rotation = targetRotation;
        thirdPersonFollow.ShoulderOffset = targetFollowOffset;
    }
}