using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

[DefaultExecutionOrder(-50)]
public class EllaPickUp : PickUp
{
    [SerializeField]
    private EllaPickupType _type;

    public EllaPickupType Type => _type;

    protected override void PlayerFeedback()
    {
        // play particle effect


        Destroy(gameObject);
    }

    protected override void LoadVisuals()
    {
        // destroy my alrdy existing child
        Destroy(this.transform.GetChild(0).gameObject);

        // instantiate corresponding object from list
        GameObject objToSpawn = ServiceLocator.Instance.GetService<PickUpManager>().PickupsSpecialVisuals[((int)_type)];
        Instantiate(objToSpawn, this.transform);
    }
}

public enum EllaPickupType
{
    Special_1 = 0,
    Special_2 = 1,
    Special_3 = 2,
    Special_4 = 3,
    Special_5 = 4
}
