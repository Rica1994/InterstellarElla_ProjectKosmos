using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class EllaPickUp : PickUp
{
    [SerializeField]
    private TMP_Text _letter;

    public char Letter => _letter.text[0];
}
