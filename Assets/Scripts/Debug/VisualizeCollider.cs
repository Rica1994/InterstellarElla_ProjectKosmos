#if UNITY_EDITOR
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class VisualizeCollider : MonoBehaviour
{
    [SerializeField]
    private Color _color = new Color(0, 1, 1, 0.5f);

    public bool DrawCollider = true;

    private void OnDrawGizmos()
    {
        if (DrawCollider)
        {
            var box = GetComponent<BoxCollider>();
            if (box == null) return;
            Gizmos.color = new Color(0, 1, 1, 0.5f);
            Gizmos.matrix = Matrix4x4.TRS(transform.position, transform.rotation, transform.localScale);
            Gizmos.DrawCube(box.center, box.size);
        }
    }

    public void ToggleVisuals(bool drawCollider)
    {
        DrawCollider = drawCollider;
    }
}
#endif