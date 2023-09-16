using UnityEngine;
using UnityEngine.InputSystem;

namespace Player.Motion
{
    /// <summary>
    /// Inherit from this base class to implement walk/run behaviors for the player character.
    /// </summary>
    /// <remarks>
    /// <p>Objects or components that need to know when the player moves (e.g., an animator controller) should
    /// subscribe to the <c>Moved</c> event of a movement controller.</p>
    /// <p>Prerequisites: The active action map of the <c>PlayerInput</c> component must have:
    /// <list type="bullet">
    ///     <item>An input action named "Move" that encapsulates a <c>Vector2</c> value</item>
    ///     <item>An input action named "Sprint"</item>
    /// </list>
    /// Note that this component will fail gracefully (and silently) if the required input actions are not found.</p>
    /// <p>Though not a strict requirement, this script ideally should precede <c>Player.Body</c> in the Script
    /// Execution Order. <see href="https://docs.unity3d.com/Manual/ExecutionOrder.html"/></p>
    /// </remarks>
    public abstract class Movement : BodyController
    {
        /// <summary>
        /// Dispatched to registered listeners when the player character receives movement commands.
        /// </summary>
        public event System.Action<EventArgs> Moved;

        /// <summary>
        /// Walk speed of the character in m/s.
        /// </summary>
        [Header("Player Motion")] [SerializeField, Range(1.0f, 5.0f), Tooltip("Walk speed of the character in m/s")]
        protected float walkSpeed = 2.0f;

        /// <summary>
        /// Sprint speed of the character in m/s.
        /// </summary>
        [SerializeField, Range(2.0f, 10.0f), Tooltip("Sprint speed of the character in m/s")]
        protected float sprintSpeed = 5.335f;

        /// <summary>
        /// Use to adjust the rate of acceleration/deceleration.
        /// </summary>
        [SerializeField, Range(1.0f, 10.0f), Tooltip("Acceleration and deceleration")]
        private float _speedChangeRate = 10.0f;

        /// <summary>
        /// The current direction of motion.
        /// </summary>
        /// <remarks>
        /// Derived movements must override this to implement their specific behaviour.
        /// </remarks>
        protected abstract Vector3 Direction { get; }

        /// <summary>
        /// Indicates whether the motion is a sprint or a walk.
        /// </summary>
        protected bool IsSprinting { get; private set; }

        /// <summary>
        /// The speed at which this movement drives the character body.
        /// </summary>
        /// <remarks>
        /// To
        /// </remarks>
        protected float TargetSpeed { get; set; }

        /// <summary>
        /// Cached magnitude of the last input vector.
        /// </summary>
        private float InputMagnitude { get; set; }

        protected virtual void Update()
        {
            float speed = Accelerate();
            if (speed > 0)
            {
                MoveBody(speed);
            }
            else
            {
                Stop();
            }
        }

        /// <summary>
        /// Apply simple acceleration to smoothly reach the target speed.
        /// </summary>
        private float Accelerate()
        {
            const float speedOffset = 0.01f;

            Vector3 velocity = body.Velocity;
            float targetSpeed = TargetSpeed;
            float planarSpeed = Mathf.Sqrt(velocity.sqrMagnitude - (velocity.y * velocity.y)); // Pythagoras

            return Mathf.Abs(targetSpeed - planarSpeed) > speedOffset
                ? Mathf.Lerp(planarSpeed, targetSpeed, Time.deltaTime * _speedChangeRate)
                : targetSpeed;
        }

        /// <summary>
        /// Gives motion to the character body.
        /// </summary>
        /// <remarks>
        /// Notifies any objects listening to the <c>Moved</c> event that the player has moved.
        /// </remarks>
        protected virtual void MoveBody(float speed)
        {
            body.Force = Direction * speed;
            Moved?.Invoke(new(this));
        }

        /// <summary>
        /// Initiates a simple directional movement.
        /// </summary>
        /// <param name="move">indicates the direction and magnitude of the input movement</param>
        /// <remarks>
        /// We separate this from the <c>OnMove</c> handler and expose it publicly so that movement can be driven
        /// programmatically if desired (e.g., in a non-interactive narrative sequence).
        /// </remarks>
        public abstract void Go(Vector2 move);

        /// <summary>
        /// Brings the character to a smooth stop according to the acceleration of the character body.
        /// </summary>
        /// <remarks>
        /// We separate this from the <c>OnMove</c> handler and expose it publicly so that movement can be stopped
        /// programmatically if desired (e.g., in a non-interactive narrative sequence).
        /// </remarks>
        public void Stop()
        {
            body.Force = Vector3.zero;
            TargetSpeed = 0f;
        }

        /// <summary>
        /// Brings the character to a smooth stop according to the acceleration of the character body.
        /// </summary>
        /// <remarks>
        /// We separate this from the <c>OnMove</c> handler and expose it publicly so that we can programmatically
        /// switch between walking and sprinting if desired (e.g., in a non-interactive narrative sequence).
        /// </remarks>
        public void ToggleSprint()
        {
            IsSprinting = !IsSprinting;
            TargetSpeed = TargetSpeed == 0f ? 0f : IsSprinting ? sprintSpeed : walkSpeed;
        }

        /// <summary>
        /// Handles "Move" input actions triggered by the input system.
        /// </summary>
        /// <param name="context">should represent a "Value" action with <c>Vector2</c> control type</param>
        /// <remarks>Start moving the character when the "Move" action is started/performed, and stop the character
        /// movement when the "Move" action is canceled.</remarks>
        private void OnMove(InputAction.CallbackContext context)
        {
            Vector2 move = context.ReadValue<Vector2>();

            if (context.canceled || move == Vector2.zero)
            {
                InputMagnitude = 0;
                Stop();
            }
            else
            {
                InputMagnitude = move.magnitude;
                Go(move);
            }
        }

        /// <summary>
        /// Handles "Sprint" input actions triggered by the input system.
        /// </summary>
        /// <param name="context">
        /// should represent a "Button" action in the <c>performed</c> phase, though we never inspect the context
        /// </param>
        private void OnSprint(InputAction.CallbackContext context)
        {
            ToggleSprint();
        }

        /// <inheritdoc/>
        /// <remarks>This controller responds to "Move" and "Sprint" input actions.</remarks>
        protected override void RegisterEventHandlers(PlayerInput input)
        {
            InputAction moveAction = input.actions.FindAction("Move");
            if (moveAction != null)
            {
                moveAction.canceled += OnMove;
                moveAction.performed += OnMove;
            }

            InputAction sprintAction = input.actions.FindAction("Sprint");
            if (sprintAction != null)
            {
                sprintAction.performed += OnSprint;
            }
        }

        /// <inheritdoc/>
        /// <remarks>This controller responds to "Move" and "Sprint" input actions.</remarks>
        protected override void UnregisterEventHandlers(PlayerInput input)
        {
            InputAction moveAction = input.actions.FindAction("Move");
            if (moveAction != null)
            {
                moveAction.canceled -= OnMove;
                moveAction.performed -= OnMove;
            }

            InputAction sprintAction = input.actions.FindAction("Sprint");
            if (sprintAction != null)
            {
                sprintAction.performed -= OnSprint;
            }
        }

        /// <summary>
        /// Inner class representing event data related to the motion of the character.
        /// </summary>
        public class EventArgs : System.EventArgs
        {
            /// <summary>
            /// Movement component that generated the event.
            /// </summary>
            private readonly Movement _movement;

            /// <summary>
            /// The most recent input magnitude (as a scalar) used in the character motion.
            /// </summary>
            public float Magnitude => _movement.InputMagnitude;

            /// <summary>
            /// The current rate-of-change in linear velocity for the character motion.
            /// </summary>
            public float ChangeRate => _movement._speedChangeRate;

            /// <summary>
            /// The movement speed that the character is trying to reach.
            /// </summary>
            public float TargetSpeed => _movement.TargetSpeed;

            /// <summary>
            /// Initialize event arguments bound to the given <c>Movement</c> component.
            /// </summary>
            /// <param name="movement">component that provides motion data for this <c>EventArgs</c></param>
            internal EventArgs(Movement movement) { _movement = movement; }
        }
    }
}