using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelManager : Service
{
    [SerializeField]
    private List<Section> _sections = new List<Section>();
    
    public List<Section> Sections => _sections;
}
