using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RockWallAnimationEvent : MonoBehaviour
{
    [SerializeField]
    private RockWallNew _rockWallNew;


    public void EventFlicker()
    {
        _rockWallNew.StartCoroutine(Helpers.FlickerGo(_rockWallNew.MeshParentRocks, 3.0f, 10));
        _rockWallNew.StartCoroutine(Helpers.DoAfter(3.0f, () => Destroy(_rockWallNew.MeshParentRocks)));
    }
}
