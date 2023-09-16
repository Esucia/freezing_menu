using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Player.Motion
{
    /// <summary>
    /// Provides jumping behavior for the player character in response to input system events.
    /// </summary>
    /// <remarks>
    /// <p>Object or components that need to know when the player jumps (e.g., an animator controller) should
    /// subscribe to the <c>Jumped</c> event.</p>
    /// <p>Designed separate from <c>Movement</c> so that we can selectively enable/disable this mechanic, such as when
    /// the player acquires the Trainers equipment. The player jumps once upon script <c>Start</c>, as feedback to the
    /// player indicating that they now have the ability to jump.</p>
    /// <p>Prerequisites:
    /// <list type="bullet">
    ///     <item>Active action map of the <c>PlayerInput</c> component must have an input action named "Jump"</item>
    /// </list>
    /// Note that this component will fail gracefully (and silently) if the required input action is not found.</p>
    /// </remarks>
    public class Jumping : BodyController
    {
        /// <summary>
        /// Dispatched to registered listeners when the player character jumps.
        /// </summary>
        public event System.Action Jumped;

        /// <summary>
        /// How high the player can jump (in meters).
        /// </summary>
        [SerializeField, Range(0.5f, 5.0f), Tooltip("The height the player can jump")]
        internal float _jumpHeight = 1.5f;

        /// <summary>
        /// Elapsed time that must pass before the character can jump again.
        /// </summary>
        [SerializeField, Range(0.0f, 5.0f)
         , Tooltip("Time required to pass before being able to jump again. Set to 0f to instantly jump again")]
        internal float _jumpTimeout = 0.5f;

        /// <summary>
        /// Checks if the player has obtained the trainers. Is only accessed by Trainers.cs
        /// </summary>
        public bool hasTrainers = false;

        /// <summary>
        /// Initially is 0 and updated to be the jump action contexts start time for every initial jump
        /// </summary>
        private double _jumpStartTime = 0.0;

        /// <summary>
        /// Set as a double jump but can be changed later to include more jumps if needed. 
        /// </summary>
        [SerializeField, Range(0,10),
            Tooltip("Set as a double jump but can be changed later to include more jumps if needed.")]
        private int _maxJumpCount = 2;
        private int _currentJumpCount = 0;

        private void Start()
        {

        }

        /// <summary>
        /// Performs a simple, vertical jump.
        /// </summary>
        /// <remarks>
        /// Notifies objects listening for the <c>Jumped</c> event that the player has jumped. We separate this from the
        /// <c>OnJump</c> handler and expose it publicly so that jumping can be triggered programmatically if desired.
        /// </remarks>
        private void Jump()
        {
            //Debug.Log("Jumped");
            body._verticalVelocity  = Mathf.Sqrt(_jumpHeight * -2f * body.gravity);            
            _currentJumpCount++;
            Jumped?.Invoke();
        }

        /// <summary>
        /// Handles "Jump" input actions triggered by the input system.
        /// </summary>
        /// <param name="context">
        /// should represent a "Button" action in the <c>performed</c> phase, though we never inspect the context
        /// </param>
        private void OnJump(InputAction.CallbackContext context)
        {
            if (body.grounded)
            {
                _currentJumpCount = 0;
            }
            double jumpTime = _jumpStartTime + _jumpTimeout;

            if(_currentJumpCount == 0)
            {
                _jumpStartTime = context.startTime;
                Jump();
            }
            else if(_currentJumpCount < _maxJumpCount && hasTrainers && jumpTime < context.startTime)
            {
                Jump();
            }
        }

        /// <inheritdoc/>
        /// <remarks>This controller responds to "Jump" input actions.</remarks>
        protected override void RegisterEventHandlers(PlayerInput input)
        {
            InputAction jumpAction = input.actions.FindAction("Jump");
            if (jumpAction != null)
            {
                jumpAction.performed += OnJump;
            }
        }

        /// <inheritdoc/>
        /// <remarks>This controller responds to "Jump" input actions.</remarks>
        protected override void UnregisterEventHandlers(PlayerInput input)
        {
            InputAction jumpAction = input.actions.FindAction("Jump");
            if (jumpAction != null)
            {
                jumpAction.performed -= OnJump;
            }
        }
    }
}
