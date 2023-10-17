using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;
using UnityEngine.Playables;

public class SkipCutscene : MonoBehaviour
{
    [HideInInspector]
    public PlayableDirector playableDirector;
    public void SkipTheCutscene()
    {
        ChainManager.Instance.GetChain().EndCurrentChainAction(playableDirector);
    }
}