using UnityEngine;

public class ShowObjectChainAction : ChainAction
{
    [SerializeField]
    private GameObject[] _gameObjects;

    [SerializeField]
    private bool _showInstantly = false;

    [SerializeField]
    private float _showTime = 1.0f;

    public override void Execute()
    {
        base.Execute();
        foreach (var go in _gameObjects)
        {
            if (_showInstantly) go.SetActive(true);
            else
            {
                Helpers.Show(go.transform, _showTime);
            }
        }
    }
}