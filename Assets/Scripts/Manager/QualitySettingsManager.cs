using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class QualitySettingsManager : MonoBehaviour
{
    [SerializeField]
    private List<GameObject> _features = new List<GameObject>();

    private void Start()
    {
        if (QualitySettings.GetQualityLevel() > 0)
        {
            for (int i = 0; i < _features.Count; ++i)
            {
                _features[i].SetActive(true);
            }
        }
    }
}
