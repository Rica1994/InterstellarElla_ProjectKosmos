using UnityEngine;

public class Autopilot : MonoBehaviour
{
    public Transform followTarget; // the target to follow

    [SerializeField]
    private float followDistance = 5.0f; // the distance to maintain from the target

    [SerializeField]
    private float acceleration = 2.0f; // acceleration to reach target

    private SpeederGround speeder; // reference to our SpeederGround script

    private void Start()
    {
        speeder = GetComponent<SpeederGround>();
    }

    private void Update()
    {
        if (followTarget)
        {
            // Calculate the desired position behind the target
            Vector3 desiredPosition = followTarget.position - followTarget.forward * followDistance;

            // Calculate direction and distance to the desired position
            Vector3 toDesiredPosition = desiredPosition - transform.position;

            // We'll only consider horizontal plane for simplicity
            toDesiredPosition.y = 0;
            float currentDistance = toDesiredPosition.magnitude;

            // If we're not close enough to the desired position, move towards it
            if (currentDistance > 0.1f)  // small threshold to prevent jittery movement
            {
                // Normalize current distance relative to followDistance
                float distanceFactor = Mathf.Clamp01(currentDistance / followDistance);

                // Determine the direction to move
                Vector2 moveDirection = new Vector2(toDesiredPosition.x, toDesiredPosition.z).normalized;

                // Interpolate acceleration based on how close we are to the desired position
                moveDirection *= acceleration * distanceFactor;

                // Update the speeder's input for movement
                speeder.SetInput(moveDirection);
            }
            else
            {
                speeder.SetInput(Vector2.zero);  // Stop the speeder when at the desired position
            }
        }
    }


}
