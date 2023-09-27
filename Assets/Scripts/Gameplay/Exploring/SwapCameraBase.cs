using Cinemachine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[DefaultExecutionOrder(-100)]
public class SwapCameraBase : MonoBehaviour
{
    [SerializeField]
    protected GameObject _target;

    [SerializeField]
    protected CinemachineVirtualCamera _virtualCamera;

    [SerializeField]
    protected float _blendSpeed = 3f;

    [SerializeField]
    protected bool _isStatic = false;

    [Header("Visual stuff")]
    [SerializeField]
    private MeshRenderer _meshRendererTrigger;
    [SerializeField]
    private GameObject _targetVisuals;


    public virtual void Start()
    {
        // disable the camera in the start
        _virtualCamera.gameObject.SetActive(false);


        // parent camera to manager
        _virtualCamera.transform.SetParent(ServiceLocator.Instance.GetService<VirtualCameraManagerExploring>().transform);

        // unparent the visuals (keep arrow in place)
        if (_target != null)
        {
            _target.transform.GetChild(0).transform.SetParent(this.transform);
        }
    }


    public void ToggleSwapCameraVisuals(bool showThem = true)
    {
        if (_meshRendererTrigger == null || _targetVisuals == null)
        {
            Debug.Log(" my swap camera is missing references to its visuals. This is expected from speederground. -> " + this.gameObject.name);
        }

        if (_meshRendererTrigger != null)
        {
            _meshRendererTrigger.enabled = showThem;
        }

        if (_targetVisuals != null)
        {
            _targetVisuals.SetActive(showThem);
        }

        // get additional children with SwapCameraTriggers, access their meshrenderer, disable/enable
        var triggerChildrenNormal = GetComponentsInChildren<SwapCameraTrigger>();
        for (int i = 0; i < triggerChildrenNormal.Length; i++)
        {
            triggerChildrenNormal[i].GetComponent<MeshRenderer>().enabled = showThem;
        }

        var triggerChildrenExploring = GetComponentsInChildren<SwapCameraTriggerExploring>();
        for (int i = 0; i < triggerChildrenExploring.Length; i++)
        {
            triggerChildrenExploring[i].GetComponent<MeshRenderer>().enabled = showThem;
        }

        var triggerChildrenGlitch = GetComponentsInChildren<SwapCameraTriggerGlitch>();
        for (int i = 0; i < triggerChildrenGlitch.Length; i++)
        {
            triggerChildrenGlitch[i].GetComponent<MeshRenderer>().enabled = showThem;
        }
    }
}
