using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[DefaultExecutionOrder(100)]
public class Section : MonoBehaviour
{
    public delegate void SectionCallBack(Section section);
    public event SectionCallBack Loaded;

    private void Start()
    {
        Loaded?.Invoke(this);
    }
}
