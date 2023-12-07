using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[DefaultExecutionOrder(-999)]
public class ClientOrDevEnabler : MonoBehaviour
{
    [SerializeField]
    private GameManager.BuildType _buildType;

    private void Awake()
    {
        if (ServiceLocator.Instance.GetService<GameManager>().TargetBuildType != _buildType)
        {
            gameObject.SetActive(false);
        }
    }
}