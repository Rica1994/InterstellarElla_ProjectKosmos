using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Playables;

public class SkipCutscene : MonoBehaviour
{
    public void SkipTheCutscene(PlayableDirector playableDirector)
    {
        ChainManager.Instance.GetChain().EndCurrentChainAction(playableDirector);
    }
}