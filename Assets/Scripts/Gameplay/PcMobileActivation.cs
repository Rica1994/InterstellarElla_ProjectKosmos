using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PcMobileActivation : MonoBehaviour
{
    [SerializeField]
    private bool _activeForPc = false;

    private void Awake()
    {
        gameObject.SetActive(!(_activeForPc && ServiceLocator.Instance.GetService<GameManager>().IsMobileWebGl));
    }
}
