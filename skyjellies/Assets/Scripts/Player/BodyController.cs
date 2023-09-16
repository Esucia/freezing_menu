using UnityEngine;

namespace Player
{
    /// <summary>
    /// Inherit from this abstract base class to make components that control the physical body of the player character
    /// in response to appropriate input events.
    /// </summary>
    [RequireComponent(typeof(Body))]
    public abstract class BodyController : InputHandler
    {
        /// <summary>
        /// Reference to the character body that this component controls.
        /// </summary>
        protected Body body;

        protected virtual void Awake()
        {
            body = GetComponent<Body>();
        }
    }
}
