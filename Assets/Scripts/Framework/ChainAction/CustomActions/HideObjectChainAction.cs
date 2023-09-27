using UnityEngine;

public class HideObjectChainAction : ChainAction
{
    [SerializeField]
    private GameObject[] _gameObjects;

    [SerializeField]
    private float _delayTime = 1.0f;

    public override void Execute()
    {
        base.Execute();
        foreach (var go in _gameObjects)
        {
            if (_delayTime <= 0) go.SetActive(false);
            else
            {
                Helpers.Hide(go.transform, _delayTime, this);
            }
        }
    }
}