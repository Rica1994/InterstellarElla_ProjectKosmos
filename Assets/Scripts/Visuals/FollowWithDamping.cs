using UnityEngine;

public class FollowWithDamping : MonoBehaviour
{
    public enum UpdateMethod
    {
        FixedUpdate,
        Update,
    }

    public UpdateMethod updateMethod;

    [SerializeField]
    private Transform _target; // The GameObject to follow
    public float smoothTime = 0.05f; // Time taken for the follower to catch up with the target. Decrease for faster follow.

    private Vector3 velocity = Vector3.zero; // This value is modified by the SmoothDamp function each frame.

    [Header("If in space")]
    [SerializeField]
    private Transform _transformParent;


    private void Start()
    {
        if (_target != null && _transformParent != null)
        {
            // disable target follow if both have value
            _target = null;
        }

        if (_target == null)
        {
            this.enabled = false;
        }

        if (_transformParent != null)
        {
            this.transform.SetParent(_transformParent);
            this.transform.localPosition = Vector3.zero;
        }
        else if (_target == null & _transformParent == null)
        {
            Debug.LogWarning("Maggie not set-up correctly");
        }
    }

    void FixedUpdate()
    {
        if (_target && updateMethod == UpdateMethod.FixedUpdate)
        {
            // Determine new position
            Vector3 targetPosition = _target.position;

            // SmoothDamp to the target position
            transform.position = Vector3.SmoothDamp(transform.position, targetPosition, ref velocity, smoothTime);

        }
    }

    void Update()
    {
        if (_target && updateMethod == UpdateMethod.Update)
        {
            // Determine new position
            Vector3 targetPosition = _target.position;

            if (smoothTime == 0)
            {
                transform.position = targetPosition;
            }
            else
            {
                // SmoothDamp to the target position
                transform.position = Vector3.SmoothDamp(transform.position, targetPosition, ref velocity, smoothTime);
            }
        }
    }
}
