using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(TriggerHandler))]
public class AimCamera : MonoBehaviour
{
    private TriggerHandler _triggerHandler;

    [SerializeField]
    private float rotationSpeed = 1.0f; // Speed of the rotation

    [SerializeField]
    private float rotationDuration = 1.0f; // Duration of the rotation

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
            rotationCoroutine = StartCoroutine(RotateTowardsTarget(carController.TransformFollower.transform));
        }
    }

    private IEnumerator RotateTowardsTarget(Transform targetTransform)
    {
        float elapsedTime = 0f;
        Quaternion initialRotation = targetTransform.rotation;
        Quaternion targetRotation = transform.rotation;

        while (elapsedTime < rotationDuration)
        {
            float t = elapsedTime / rotationDuration;
            targetTransform.rotation = Quaternion.Slerp(initialRotation, targetRotation, t * rotationSpeed);
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        targetTransform.rotation = targetRotation;
    }
}