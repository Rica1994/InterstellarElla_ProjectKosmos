using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class EllaPickUp : PickUp
{
    [SerializeField]
    private EllaPickupType _type;

    public EllaPickupType Type => _type;
}

public enum EllaPickupType
{
    IN = 0,
    TER = 1,
    STEL = 2,
    LAR = 3
}
