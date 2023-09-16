using UnityEngine;
using UnityEngine.Events;

/// <summary>
/// A simple GameEventListener that can listen for an event with no parameters.
/// </summary>
public class GameEventListener : MonoBehaviour
{
    /// <summary>
    /// The GameEvent this listener script will listen changes for.
    /// </summary>
    [Tooltip("Event to register with.")]
    public GameEventScriptableObject Event;

    /// <summary>
    /// The response to invoke when Event is raised.
    /// </summary>
    [Tooltip("Response to invoke when Event is raised.")]
    public UnityEvent Response;
    
    private void OnEnable()
    {
        Event.RegisterListener(this);
    }

    private void OnDisable()
    {
        Event.UnregisterListener(this);
    }

    /// <summary>
    /// The function that will be called when Event occurs.
    /// </summary>
    public void OnEventRaised()
    {
        Response.Invoke();
    }
}
