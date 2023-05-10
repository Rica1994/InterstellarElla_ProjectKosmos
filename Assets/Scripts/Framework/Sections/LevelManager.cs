using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelManager : Service
{
    [SerializeField]
    private List<Section> _sections = new List<Section>();

    [SerializeField]
    private TriggerHandler _endGameTrigger; 
    
    public List<Section> Sections => _sections;
    
    private void Awake()
    {
        base.Awake();
        _endGameTrigger.OnTriggered += OnEndGameTriggered;
    }

    private void OnEndGameTriggered(Collider other, bool hasEntered)
    {
        if (hasEntered)
        {
            ServiceLocator.Instance.GetService<GameManager>().EndGame();
        }
    }
}
