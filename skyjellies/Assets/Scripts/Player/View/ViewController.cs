using UnityEngine;
using UnityEngine.InputSystem;

namespace Player.View
{
    /// <summary>
    /// Inherit from this base class to implement different input-driven view perspectives the player character.
    /// </summary>
    /// <remarks>
    /// <p>The active action map of the <c>PlayerInput</c> component must have an input action named "Look" that
    /// encapsulates a <c>Vector2</c> value. Note that this component will fail gracefully (and silently) if the
    /// required input action is not found.</p>
    /// <p>Note that this component will fail gracefully (and silently) if the required input actions are not found.</p>
    /// </remarks>
    public abstract class ViewController : InputHandler
    {
        /// <summary>
        /// The target GameObject that the Cinemachine Virtual Camera that the camera will follow.
        /// </summary>
        /// <remarks>
        /// This reference must be set manually in the Unity editor.
        /// </remarks>
        [Header("Cinemachine")]
        [SerializeField, Tooltip("The follow target set in the Cinemachine Virtual Camera that the camera will follow")]
        protected GameObject cinemachineCameraTarget;

        /// <summary>
        /// Upper bound for camera pitch angle.
        /// </summary>
        [SerializeField, Range(0, 90), Tooltip("How far in degrees can you move the camera up")]
        protected float topClamp = 90.0f;

        /// <summary>
        /// Lower bound for camera pitch angle.
        /// </summary>
        [SerializeField, Range(-90, 0), Tooltip("How far in degrees can you move the camera down")]
        protected float bottomClamp = -90.0f;

        /// <summary>
        /// Controls the sensitivity of camera rotation.
        /// </summary>
        /// <remarks>
        /// \todo Determine whether this field is necessary or even useful.
        /// </remarks>
        [SerializeField, Range(0.001f, 10f), Tooltip("Controls the sensitivity of camera movement")]
        protected float cameraSensitivity = 1f;

        /// <summary>
        /// Inputs below this magnitude do not trigger view updates.
        /// </summary>
        protected const float LookThreshold = 0.01f;

        /// <summary>
        /// Cache the most recent "Look" input for use during the player loop.
        /// </summary>
        protected Vector2 lookDirection;

        /// <summary>
        /// Yaw angle of the target object followed by a cinemachine virtual camera configured for "3rd Person Follow".
        /// </summary>
        protected float cinemachineTargetYaw;

        /// <summary>
        /// Pitch angle of the target object followed by a cinemachine virtual camera configured for "3rd Person Follow".
        /// </summary>
        protected float cinemachineTargetPitch;

        protected void Awake()
        {
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible   = false;
        }

        protected void LateUpdate()
        {
            TurnCamera();
        }

        /// <summary>
        /// Smoothly adjust the camera orientation.
        /// </summary>
        protected abstract void TurnCamera();

        /// <summary>
        /// Update the view to align with the latest "Look" input.
        /// </summary>
        /// <remarks>
        /// We separate this from the <c>OnLook</c> handler and expose it publicly so that camera orientation can be
        /// changed programmatically if desired.
        /// \todo Decide whether we need this function since it is currently just a pass-through for the input vector.
        /// </remarks>
        public void Look(Vector2 look)
        {
            lookDirection = look * cameraSensitivity;
        }

        /// <summary>
        /// Handles "Look" input actions triggered by the input system.
        /// </summary>
        /// <param name="context">should represent a "Value" action with <c>Vector2</c> control type</param>
        private void OnLook(InputAction.CallbackContext context)
        {
            Look(context.ReadValue<Vector2>());
        }

        /// <inheritdoc/>
        /// <remarks>This controller responds to "Look" input actions.</remarks>
        protected override void RegisterEventHandlers(PlayerInput input)
        {
            InputAction lookAction = input.actions.FindAction("Look");
            if (lookAction != null)
            {
                lookAction.started   += OnLook;
                lookAction.performed += OnLook;
                lookAction.canceled  += OnLook;
            }
        }

        /// <inheritdoc/>
        /// <remarks>This controller responds to "Look" input actions.</remarks>
        protected override void UnregisterEventHandlers(PlayerInput input)
        {
            InputAction lookAction = input.actions.FindAction("Look");
            if (lookAction != null)
            {
                lookAction.started   -= OnLook;
                lookAction.performed -= OnLook;
                lookAction.canceled  -= OnLook;
            }
        }

        /// <summary>
        /// Indicates whether or not "Look" actions are coming from a mouse device.
        /// </summary>
        /// <remarks>
        /// \todo Do we need actually this to support other devices or switching of input control schemes?
        /// </remarks>
        protected bool IsCurrentDeviceMouse => true;

        /// <summary>
        /// Used to ensure that our camera angles (in degrees) remain within a given range.
        /// </summary>
        /// <param name="lfAngle">an angle in degrees, possibly outside the range [<c>lfMin</c>, <c>lfMax</c>].</param>
        /// <param name="lfMin">lower bound for the output angle</param>
        /// <param name="lfMax">upper bound for the output angle</param>
        /// <returns>an angle in degrees guaranteed to be within [<c>lfMin</c>, <c>lfMax</c>]</returns>
        protected static float ClampAngle(float lfAngle, float lfMin, float lfMax)
        {
            if (lfAngle < -360f) { lfAngle += 360f; }

            if (lfAngle > 360f) { lfAngle -= 360f; }

            return Mathf.Clamp(lfAngle, lfMin, lfMax);
        }
    }
}
