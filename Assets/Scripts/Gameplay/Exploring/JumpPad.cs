using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class JumpPad : MonoBehaviour
{
    [Header("Target")]
    [SerializeField]
    private Transform _target;

    public float LaunchSpeed = 5f; // is automatically adjusted 

    [SerializeField]
    private Transform _transformDirectionXZ;
    [SerializeField]
    private Transform _transformDirectionY;

    [Header("Zoom params")]
    [SerializeField] private float _zoomDuration = 3f;
    [SerializeField, Range(0.01f, 0.5f)] private float _zoomFactor = 0.02f;
    [SerializeField, Range(1f, 4f)] private float _zoomLimit = 2.5f;


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

    [SerializeField]
    private float _currentTimeOfFlight;

    private bool _runningCoroutine;


    private void Start()
    {
        HideArcVisuals();
    }

    private void HideArcVisuals()
    {
        _lineRenderer.gameObject.SetActive(false);
    }

    private void OnTriggerEnter(Collider other)
    {
        // perhaps use layer matrix to optimize this
        if (other.TryGetComponent(out EllaExploring exploringScript))
        {
            if (_runningCoroutine == false)
            {
                // disable the character component
                StartCoroutine(TogglePlayerInput(exploringScript));

                // set the rigidbody velocity
                exploringScript.Rigid.velocity = _transformDirectionY.up * LaunchSpeed;

                // set the playier in proper direction and animation
                exploringScript.JumpPadAnimater(_transformDirectionXZ);

                // add zoom
                ServiceLocator.Instance.GetService<VirtualCameraManagerExploring>().ZoomOutCamera(_zoomDuration, _zoomFactor, _zoomLimit);
            }           
        }
    }

    private IEnumerator TogglePlayerInput(EllaExploring exploringScript)
    {
        _runningCoroutine = true;

        exploringScript.CharacterControl.enabled = false;
        exploringScript.Collider.enabled = true;

        //Debug.Log("time in coroutine -> " + _currentTimeOfFlight);

        yield return new WaitForSeconds(_currentTimeOfFlight);

        exploringScript.CharacterControl.enabled = true;
        exploringScript.Collider.enabled = false;

        //Debug.Log(exploringScript.CharacterControl.isGrounded); 
        
        // this here is key to bug !!!
        // when entering pad -> CC.isGrounded = true, we then shut off CC with it remembering it having been grounded
        // afterwards when we re-enable the CC, it simply uses its previous check (which could be in-accurate if arrival point is midair)

        // possible quickfix -> don't check for grounded animation until one update loop has happened 
        //                  OR, restructure code to check for grounded near and of update (used this)

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

        // set angle for propulsion
        _transformDirectionXZ.rotation = Quaternion.LookRotation(-direction) * Quaternion.Euler(-90, -90, 0);
        var propulsionAngle = (_currentAngle * Mathf.Rad2Deg);
        _transformDirectionY.localRotation = Quaternion.Euler(90, 90, 0) * Quaternion.AngleAxis(propulsionAngle, Vector3.forward);

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
