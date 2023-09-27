using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InvisibleWall : MonoBehaviour
{
    [SerializeField]
    private MeshRenderer _meshRenderer;

    public void ToggleVisuals(bool showThem = true)
    {
        if (_meshRenderer == null)
        {
            _meshRenderer = GetComponent<MeshRenderer>();
        }

        _meshRenderer.enabled = showThem;
    }
}
