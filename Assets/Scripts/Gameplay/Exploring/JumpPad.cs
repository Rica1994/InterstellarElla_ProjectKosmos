using System.Collections;
using System.Collections.Generic;
using UnityCore.Audio;
using UnityEngine;

public class JumpPad : MonoBehaviour
{
    [Header("if too Low, will automatically be adjusted")]
    public float LaunchSpeed = 5f; // is automatically adjusted 

    [Header("Status")]
    [SerializeField] private bool _isActive = true;

    [Header("Components &|| Children")]
    [SerializeField] private Collider _trigger;
    [SerializeField] private GameObject _visuals;
    [SerializeField]
    private Transform _target;
    [SerializeField]
    private Transform _transformDirectionXZ;
    [SerializeField]
    private Transform _transformDirectionY;

    [Header("Swap camera if present")]
    [SerializeField]
    private SwapCamera _swapCamera;

    [Header("Zoom params")]
    [SerializeField] private float _zoomDuration = 3f;
    [SerializeField, Range(0.01f, 0.5f)] private float _zoomFactor = 0.02f;
    [SerializeField, Range(1f, 4f)] private float _zoomLimit = 2.5f;

    [Header("Visualizations Arc")]
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

    private bool _isOnCooldown;

    [Header("Sounds")]
    [SerializeField] private AudioElement _soundActivation;
    [SerializeField] private AudioElement _soundEntered;

    [SerializeField] private AudioSource _mySoundIdleSource;


    private void Start()
    {
        HideArcVisuals();

        if (_isActive == true)
        {
            ActivateJumpPad(false);
        }
        if (_swapCamera != null)
        {
            _swapCamera.gameObject.SetActive(false);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        // perhaps use layer matrix to optimize this
        if (other.TryGetComponent(out EllaExploring exploringScript))
        {
            if (_isOnCooldown == false)
            {
                // start cooldown
                StartCoroutine(JumpPadCooldown());

                // enable the SwapCamera if present
                ActivateSwapCamera();

                // disable the character component // store this coroutine, so i could destroy it if i enter new jump pad (should be stored in player)
                exploringScript.JumpPadCCToggle(_currentTimeOfFlight);

                // set player on centre of the jumpad
                Vector3 centrePad = new Vector3(this.transform.position.x, exploringScript.transform.position.y, this.transform.position.z);
                exploringScript.transform.position = centrePad;

                // set the rigidbody velocity
                exploringScript.Rigid.velocity = _transformDirectionY.up * LaunchSpeed;

                // set the playier in proper direction and animation
                exploringScript.JumpPadAnimater(_transformDirectionXZ);

                // add zoom
                ServiceLocator.Instance.GetService<VirtualCameraManagerExploring>().ZoomOutCamera(_zoomDuration, _zoomFactor, _zoomLimit);

                // play sound effect
                ServiceLocator.Instance.GetService<AudioController>().PlayAudio(_soundEntered);
            }           
        }
    }

    private void ActivateSwapCamera()
    {
        if (_swapCamera != null)
        {
            StartCoroutine(ToggleSwapCamera());            
        }
    }
    private void HideArcVisuals()
    {
        _lineRenderer.gameObject.SetActive(false);
    }


    private IEnumerator ToggleSwapCamera()
    {
        _swapCamera.gameObject.SetActive(true);

        yield return new WaitForSeconds(_currentTimeOfFlight);

        _swapCamera.gameObject.SetActive(false);
    }
    private IEnumerator ToggleCharacterController(EllaExploring exploringScript)
    {
        _isOnCooldown = true;

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
        //     OR, restructure code to check for grounded near and of update (used this)

        _isOnCooldown = false;
    }
    private IEnumerator JumpPadCooldown()
    {
        _isOnCooldown = true;

        yield return new WaitForSeconds(3f);

        _isOnCooldown = false;
    }



    // called from start, or somewhere else when lever is procced
    public void ActivateJumpPad(bool playSoundActivation = false)
    {
        _trigger.enabled = true;

        // activate particle
        var particleIdle = ServiceLocator.Instance.GetService<ParticleManager>().CreateParticleLocalSpacePermanent(ParticleType.PS_JumpPadIdle, _visuals.transform);
        particleIdle.Play();

        // play sound activation
        if (playSoundActivation == true)
        {
            ServiceLocator.Instance.GetService<AudioController>().PlayAudio(_soundActivation);
        }

        // enable sound idle
        _mySoundIdleSource.enabled = true;
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
