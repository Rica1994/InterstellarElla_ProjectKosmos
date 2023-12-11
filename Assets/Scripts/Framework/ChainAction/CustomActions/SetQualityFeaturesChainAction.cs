using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SetQualityFeaturesChainAction : ChainAction
{
    public override void Execute()
    {
        base.Execute();

        ServiceLocator.Instance.GetService<QualitySettingsManager>().Initialize();
    }
}