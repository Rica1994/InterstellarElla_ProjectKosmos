using UnityEngine;

public class FollowWithDamping : MonoBehaviour
{
    enum UpdateMethod
    {
        FixedUpdate,
        Update,
    }

    [SerializeField]
    private UpdateMethod _updateMethod;

    public Transform target; // The GameObject to follow
    public float smoothTime = 0.05f; // Time taken for the follower to catch up with the target. Decrease for faster follow.

    private Vector3 velocity = Vector3.zero; // This value is modified by the SmoothDamp function each frame.

    void FixedUpdate()
    {
        if (target && _updateMethod == UpdateMethod.FixedUpdate)
        {
            // Determine new position
            Vector3 targetPosition = target.position;

            // SmoothDamp to the target position
            transform.position = Vector3.SmoothDamp(transform.position, targetPosition, ref velocity, smoothTime);

        }
    }

    void Update()
    {
        if (target && _updateMethod == UpdateMethod.Update)
        {
            // Determine new position
            Vector3 targetPosition = target.position;

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
