using UnityEngine;
using UnityEngine.InputSystem;

namespace Player
{
    /// <summary>
    /// Inherit from this abstract base class to create controller components that respond to input system events.
    /// </summary>
    /// <remarks>
    /// When enabled, a <c>Controller</c> component will begin listening for input events. When disabled, it will stop
    /// listening for such events. Concrete derived controllers must define which input events to listen for.
    /// </remarks>
    [RequireComponent(typeof(PlayerInput))]
    public abstract class InputHandler : MonoBehaviour
    {
        protected void OnDisable()
        {
            if (!TryGetComponent(out PlayerInput input)) { return; }

            UnregisterEventHandlers(input);
        }

        protected void OnEnable()
        {
            if (!TryGetComponent(out PlayerInput input)) { return; }

            RegisterEventHandlers(input);
        }

        /// <summary>
        /// Attempts to register with the input system as a listener for player control events.
        /// </summary>
        /// <remarks>
        /// Gracefully fails if unable to find the <c>PlayerInput</c> component or required input actions, in which
        /// case the character simply does not move in response to any input.
        /// </remarks>
        protected abstract void RegisterEventHandlers(PlayerInput input);

        /// <summary>
        /// Attempts to remove this component from the invocation list for previously registered input events.
        /// </summary>
        protected abstract void UnregisterEventHandlers(PlayerInput input);
    }
}
