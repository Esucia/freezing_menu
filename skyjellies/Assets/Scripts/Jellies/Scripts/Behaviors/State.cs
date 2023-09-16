using UnityEngine;

namespace Jellies.Behaviors
{
    /// <summary>
    /// Together with MonBehaviour provides minimal operations for managing Jelly state.
    /// </summary>
    /// <remarks>
    /// Use of the state pattern here is likely temporary. We should replace this with a richer AI framework
    /// such as behavior trees or a planning system.
    /// </remarks>
    public abstract class State : MonoBehaviour
    {
        /// <summary>
        /// Register with this event if you need to know when the jelly starts an action.
        /// </summary>
        public event System.Action<State> Entered;

        /// <summary>
        /// Register with this event if you need to know when the jelly stops an action.
        /// </summary>
        public event System.Action<State> Exited;

        /// <summary>
        /// Enables this state and notifies listeners that the jelly has entered this state.
        /// </summary>
        /// <remarks>
        /// Override this to customize what happens when jelly enters a derived state.
        /// </remarks>
        public virtual void Enter(GameObject entity)
        {
            enabled = true;
            Entered?.Invoke(this);
        }

        /// <summary>
        /// Disables this state and notifies listeners that the jelly has left this state.
        /// </summary>
        /// <remarks>
        /// Override this to customize what happens when jelly exits a derived state.
        /// </remarks>
        public virtual void Exit()
        {
            enabled = false;
            Exited?.Invoke(this);
        }
    }
}
