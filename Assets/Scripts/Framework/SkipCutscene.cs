using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SkipCutscene : MonoBehaviour
{
    public void SkipTheCutscene()
    {
        ChainManager.Instance.GetChain().EndCurrentChainAction();
    }
}