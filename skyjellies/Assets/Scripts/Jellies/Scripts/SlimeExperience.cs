using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;
///<summary>
/// Script: SlimeExperience.cs
/// Author: Michael Spangenberg (m_spangenberg)
/// Summary: A script that handles the exp gained by jellies and leveling up
/// This script does not take the jelly's loyalty, evolution, or hunger into account
/// It has not been tested on more than one jelly at the same time
/// Link to Trello Card: https://trello.com/c/FTKfqOR4/2513-programming-mspangenberg-jelly-leveling-and-experience-a-more-dynamic-system-between-player-and-jelly
/// TODO: Implement the save system when one is created
/// TODO: Implement other factors that could affect the leveling of a jelly, like evolution
/// </summary>
public class SlimeExperience : MonoBehaviour
{
    [Tooltip("The max level of the jelly")]
    [SerializeField]
    private int _maxLevel;
    [Tooltip("The current level of the jelly")]
    [SerializeField]
    private int _levelNum;
    [Tooltip("How much EXP the jelly has currently")]
    [SerializeField]
    private float _currentEXP;
    [Tooltip("How much EXP in total that the jelly needs to get to the next level")]
    [SerializeField]
    private float _expThreshold;
    [Tooltip("How much EXP the jelly still need to get to the next level")]
    [SerializeField]
    private float _expToNextLevel;
    [Tooltip("The multiplier for the EXP threshold to increase the EXP threshold after a level up")]
    [SerializeField]
    private float _expMultiplier = 1.5f;
    [Tooltip("A variable for testing the EXP system, not meant to be used in production code")]
    [SerializeField]
    private float _testEXPGain = 10f; 
    ///<summary>
    /// If the game is running in the editor and the spacebar is pressed, experience is added to the jelly
    /// This is for testing purposes and not meant to be used in the final production code
    /// </summary>
    void Update()
    {
    #if UNITY_EDITOR
        if(Input.GetKeyDown(KeyCode.Space))
        {
            AddEXP(_testEXPGain);
        }
    #endif
    }
    ///<summary>
    /// Levels up the jelly if the jelly has not reached the max level
    /// </summary>
    private void LevelUp()
    { 
        float _extraEXP = _currentEXP - _expThreshold; //The extra EXP after a level up
        if(_levelNum < _maxLevel)
        {
            _levelNum++; 
        }
        
        _currentEXP = _extraEXP;
        _expThreshold *= _expMultiplier; 
        _expToNextLevel = GetEXPtoNextLevel(); 
    }
    /// <summary>
    /// Calculates the EXP that the jelly needs in order to get to the next level
    /// </summary>
    public float GetEXPtoNextLevel()
    {
        return _expThreshold - _currentEXP;
    }
    /// <summary>
    /// Adds additional EXP to the currentEXP
    /// If the EXP total reaches the EXP threshold, the level up function is called
    /// The function is meant to be called from other scripts after an action that grants experience has been completed
    /// It is not right now
    /// parameter EXP: The EXP gained from the action
    /// </summary>
    public void AddEXP(float EXP)
    {
        _currentEXP += EXP; 

        if (_currentEXP >= _expThreshold)
        {
            LevelUp();
        }
        _expToNextLevel = GetEXPtoNextLevel();
    }
}
