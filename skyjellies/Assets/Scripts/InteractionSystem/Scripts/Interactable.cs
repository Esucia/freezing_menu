using JetBrains.Annotations;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

/// <summary>
/// Currently a stub. Intended functionality:
/// Allows a gameobject to be found by InteractionFinder
/// Sends out an event when InteractionFinder finds and selects it
/// Holds a list of all possible interactions
/// </summary>

public class Interactable : MonoBehaviour
{
    [SerializeField] private InteractionScriptableObject[] _interactions;
    
    [CanBeNull]
    public InteractionScriptableObject EvaluatePriority()
    {
        if (_interactions == null || _interactions.Length == 0)
        {
            Debug.LogWarning("Interactable object has no interactions registered.");
            return null;
        }

        return _interactions[0];
    }
}
