using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoulderAnimationEvent : MonoBehaviour
{
    [SerializeField]
    private GlitchAnimatedEventBoulder _boulderEvent;

    public void EventSmashRocks()
    {
        _boulderEvent.SmashRocks();
    }

    //public void EventFlickerAndDestroy()
    //{
    //    _boulderEvent.StartCoroutine(Helpers.FlickerGo(_boulderEvent.RockwallMeshParent, 3.0f, 10));
    //    _boulderEvent.StartCoroutine(Helpers.DoAfter(3.0f, () => Destroy(_boulderEvent.RockwallMeshParent)));
    //}
}
