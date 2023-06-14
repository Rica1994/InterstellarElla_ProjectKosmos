using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class JumpPad : MonoBehaviour
{
    [Header("Target")]
    [SerializeField]
    private Transform _target;

    [HideInInspector]
    public float LaunchSpeed = 5f; // is automatically adjusted 

    [SerializeField]
    private Transform _transformDirectionToBoost;

    [Header("Visualizations")]
    [SerializeField]
    private int _iterations = 20;
    [SerializeField]
    private LineRenderer _lineRenderer;

    //[SerializeField]
    //private Color _errorColor;
    //[SerializeField]
    //private Color _initialColor;

    private float _currentAngle;
    private float _currentTimeOfFlight;

    private bool _runningCoroutine;



    private void OnTriggerEnter(Collider other)
    {
        // perhaps use layer matrix to optimize this
        if (other.TryGetComponent(out EllaExploring exploringScript))
        {
            Debug.Log("Player entered");

            if (_runningCoroutine == false)
            {
                // disable the character component
                StartCoroutine(TogglePlayerInput(exploringScript.CharacterControl));

                // set the rigidbody velocity
                // logic from og
                var turretAngle = (_currentAngle * Mathf.Rad2Deg);
                _transformDirectionToBoost.localRotation = Quaternion.Euler(90, 90, 0) * Quaternion.AngleAxis(turretAngle, Vector3.forward);

                exploringScript.Rigid.velocity = _transformDirectionToBoost.up * LaunchSpeed;
            }
            
        }
    }

    private IEnumerator TogglePlayerInput(CharacterController characterController)
    {
        _runningCoroutine = true;

        characterController.enabled = false;

        yield return new WaitForSeconds(_currentTimeOfFlight);

        characterController.enabled = true;

        _runningCoroutine = false;
    }





    // call this from external in-editor button
    public void SetTargetWithSpeed(float gravityPlayer, bool useLowAngle = false)
    {
        if (gravityPlayer == 0)
        {
            Debug.Log("Gravity set to 0 -> stopping here. Perhaps I did not find the players gravity ?");
            return;
        }

        if (gravityPlayer <= 0)
        {
            gravityPlayer *= (-1);
        }

        Vector3 direction = transform.position - _target.position;
        float yOffset = -direction.y;
        //Debug.Log(yOffset + " is the y offset " + this.gameObject.name);

        direction = ArcMath.ProjectVectorOnPlane(Vector3.up, direction);
        float distance = direction.magnitude;

        // 2 angles will be calculated for a possible launch
        float angle0, angle1;
        // initial check whether the target is in range
        bool targetInRange = ArcMath.LaunchAngle(LaunchSpeed, distance, yOffset, gravityPlayer, out angle0, out angle1);
        while (targetInRange == false)
        {
            LaunchSpeed += 1;

            targetInRange = ArcMath.LaunchAngle(LaunchSpeed, distance, yOffset, gravityPlayer, out angle0, out angle1);
        }

        if (targetInRange == true)
        {
            //_currentAngle = useLowAngle ? angle1 : angle0;

            // same statement below !
            if (useLowAngle == true)
            {
                _currentAngle = angle1;
            }
            else
            {
                _currentAngle = angle0;
            }
        } 
        else
        {
            Debug.Log("target not in range for -> " + this.gameObject.name);
            return;
        }
            
        // visualizes the arc (needs to use the gravity set on the player)
        UpdateArc(LaunchSpeed, distance, gravityPlayer, _currentAngle, direction, targetInRange);

        // calculates time it would take for the launched object to reach the target
        _currentTimeOfFlight = ArcMath.TimeOfFlight(LaunchSpeed, _currentAngle, -yOffset, gravityPlayer);
    }



    // this should be called through a button to instantly set linerenderers in editor
    /// <summary>
    /// This will visualize the arc using the linerenderer component
    /// </summary>
    /// <param name="speed"></param>
    /// <param name="distance"></param>
    /// <param name="gravity"></param>
    /// <param name="angle"></param>
    /// <param name="direction"></param>
    /// <param name="valid"></param>
    public void UpdateArc(float speed, float distance, float gravity, float angle, Vector3 direction, bool valid)
    {
        Vector2[] arcPoints = ArcMath.ProjectileArcPoints(_iterations, speed, distance, gravity, angle);
        Vector3[] points3d = new Vector3[arcPoints.Length];

        for (int i = 0; i < arcPoints.Length; i++)
        {
            points3d[i] = new Vector3(0, arcPoints[i].y, arcPoints[i].x);
        }

        _lineRenderer.positionCount = arcPoints.Length;
        _lineRenderer.SetPositions(points3d);

        _lineRenderer.transform.rotation = Quaternion.LookRotation(-direction);

        //_lineRenderer.sharedMaterial.color = valid ? _initialColor : _errorColor;
    }
}
