using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[DefaultExecutionOrder(-503)]
public class QualityObject : MonoBehaviour
{
    public delegate void QualityObjectDelegate(QualityObject qualityObject);
    public event QualityObjectDelegate DestroyEvent;

    public QualitySettingsManager.QualityRank QualityRank;

    [SerializeField]
    private Behaviour[] _behaviours;

    public void EnableObject(bool enable)
    {
        if (_behaviours.Length <= 0)
        {
            gameObject.SetActive(enable);
        }
        else
        {
            for (int i = 0; i < _behaviours.Length; i++)
            {
                _behaviours[i].enabled = enable;
            }
        }
    }

    private void OnDestroy()
    {
        DestroyEvent?.Invoke(this);
    }
}
