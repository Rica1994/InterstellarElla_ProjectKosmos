using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelSectionCreator : MonoBehaviour
{
    [Header("Assign children with box colliders")]
    [SerializeField]
    private List<BoxCollider> _collidersDefiningMySection = new List<BoxCollider>();
    public List<BoxCollider> CollidersDefiningMySection => _collidersDefiningMySection;

    [Header("my section index")]
    //[SerializeField]
    //private LevelType _typeOfLevel;
    //public LevelType TypeOfLevel => _typeOfLevel;
    [SerializeField]
    private int _sectionIndex;
    public int SectionIndex => _sectionIndex;

    [Header("Assign the Section child")]
    [SerializeField]
    private Section _levelSectionChild;
    public Section Section => _levelSectionChild;
}



public enum LevelType
{
    SpeederGround = 0,
    SpeederSpace = 1,
    Exploring = 2,
    Car = 3,
    All = 4
};
