using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimatedEventTrigger : MonoBehaviour
{
    [SerializeField]
    private GlitchAnimatedEvent _glitchAnimatedEvent;

    [SerializeField]
    private Collider _trigger;
    [SerializeField]
    private MeshRenderer _triggerVisual;


    private void OnTriggerEnter(Collider other)
    {
        // use layers here for collision
        if (other.TryGetComponent(out SimpleCarController glitch))
        {
            _glitchAnimatedEvent.ActivateCameraCutscene(glitch);

            _trigger.enabled = false;
        }
    }


    public void ToggleTriggerVisuals(bool showThem = true)
    {
        if (_triggerVisual == null)
        {
            Debug.Log(" my GlitchAnimatedEventBoulder is missing references to its visuals. -> " + this.gameObject.name);
        }

        if (_triggerVisual != null)
        {
            _triggerVisual.enabled = showThem;
        }
    }
}
