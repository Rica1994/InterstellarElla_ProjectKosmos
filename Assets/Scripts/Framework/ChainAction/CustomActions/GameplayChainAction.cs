using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameplayChainAction : ChainAction
{
    [SerializeField] private bool _isInCutscene = false;

    [SerializeField] private bool _pauseGameplay = false;
    public override void Execute()
    {
        base.Execute();

        GameManager.IsInCutscene = _isInCutscene;
    }
}
